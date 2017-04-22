using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Data.SqlClient;
using ReceiptExport.classes;
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
        static string kidcareConnString;
        static string itemprocessingConnString;

        static int record05Length = 300;
        static int addressErrorCount = 0;
        static int NumOf05RecordsWritten = 0;
        static int total05Records = 0;
        static StreamWriter receiptWriter;
        static DateTime setChartsExportedDate = DateTime.Now;
        static string itemprocCS = ConfigurationManager.ConnectionStrings["itemCS"].ToString();


        static void Main(string[] args)
        {
            //GetParameters(args);
            fileDir = ConfigurationManager.AppSettings["fileDir"].ToString();
            Log.CreateLogFile(FileDir);
            CreateOutputFile();
            ProcessRecords();
            receiptWriter.Close();
            Log.Close();            

            System.Environment.Exit((int)ExitCode.Success); //exit
        }

        private static DataTable GetReceipts()
        {
            DbProviderFactory factory = Database.GetFactory();

            using (IDbConnection con = factory.CreateConnection())
            {
                con.ConnectionString = itemprocCS;
                string sqlString = "ReceiptExport";

                IDbCommand cmd = factory.CreateCommand();
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Connection = con;
                cmd.Connection.Open();

                IDataReader rdr = cmd.ExecuteReader();

                IDataAdapter adapter = factory.CreateDataAdapter();
                DataSet ds = new DataSet();

                adapter.Fill(ds);

                DataTable table = (DataTable)ds.Tables["#tmp_ReceiptExport"];
                return table;
            }
        }

        public static List<Receipt> GetReceiptList()
        {
            using(var db = new ReceiptDBContext())
            {
                List<Receipt> receiptList = db.Database.SqlQuery<Receipt>("ReceiptExport").ToList<Receipt>();

                return receiptList;
            }
        }

        private static void ProcessRecords()
        {            
            //header record
            RecordType01 rec01 = new RecordType01();
            rec01.CreationStamp = DateTime.Now;

            /*
            DataTable table = GetReceipts();
            Log.WriteLine(String.Format("GetReceipts() returned {0} records ", table.Rows.Count));
            */

            List<Receipt> receipts = GetReceiptList().OrderBy(x => x.GlobalStubID).ToList<Receipt>();



                if (receipts.Count > 0)
                {
                    //Create array of type 05 records
                    RecordType05[] rec05 = new RecordType05[receipts.Count];

                    //DataView dv = table.DefaultView;
                    //dv.Sort = "globalbatchid asc";



                    //DataTable sortedDT = dv.ToTable();

                    int i = 0;
                    string prevGlobalBatchId;
                    bool batchHasUnidentified = false;

                    #region loop records
                    foreach (Receipt receipt in receipts)
                    {
                        prevGlobalBatchId = receipt.GlobalBatchID;

                        if (receipt.ExportedAsUnidentified == "1")
                            batchHasUnidentified = true;


                        rec05[i] = new RecordType05();

                        rec05[i].SduBatchId = receipt.GlobalBatchID;
                        rec05[i].SduTranId = receipt.SDUTranID;
                        rec05[i].ReceiptNumber = receipt.RTNumber;

                        #region retransmital
                        DateTime exportedToChartsDate;
                        DateTime processingDate;
                        if (DateTime.TryParse(row["ProcessingDate"].ToString(), out processingDate))
                        {
                            if (DateTime.TryParse(row["ExportedToCHARTSDate"].ToString(), out exportedToChartsDate))
                            {
                                //If ProcessingDate is later than exportedToChartsDate
                                if (DateTime.Compare(processingDate, exportedToChartsDate) > 0)
                                    rec05[i].RetransmittalIndicator = true;
                                else
                                    rec05[i].RetransmittalIndicator = false;
                            }
                            else { } // processingDate is required
                        }
                        #endregion
                        rec05[i].PayorID = row["ARP"].ToString().Trim();
                        rec05[i].PayorSSN = row["SSN"].ToString().Trim();
                        rec05[i].PaidBy = row["PaidBy"].ToString().Trim();
                        rec05[i].PayorLastName = row["LastName"].ToString().Trim();
                        rec05[i].PayorFirstName = row["FirstName"].ToString().Trim();
                        rec05[i].PayorMiddleName = row["MiddleName"].ToString().Trim();
                        rec05[i].PayorSuffix = row["Suffix"].ToString().Trim();
                        rec05[i].Amount = String.IsNullOrEmpty(row["Amount"].ToString().Trim()) ?
                                                0 : Double.Parse(row["Amount"].ToString().Trim());
                        rec05[i].OfcAmount = String.IsNullOrEmpty(row["OFCAmount"].ToString().Trim()) ?
                                                0 : Double.Parse(row["OFCAmount"].ToString().Trim());
                        rec05[i].PaymentMode = row["PaymentMode"].ToString().Trim();
                        rec05[i].PaymentSource = row["PaymentSource"].ToString().Trim();
                        rec05[i].ReceiptReceivedDate = row["ProcessingDate"].ToString().Trim();
                        rec05[i].ReceiptEffectiveDate = row["EffectiveDate"].ToString().Trim();
                        rec05[i].CheckNumber = row["Serial"].ToString().Trim();
                        rec05[i].ComplianceExemptionReason = row["ComplianceExemptionReason"].ToString().Trim();
                        rec05[i].TargetedPaymentIndicator = row["TargetedPayment"].ToString().Trim();
                        rec05[i].Fips = row["FIPS"].ToString().Trim();
                        rec05[i].CourtCaseNumber = row["CaseNumber"].ToString().Trim();
                        rec05[i].CourtJudgementNumber = row["CourtJudgment"].ToString().Trim();
                        rec05[i].CourtGuidelineNumber = row["CourtGuideline"].ToString().Trim();
                        rec05[i].ReasonCode = row["ReasonCode"].ToString().Trim();

                        string currentRecord = rec05[i].RecordLine();

                        if (currentRecord.Length != record05Length)
                            LogErrorColumns(rec05[i]);

                        //-----------------------------------------------------------------------------------------
                        //update StubsDataEntry
                        //SDUConnection ip_conn = new SDUConnection();

                        string chartsStubPrefix = setChartsExportedDate.Year.ToString() + setChartsExportedDate.Month.ToString() + setChartsExportedDate.Day.ToString();
                        string sduTranId = chartsStubPrefix + row["GlobalStubId"].ToString().PadLeft(12);




                        string sqlString = String.Format(
                            "UPDATE StubsDataEntry " +
                            "SET ExportedToCHARTS = 1" +
                            ", ExportedToCHARTSDate = '{0}'" +
                            ", SDUTranID = COALESCE(SDUTranID, '{1}')" +
                            ", CHARTSStubPrefix = COALESCE(CHARTSStubPrefix, '{2}') " +
                            ", ExportedAsUnidentified = {3}" +
                            "WHERE GlobalStubID = {4}"
                            , setChartsExportedDate
                            , sduTranId
                            , chartsStubPrefix
                            , row["ExportedAsUnidentified"].ToString()
                            , row["GlobalStubId"].ToString()
                            );


                    DbProviderFactory factory = Database.GetFactory();

                    using (IDbConnection con = factory.CreateConnection())
                    {
                        IDbCommand cmd = factory.CreateCommand();
                        cmd.CommandText = sqlString;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;

                        cmd.ExecuteNonQuery();
                    }



                        //conn.UpdateQuery(sqlString, itemprocessingConnString);

                        // IF THEREIS ITEM UNIDENTIFIED SET THE BATCH TO INCOMPLETE ************************
                       if (rec05[i].SduBatchId != prevGlobalBatchId)
                        {
                            if (batchHasUnidentified)
                            {
                                UpdateBatch(prevGlobalBatchId);
                                batchHasUnidentified = false;
                            }

                            prevGlobalBatchId = rec05[i].SduBatchId;
                        }
                        //-----------------------------------------------------------------------------------------

                        receiptWriter.WriteLine(currentRecord);

                        i++;

                        rec01.TotalAmount += rec05[i].Amount;
                        if (rec05[i].RetransmittalIndicator)
                        {
                            rec01.FirstTimeRecordCount++;
                            rec01.FirstTimeAmount += rec05[i].Amount;
                        }
                        else
                        {
                            rec01.RetransmittalRecordCount++;
                            rec01.RetransmittalAmount += rec05[i].Amount;
                        }
                    }

                    rec01.RecordCount = i;

                    #endregion

                }
                else
                {
                    ; //no records found 
                }

            }

        private static string UpdateBatch(string prevGlobalBatchId)
        {
            string sqlString = String.Format(
                    "UPDATE Batch SET DEStatus = 3 WHERE GlobalBatchID = {0}", prevGlobalBatchId);

            //con.UpdateQuery(sqlString, itemprocessingConnString);
            var factory = Database.GetFactory();

            using (IDbConnection conn = (Database.GetFactory()).CreateConnection())
            {
                IDbCommand cmd = factory.CreateCommand();
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            return sqlString;
        }

        //SDUConnection conn = new SDUConnection();
        //DataTable table = conn.GetDataTableWithStoredProcedure(sqlString, kidcareConnString);



        private static void GetParameters(string[] args)
        {
            int requiredNumberOfArguments = Int32.Parse(ConfigurationManager.AppSettings["CommandLineArguments"].ToString());
            if (args.Count() != requiredNumberOfArguments)
            {
                errMsg = String.Format("Number of arguments required: {0}", requiredNumberOfArguments.ToString());
                Log.Exit(errMsg, ExitCode.InvalidParameter);
            }
            else
            {
                for (int i = 0; i < args.Count(); i++)
                {
                    string param = args[i];
                    int pos = param.IndexOf('=') + 1;
                    int charCount = param.Length - pos;
                    string paramName = param.Substring(0, pos).ToUpper();
                    string paramValue = param.Substring(pos, charCount).ToUpper();

                    if (paramName == "SERVER=")
                    { paramServer = paramValue; }

                }

                ValidateServerParam(paramServer);
            }
        }

        private static void ValidateServerParam(string _serverName)
        {
            try
            {
                //Match command line server parameter with app.config value
                if (_serverName.Trim() == ConfigurationManager.AppSettings["server"].ToString())
                {
                    ////paramServer = _serverName.ToUpper();
                    //fileDir = ConfigurationManager.AppSettings["fileDir"].ToString();
                    //itemprocessingConnString = ConfigurationManager.ConnectionStrings["itemCS"].ConnectionString;
                }

                else
                {
                    EmailNotification((int)ExitCode.InvalidParameter);
                }
            }
            catch (Exception ex)
            {
                errMsg = "ValidateServerparam(): \n";
                errMsg += ex.Message;
                EmailNotification((int)ExitCode.UnknownError);
                // don't do this log doesn't exit yet.  Need parameter to create log.
                //Log.Exit(errMsg, ExitCode.UnknownError);
            }
        }

        public static void EmailNotification(int _exitCode)
        {
            //MailMessage mail = new MailMessage();

            //mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());



            //if (_exitCode == (int)ExitCode.Success)
            //{
            //    mail.Subject = "ReceiptExport: Successful Process";
            //    mail.To.Add(ConfigurationManager.AppSettings["NotifyEmail"].ToString());
            //    mail.Body = "ReceiptExport job processed <b>successfully</b>. <br /><br />";
            //    mail.Body += "Data file: " + fileDir + " <br />";
            //    mail.Body += "See attached log file for additional details.";
            //}
            //else
            //{
            //    mail.Subject = "ReceiptExport Failed";
            //    mail.To.Add(ConfigurationManager.AppSettings["ItStaffEmail"].ToString());
            //    mail.Body = "ReceiptExport job <b>failed</b> with exit code " + _exitCode + ".<br /><br />";
            //    mail.Body += "See attached log file for additional details.";
            //}

            //mail.IsBodyHtml = true;

            //bool fileExists = File.Exists(Log.FilePath);
            //if (fileExists)
            //{
            //    mail.Attachments.Add(new Attachment(Log.FilePath));
            //}

            //SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["MailHost"].ToString());
            //smtp.Send(mail);

        }

        public static void CreateOutputFile()
        {
            try
            {
                string strDate = String.Format("{0:MMddyyyy}", Log.CurrentDateTime);
                receiptFilePath = String.Format("{0}RCP{1}.dat", FileDir, strDate);

                if (!File.Exists(receiptFilePath))
                    receiptWriter = new StreamWriter(receiptFilePath);
                else
                    Log.Exit("File already exists: " + receiptFilePath, ExitCode.CreateReceiptFileError);
            }
            catch
            {
                errMsg = "CreateOutputFile()\n";
                errMsg += "Unable to Create Receipt Data File";
                Log.Exit(errMsg, ExitCode.CreateReceiptFileError);
            }
        }

        public static void CreateFlagFile()
        {
            try
            {
                flagFileName = Path.GetFileNameWithoutExtension(receiptFilePath);
                flagFilePath = String.Format("{0}{1}.flg", FileDir, flagFileName);

                File.CreateText(flagFilePath);
            }
            catch
            {
                errMsg = "Unable to Create Flag File";
                Log.Exit(errMsg, ExitCode.CreateFlagFileError);
            }
        }

        private static void PreExit()
        {
            try
            {
                Log.WriteLine("");//enter

                Log.WriteLine(String.Format("(05) Error Count: {0}", addressErrorCount));
                Log.WriteLine(String.Format("(05) Records Written: {0}", NumOf05RecordsWritten));
                Log.WriteLine(String.Format("(05) Total Records: {0}", total05Records));
                Log.WriteLine(String.Format("{0}----- Done: {1} -----", Environment.NewLine, DateTime.Now.ToString()));

                if (receiptWriter != null)
                    receiptWriter.Close();
                if (!Log.IsLogWriterNull())
                    Log.Close();

            }
            catch (Exception ex)
            {
                errMsg = Environment.NewLine + "Error in PreExit()";
                errMsg += ex.Message + ex.InnerException;
                Log.Exit(errMsg, ExitCode.UnknownError);
            }
        }

        private static void LogErrorColumns(RecordType05 rec05)
        {
            errMsg = String.Format("Record: {0}.  Invalid length: {1}", rec05.SduTranId, rec05.RecordLine().Length);

            if (rec05.SduBatchId.Length > (int)FieldLength.SduBatchId)
                errMsg += String.Format("{0}SduBatchId length: {1}. Max is: {2}", Environment.NewLine, rec05.SduBatchId.Length, FieldLength.SduBatchId);
            if (rec05.SduTranId.Length > (int)FieldLength.SduTranId)
                errMsg += String.Format("{0}SduTranId length: {1}. Max is: {2}", Environment.NewLine, rec05.SduTranId.Length, FieldLength.SduTranId);
            if (rec05.ReceiptNumber.Length > (int)FieldLength.ReceiptNumber)
                errMsg += String.Format("{0}ReceiptNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptNumber.Length, FieldLength.ReceiptNumber);
            if (rec05.StrRetransmittalIndicator.Length > (int)FieldLength.RetransmittalIndicator)
                errMsg += String.Format("{0}RetransmittalIndicator length: {1}. Max is: {2}", Environment.NewLine, rec05.StrRetransmittalIndicator.Length, FieldLength.RetransmittalIndicator);
            if (rec05.PayorID.Length > (int)FieldLength.PayorID)
                errMsg += String.Format("{0}PayorID length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorID.Length, FieldLength.PayorID);
            if (rec05.PayorSSN.Length > (int)FieldLength.PayorSSN)
                errMsg += String.Format("{0}PayorSSN length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorSSN.Length, FieldLength.PayorSSN);
            if (rec05.PaidBy.Length > (int)FieldLength.PaidBy)
                errMsg += String.Format("{0}PaidBy length: {1}. Max is: {2}", Environment.NewLine, rec05.PaidBy.Length, FieldLength.PaidBy);
            if (rec05.PayorLastName.Length > (int)FieldLength.PayorLastName)
                errMsg += String.Format("{0}PayorLastName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorLastName.Length, FieldLength.PayorLastName);
            if (rec05.PayorFirstName.Length > (int)FieldLength.PayorFirstName)
                errMsg += String.Format("{0}PayorFirstName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorFirstName.Length, FieldLength.PayorFirstName);
            if (rec05.PayorMiddleName.Length > (int)FieldLength.PayorMiddleName)
                errMsg += String.Format("{0}PayorMiddleName length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorMiddleName.Length, FieldLength.PayorMiddleName);
            if (rec05.PayorSuffix.Length > (int)FieldLength.PayorSuffix)
                errMsg += String.Format("{0}PayorSuffix length: {1}. Max is: {2}", Environment.NewLine, rec05.PayorSuffix.Length, FieldLength.PayorSuffix);
            if (rec05.StrAmount.Length > (int)FieldLength.Amount)
                errMsg += String.Format("{0}Amount length: {1}. Max is: {2}", Environment.NewLine, rec05.StrAmount.Length, FieldLength.Amount);
            if (rec05.StrOfcAmount.Length > (int)FieldLength.OfcAmount)
                errMsg += String.Format("{0}OfcAmount length: {1}. Max is: {2}", Environment.NewLine, rec05.StrOfcAmount.Length, FieldLength.OfcAmount);
            if (rec05.PaymentMode.Length > (int)FieldLength.PaymentMode)
                errMsg += String.Format("{0}PaymentMode length: {1}. Max is: {2}", Environment.NewLine, rec05.PaymentMode.Length, FieldLength.PaymentMode);
            if (rec05.PaymentSource.Length > (int)FieldLength.PaymentSource)
                errMsg += String.Format("{0}PaymentSource length: {1}. Max is: {2}", Environment.NewLine, rec05.PaymentSource.Length, FieldLength.PaymentSource);
            if (rec05.ReceiptReceivedDate.Length > (int)FieldLength.ReceiptReceivedDate)
                errMsg += String.Format("{0}ReceiptReceivedDate length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptReceivedDate.Length, FieldLength.ReceiptReceivedDate);
            if (rec05.ReceiptEffectiveDate.Length > (int)FieldLength.ReceiptEffectiveDate)
                errMsg += String.Format("{0}ReceiptEffectiveDate length: {1}. Max is: {2}", Environment.NewLine, rec05.ReceiptEffectiveDate.Length, FieldLength.ReceiptEffectiveDate);
            if (rec05.CheckNumber.Length > (int)FieldLength.CheckNumber)
                errMsg += String.Format("{0}CheckNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CheckNumber.Length, FieldLength.CheckNumber);
            if (rec05.ComplianceExemptionReason.Length > (int)FieldLength.ComplianceExemptionReason)
                errMsg += String.Format("{0}ComplianceExemptionReason length: {1}. Max is: {2}", Environment.NewLine, rec05.ComplianceExemptionReason.Length, FieldLength.ComplianceExemptionReason);
            if (rec05.TargetedPaymentIndicator.Length > (int)FieldLength.TargetedPaymentIndicator)
                errMsg += String.Format("{0}TargetedPaymentIndicator length: {1}. Max is: {2}", Environment.NewLine, rec05.TargetedPaymentIndicator.Length, FieldLength.TargetedPaymentIndicator);
            if (rec05.Fips.Length > (int)FieldLength.Fips)
                errMsg += String.Format("{0}Fips length: {1}. Max is: {2}", Environment.NewLine, rec05.Fips.Length, FieldLength.Fips);
            if (rec05.CourtCaseNumber.Length > (int)FieldLength.CourtCaseNumber)
                errMsg += String.Format("{0}CourtCaseNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtCaseNumber.Length, FieldLength.CourtCaseNumber);
            if (rec05.CourtJudgementNumber.ToString().Length > (int)FieldLength.CourtJudgementNumber)
                errMsg += String.Format("{0}CourtJudgementNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtJudgementNumber.Length, FieldLength.CourtJudgementNumber);
            if (rec05.CourtGuidelineNumber.ToString().Length > (int)FieldLength.CourtGuidelineNumber)
                errMsg += String.Format("{0}CourtGuidelineNumber length: {1}. Max is: {2}", Environment.NewLine, rec05.CourtGuidelineNumber.Length, FieldLength.CourtGuidelineNumber);
            if (rec05.ReasonCode.Length > (int)FieldLength.ReasonCode)
                errMsg += String.Format("{0}ReasonCode length: {1}. Max is: {2}", Environment.NewLine, rec05.ReasonCode.Length, FieldLength.ReasonCode);

            
            Log.Exit(errMsg, ExitCode.InvalidRecordLength);
            //Log.Exit(errMsg + Environment.NewLine + rec05.Address1
            //    + Environment.NewLine + rec05.Address2
            //    + Environment.NewLine + rec05.Address3, ExitCode.InvalidRecordLength);
        }


    }
}