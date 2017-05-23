using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Data.Common;


namespace ReceiptExport
{
    public enum ExitCode : int
    {
        Success = 0,
        InvalidParameter = 1,
        CreateLogFileError = 6,
        DuplicateWorksheet = 7,
        CreateReceiptFileError = 8,
        CreateFlagFileError = 9,
        InvalidRecordLength = 10,
        NoRecordsFound = 11,
        InvalidData = 12,
        UnknownError = 99
    }


    class Program
    {
        static string fileDir;
        public static string FileDir
        {
            get { return fileDir; }
        }

        static string receiptFilePath;
        static string flagFilePath;
        static string flagFileName;
        static string errMsg;
        static string paramServer;

        static int record05Length = 300;
        static int addressErrorCount = 0;
        static int NumOf05RecordsWritten = 0;
        static int total05Records = 0;
        static DateTime CurrentDate = DateTime.Now;
        static string itemprocCS = ConfigurationManager.ConnectionStrings["itemCS"].ToString();

        enum PredepositStatus : byte { None, PreDeposit, Released, Unprocessed };

        static bool NotPredeposited(byte? status)
        {
            if (status == null)
                return true;
            else if (status == (byte?)PredepositStatus.None)
                return true;
            else if (status == (byte?)PredepositStatus.Released)
                return true;
            else
                return false;
        }

        static bool Predeposited(byte? status)
        {
            if (status == (byte?)PredepositStatus.PreDeposit)
                return true;
            else if (status == (byte?)PredepositStatus.Unprocessed)
                return true;
            else
                return false;
        }

        delegate bool MyDelegate(byte? num); 

        static IEnumerable<Receipt> FilterReceipts(IEnumerable<Receipt> receipts, MyDelegate FilterBy)
        {
            foreach(var receipt in receipts)
            {
                if (FilterBy(receipt.PredepositStatus))
                    yield return receipt;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                //GetParameters(args);
                CreateLogFile();            
                ProcessRecords();
                CreateFlagFile();
                CloseLogFile();            
                System.Environment.Exit((int)ExitCode.Success); //exit
            }
            catch(Exception ex)
            {
                Log.Exit(ex.ToString(), ExitCode.UnknownError);
            }                
        } 

        private static List<Receipt> GetReceiptList()
        {
            using(var db = new ReceiptDBContext())
            {
                db.Database.CommandTimeout = 1800;
                List<Receipt> receiptList = db.Database.SqlQuery<Receipt>("proc_Custom_ReceiptExport").ToList();

                return receiptList;
            }
        }

        private static void ProcessRecords()
        {
            /// ALL RECEIPTS    
            List<Receipt> allReceipts = GetReceiptList().OrderBy(x => x.GlobalStubID).ToList<Receipt>();
            Log.WriteLine(String.Format("GetReceipts() returned {0} records.{1}", allReceipts.Count, Environment.NewLine));
            
            /// UNIDENTIFIED RECEIPTS
            IEnumerable<Receipt> unidentifiedReceipts = allReceipts.Where(x => x.PersonID == "0").ToList();
            using (var db = new ReceiptDBContext())
            {
                foreach (var receipt in unidentifiedReceipts)
                {
                    Batch b = db.Batches.FirstOrDefault(x => x.GlobalBatchID == receipt.GlobalBatchID);
                    b.DEStatus = 3;
                }
                db.SaveChanges();
            }

            /// SKIPPED RECEIPTS
            IEnumerable<Receipt> skippedReceipts = FilterReceipts(allReceipts, Predeposited);

            foreach(var skippedReceipt in skippedReceipts)
            {
                Log.WriteLine("Skipped Stub:" + skippedReceipt.GlobalStubID + " - PredepositStatus: " + skippedReceipt.PredepositStatus);
            }

            /// NORMAL RECEIPTS
            List<Receipt> receipts = FilterReceipts(allReceipts, NotPredeposited).OrderBy(x => x.GlobalBatchID).ToList();

            if (receipts.Count > 0)
            {
                RecordType01 rec01 = new RecordType01();//header
                rec01.CreationStamp = CurrentDate;

                RecordType05[] rec05 = new RecordType05[receipts.Count];//detail

                using (var db = new ReceiptDBContext())
                {
                    int i = 0;

                    foreach (Receipt receipt in receipts)
                    {
                        rec05[i] = new RecordType05();

                        rec05[i].SduBatchId = receipt.GlobalBatchID.ToString();
                        rec05[i].SduTranId = String.IsNullOrEmpty(receipt.SDUTranID)? CreateSduTranID(receipt.GlobalStubID): receipt.SDUTranID;
                        rec05[i].ReceiptNumber = receipt.RTNumber;
                        rec05[i].RetransmittalIndicator = IsRetransmittal(receipt); // processingDate is required
                        rec05[i].PayorID = receipt.PersonID.Trim() == "0" ? "AR00000000000" : receipt.PersonID;
                        rec05[i].PayorSSN = receipt.SSN;
                        rec05[i].PaidBy = receipt.PaidBy;
                        rec05[i].PayorLastName = receipt.LastName;
                        rec05[i].PayorFirstName = receipt.FirstName;
                        rec05[i].PayorMiddleName = receipt.MiddleName;
                        rec05[i].PayorSuffix = receipt.Suffix;
                        rec05[i].Amount = receipt.Amount ?? 0;
                        rec05[i].OfcAmount = receipt.OFCAmount ?? 0;
                        rec05[i].PaymentMode = receipt.PaymentMode;
                        rec05[i].PaymentSource = receipt.PaymentSource;
                        rec05[i].ReceiptReceivedDate = receipt.ProcessingDate.ToString();
                        rec05[i].ReceiptEffectiveDate = receipt.EffectiveDate.ToString();
                        rec05[i].CheckNumber = receipt.Serial;
                        rec05[i].ComplianceExemptionReason = GetComplianceExemptionReason(receipt).ToString();
                        rec05[i].TargetedPaymentIndicator = receipt.TargetedPayment;
                        rec05[i].Fips = receipt.FIPS;
                        rec05[i].CourtCaseNumber = receipt.CaseNumber;
                        rec05[i].CourtJudgementNumber = receipt.CourtJudgment;
                        rec05[i].CourtGuidelineNumber = receipt.CourtGuideline;
                        rec05[i].ReasonCode = receipt.ReasonCode;

                        StubsDataEntry sde = db.StubsDataEntries.FirstOrDefault(x => x.GlobalStubID == receipt.GlobalStubID);
                        if (sde != null)
                        {
                            sde.ExportedToCHARTSDate = CurrentDate;
                            sde.SDUTranID = rec05[i].SduTranId;
                            sde.CHARTSStubPrefix = rec05[i].SduTranId.Substring(0, 8);
                            sde.ExportedAsUnidentified = receipt.ExportedAsUnidentified == 1 ? receipt.ExportedAsUnidentified : (receipt.PersonID == "0" ? (byte)1 : (byte)0);
                            sde.ExportedToCHARTS = 1;
                            sde.ComplianceExemptionReason = GetComplianceExemptionReason(receipt);
                            if (rec05[i].RetransmittalIndicator && rec05[i].PayorID != "AR00000000000")
                                sde.ResolvedDate = CurrentDate;
                        }
                        else
                        {
                            Log.WriteLine("Stub not found, not updated. GlobalStubID: " + receipt.GlobalStubID);
                        }

                        //update vertical and horizontal tables
                        SqlParameter pGlobalStubID = new SqlParameter("globalStubID", receipt.GlobalStubID);
                        db.Database.ExecuteSqlCommand("proc_Custom_ReceiptExport_UpdateStubDE @GlobalStubID", pGlobalStubID);

                        rec01.TotalAmount += rec05[i].Amount;

                        if (rec05[i].RetransmittalIndicator)
                        {
                            rec01.RetransmittalRecordCount++;
                            rec01.RetransmittalAmount += rec05[i].Amount;
                        }
                        else
                        {
                            rec01.FirstTimeRecordCount++;
                            rec01.FirstTimeAmount += rec05[i].Amount;
                        }

                        i++;
                    }

                    db.SaveChanges();


                    rec01.RecordCount = i;
                    RecordTotals(rec01);
                    WriteToReceiptFile(rec01, rec05);
                }
            }
            else
            {
                //no records found
                throw new CustomException("Receipt Export found " + receipts.Count + " to process", ExitCode.NoRecordsFound);
            }
        }

        private static void RecordTotals(RecordType01 rec01)
        {
            Log.WriteLine(Environment.NewLine);
            Log.WriteLine("First Time Amount: " + rec01.FirstTimeAmount);
            Log.WriteLine("Retransmitted Amount: "+ rec01.RetransmittalAmount);
            Log.WriteLine("Total Amount: " + rec01.TotalAmount);
            Log.WriteLine("First Time Records: " + rec01.FirstTimeRecordCount.ToString());
            Log.WriteLine("Retransmitted Records: " + rec01.RetransmittalRecordCount.ToString());
            Log.WriteLine("Total Records: " + rec01.RecordCount.ToString());
        }

        public static StreamWriter CreateReceiptFile()
        {
            StreamWriter sw = null;
            try
            {
                receiptFilePath = String.Format("{0}RCP{1:MMddyyyy}.dat", FileDir, DateTime.Now);

                if (!File.Exists(receiptFilePath))
                {
                    sw = new StreamWriter(receiptFilePath);
                }
                else
                    throw new CustomException("File already exists: " + receiptFilePath, ExitCode.CreateReceiptFileError);

                return sw;
            }
            catch
            {
                throw;
            }
        }

        private static void WriteToReceiptFile(RecordType01 rec01, RecordType05[] rec05)
        {
            try
            {
                using (StreamWriter receiptWriter = CreateReceiptFile())
                {
                    receiptWriter.Write(rec01.RecordLine());

                    for (int i = 0; i < rec05.Length; i++)
                    {
                        string currentRecord = rec05[i].RecordLine();

                        if (currentRecord.Length == record05Length) // and predepositstatus is null or released*******************************
                            receiptWriter.Write(currentRecord);
                        else
                            LogErrorColumns(rec05[i]);
                    }

                    receiptWriter.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        private static byte? GetComplianceExemptionReason(Receipt receipt)
        {
            if (receipt.ExportedToCHARTSDate == null)
                return null;    // normal
            else if (receipt.ExportedAsUnidentified == 1)
                return 0;       // unidentified
            else if (receipt.PredepositStatus == (byte?)PredepositStatus.Released)
                return 1;       // released from predeposit
            else
                return 2;       // other
        }

        private static bool IsRetransmittal(Receipt receipt)
        {
            try
            {
                if (receipt.ExportedToCHARTSDate == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                throw;
            }
        }

        private static string CreateSduTranID(int globalStubID)
        {
            try
            {
                string chartsStubPrefix = CurrentDate.Year.ToString().PadLeft(2, '0')
                                                + CurrentDate.Month.ToString().PadLeft(2, '0')
                                                + CurrentDate.Day.ToString().PadLeft(2, '0');

                return chartsStubPrefix + globalStubID.ToString().PadLeft(12, '0'); //EX: 20100101000012345678
            }
            catch
            {
                throw;
            }
        }

        public static void EmailNotification(int _exitCode, string msg)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());

            if (_exitCode == (int)ExitCode.Success)
            {
                mail.Subject = "ReceiptExport: Successful Process";
                mail.To.Add(ConfigurationManager.AppSettings["NotifyEmail"].ToString());
                mail.Body = "ReceiptExport job processed <b>successfully</b>. <br /><br />";
                mail.Body += "Data file: " + fileDir + " <br />";
                mail.Body += "See attached log file for additional details.";
            }
            else
            {
                mail.Subject = "ReceiptExport Failed";
                mail.To.Add(ConfigurationManager.AppSettings["ItStaffEmail"].ToString());
                mail.Body = "ReceiptExport job <b>failed</b> with exit code " + _exitCode + ".<br /><br />";
                mail.Body += "See attached log file for additional details.<br/><br/>";
                mail.Body += msg;
            }

            mail.IsBodyHtml = true;

            bool fileExists = File.Exists(Log.FilePath);
            if (fileExists)
            {
                mail.Attachments.Add(new Attachment(Log.FilePath));
            }

            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["MailHost"].ToString());
            smtp.Send(mail);

        }

        private static void CreateLogFile()
        {
            try
            {
                fileDir = ConfigurationManager.AppSettings["fileDir"].ToString();
                Log.CreateLogFile(FileDir);
            }
            catch(Exception ex)
            {
                throw new CustomException("CreateLogFile() error, exit_code: " + ExitCode.CreateLogFileError, ex);
            }
        }

        private static void CloseLogFile()
        {
            try
            {
                Log.Close();
            }
            catch { throw; }
        }

        private static void CreateFlagFile()
        {
            try
            {
                flagFileName = Path.GetFileNameWithoutExtension(receiptFilePath);
                flagFilePath = String.Format("{0}{1}.flg", FileDir, flagFileName);

                File.CreateText(flagFilePath);
            }
            catch(Exception ex)
            {
                throw new CustomException("CreateFlagFile() exit_code: " + ExitCode.CreateFlagFileError, ex);
            }
        }

        private static void LogErrorColumns(RecordType05 rec05)
        {
            errMsg = String.Format("Record: {0}.  Invalid length: {1}", rec05.SduTranId, rec05.RecordLine().Length);

            if (rec05.SduBatchId.Length != (int)FieldLength.SduBatchId)
                errMsg += String.Format("{0}SduBatchId length: {1}. Max is: {2}", Environment.NewLine, rec05.SduBatchId.Length, FieldLength.SduBatchId);
            if (rec05.SduTranId.Length != (int)FieldLength.SduTranId)
                errMsg += String.Format("{0}SduTranId length: {1}. Max is: {2}", Environment.NewLine, rec05.SduTranId.Length, FieldLength.SduTranId);
            if (rec05.ReceiptNumber.Length != (int)FieldLength.ReceiptNumber)
                errMsg += String.Format("{0}ReceiptNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptNumber.Length, FieldLength.ReceiptNumber);
            if (rec05.StrRetransmittalIndicator.Length != (int)FieldLength.RetransmittalIndicator)
                errMsg += String.Format("{0}RetransmittalIndicator length: {1}. Max is: {2}", Environment.NewLine, rec05.StrRetransmittalIndicator.Length, FieldLength.RetransmittalIndicator);
            if (rec05.PayorID.Length != (int)FieldLength.PayorID)
                errMsg += String.Format("{0}PayorID length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorID.Length, FieldLength.PayorID);
            if (rec05.PayorSSN.Length != (int)FieldLength.PayorSSN)
                errMsg += String.Format("{0}PayorSSN length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorSSN.Length, FieldLength.PayorSSN);
            if (rec05.PaidBy.Length != (int)FieldLength.PaidBy)
                errMsg += String.Format("{0}PaidBy length: {1}. Max is: {2}", Environment.NewLine, rec05.PaidBy.Length, FieldLength.PaidBy);
            if (rec05.PayorLastName.Length != (int)FieldLength.PayorLastName)
                errMsg += String.Format("{0}PayorLastName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorLastName.Length, FieldLength.PayorLastName);
            if (rec05.PayorFirstName.Length != (int)FieldLength.PayorFirstName)
                errMsg += String.Format("{0}PayorFirstName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorFirstName.Length, FieldLength.PayorFirstName);
            if (rec05.PayorMiddleName.Length != (int)FieldLength.PayorMiddleName)
                errMsg += String.Format("{0}PayorMiddleName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorMiddleName.Length, FieldLength.PayorMiddleName);
            if (rec05.PayorSuffix.Length != (int)FieldLength.PayorSuffix)
                errMsg += String.Format("{0}PayorSuffix length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorSuffix.Length, FieldLength.PayorSuffix);
            if (rec05.StrAmount.Length != (int)FieldLength.Amount)
                errMsg += String.Format("{0}Amount length: {1}. Max is: {2}", Environment.NewLine, rec05.StrAmount.Length, FieldLength.Amount);
            if (rec05.StrOfcAmount.Length != (int)FieldLength.OfcAmount)
                errMsg += String.Format("{0}OfcAmount length: {1}. Max is: {2}", Environment.NewLine, rec05.StrOfcAmount.Length, FieldLength.OfcAmount);
            if (rec05.PaymentMode.Length != (int)FieldLength.PaymentMode)
                errMsg += String.Format("{0}PaymentMode length: {1}. Max is: {2}", Environment.NewLine, rec05.PaymentMode.Length, FieldLength.PaymentMode);
            if (rec05.PaymentSource.Length != (int)FieldLength.PaymentSource)
                errMsg += String.Format("{0}PaymentSource length: {1}. Max is: {2}", Environment.NewLine, rec05.PaymentSource.Length, FieldLength.PaymentSource);
            if (rec05.ReceiptReceivedDate.Length != (int)FieldLength.ReceiptReceivedDate)
                errMsg += String.Format("{0}ReceiptReceivedDate length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptReceivedDate.Length, FieldLength.ReceiptReceivedDate);
            if (rec05.ReceiptEffectiveDate.Length != (int)FieldLength.ReceiptEffectiveDate)
                errMsg += String.Format("{0}ReceiptEffectiveDate length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptEffectiveDate.Length, FieldLength.ReceiptEffectiveDate);
            if (rec05.CheckNumber.Length != (int)FieldLength.CheckNumber)
                errMsg += String.Format("{0}CheckNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CheckNumber.Length, FieldLength.CheckNumber);
            if (rec05.ComplianceExemptionReason.Length != (int)FieldLength.ComplianceExemptionReason)
                errMsg += String.Format("{0}ComplianceExemptionReason length: {1}. Max is: {2}", Environment.NewLine, rec05.ComplianceExemptionReason.Length, FieldLength.ComplianceExemptionReason);
            if (rec05.TargetedPaymentIndicator.Length != (int)FieldLength.TargetedPaymentIndicator)
                errMsg += String.Format("{0}TargetedPaymentIndicator length: {1}. Max is: {2}", Environment.NewLine, rec05.TargetedPaymentIndicator.Length, FieldLength.TargetedPaymentIndicator);
            if (rec05.Fips.Length != (int)FieldLength.Fips)
                errMsg += String.Format("{0}Fips length: {1}. Max is: {2}", Environment.NewLine, rec05.Fips.Length, FieldLength.Fips);
            if (rec05.CourtCaseNumber.Length != (int)FieldLength.CourtCaseNumber)
                errMsg += String.Format("{0}CourtCaseNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtCaseNumber.Length, FieldLength.CourtCaseNumber);
            if (rec05.CourtJudgementNumber.ToString().Length != (int)FieldLength.CourtJudgementNumber)
                errMsg += String.Format("{0}CourtJudgementNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtJudgementNumber.Length, FieldLength.CourtJudgementNumber);
            if (rec05.CourtGuidelineNumber.ToString().Length != (int)FieldLength.CourtGuidelineNumber)
                errMsg += String.Format("{0}CourtGuidelineNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtGuidelineNumber.Length, FieldLength.CourtGuidelineNumber);
            if (rec05.ReasonCode.Length != (int)FieldLength.ReasonCode)
                errMsg += String.Format("{0}ReasonCode length: {1}. Max is: {2}", Environment.NewLine, rec05.ReasonCode.Length, FieldLength.ReasonCode);

            Log.WriteLine(errMsg);
        }

    }
}