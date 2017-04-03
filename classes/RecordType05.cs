using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReceiptExport
{
    enum FieldLength : int
    {
        RecordType = 2,
        SduBatchId = 20,
        SduTranId = 20,
        ReceiptNumber = 7,
        RetransmittalIndicator = 1,
        PayorID = 13,
        PayorSSN = 9,
        PaidBy = 15,
        PayorLastName = 25,
        PayorFirstName = 20,
        PayorMiddleName = 20,
        PayorSuffix = 3,
        Amount = 15,
        OfcAmount = 15,
        PaymentMode = 2,
        PaymentSource = 4,
        ReceiptReceivedDate = 8,
        ReceiptEffectiveDate = 8,
        CheckNumber = 18,
        ComplianceExemptionReason = 1,
        TargetedPaymentIndicator = 1,
        Fips = 7,
        CourtCaseNumber = 25,
        CourtJudgementNumber = 3,
        CourtGuidelineNumber = 3,
        ReasonCode = 3,
        Filler = 32
    }

    class RecordType05
    {
        public RecordType05() { }

        private string recordType = "05";
        private string sduBatchId;
        private string sduTranId;
        private string receiptNumber;
        private bool retransmittalIndicator;
        private string payorID;
        private string payorSSN;
        private string paidBy;
        private string payorLastName;
        private string payorFirstName;
        private string payorMiddleName;
        private string payorSuffix;
        private double amount;
        private double ofcAmount;
        private string paymentMode;
        private string paymentSource;
        private string receiptReceivedDate;
        private string receiptEffectiveDate;
        private string checkNumber;
        private string complianceExemptionReason;
        private string targetedPaymentIndicator;
        private string fips;
        private string courtCaseNumber;
        private string courtJudgementNumber;
        private string courtGuidelineNumber;
        private string reasonCode;
        private string filler = "";

        public string SduBatchId
        {
            get{ return sduBatchId; }
            set{ sduBatchId = value; }
        }
        public string SduTranId
        {
            get { return sduTranId; }
            set { sduTranId = value; }
        }
        public string ReceiptNumber
        {
            get { return receiptNumber; }
            set { receiptNumber = value; }
        }
        public bool RetransmittalIndicator
        {
            get { return retransmittalIndicator; }
            set { retransmittalIndicator = value; }
        }
        public string PayorID
        {
            get { return payorID; }
            set { payorID = value; }
        }
        public string PayorSSN
        {
            get { return payorSSN; }
            set { payorSSN = value; }
        }
        public string PaidBy
        {
            get { return paidBy; }
            set { paidBy = value; }
        }
        public string PayorLastName
        {
            get { return payorLastName; }
            set { payorLastName = value; }
        }
        public string PayorFirstName
        {
            get { return payorFirstName; }
            set { payorFirstName = value; }
        }
        public string PayorMiddleName
        {
            get { return payorMiddleName; }
            set { payorMiddleName = value; }
        }
        public string PayorSuffix
        {
            get { return payorSuffix; }
            set { payorSuffix = value; }
        }
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public double OfcAmount
        {
            get { return ofcAmount; }
            set { ofcAmount = value; }
        }
        public string PaymentMode
        {
            get { return paymentMode; }
            set { paymentMode = value; }
        }
        public string PaymentSource
        {
            get { return paymentSource; }
            set { paymentSource = value; }
        }
        public string ReceiptReceivedDate
        {
            get {
                DateTime date;
                if (DateTime.TryParse(receiptReceivedDate, out date))
                {
                    receiptReceivedDate = date.ToString("yyyyMMdd");
                }
                else Log.WriteLine("Invalid ReceiptReceivedDate" + receiptReceivedDate);

                return receiptReceivedDate;
            }
            set { receiptReceivedDate = value; }
        }
        public string ReceiptEffectiveDate
        {
            get {
                DateTime date;
                if (DateTime.TryParse(receiptEffectiveDate, out date))
                {
                    receiptEffectiveDate = date.ToString("yyyyMMdd");
                }
                else
                {
                    Log.Exit("Invalid receiptEffectiveDate" + receiptEffectiveDate, ExitCode.InvalidData);
                }
                
                return receiptEffectiveDate;
            }
            set { receiptEffectiveDate = value; }
        }
        public string CheckNumber
        {
            get { return checkNumber; }
            set { checkNumber = value; }
        }
        public string ComplianceExemptionReason
        {
            get { return complianceExemptionReason; }
            set { complianceExemptionReason = value; }
        }
        public string TargetedPaymentIndicator
        {
            get { return targetedPaymentIndicator; }
            set { targetedPaymentIndicator = value; }
        }
        public string Fips
        {
            get { return fips; }
            set { fips = value; }
        }
        public string CourtCaseNumber
        {
            get { return courtCaseNumber; }
            set { courtCaseNumber = value; }
        }
        public string CourtJudgementNumber
        {
            get { return courtJudgementNumber; }
            set { courtJudgementNumber = value; }
        }
        public string CourtGuidelineNumber
        {
            get { return courtGuidelineNumber; }
            set { courtGuidelineNumber = value; }
        }
        public string ReasonCode
        {
            get { return reasonCode; }
            set { reasonCode = value; }
        }

        // Formatted Accessors
        public string StrAmount
        {
            get
            {
                //string strAmount = Amount.ToString().Replace(".", ""); 
                string strAmount = ((int)(Amount * 100)).ToString(); 
                return strAmount;
            }
        }
        public string StrOfcAmount
        {
            get
            {
                string strOfcAmount = OfcAmount.ToString().Replace(".", "");
                return strOfcAmount;
            }
        }

        public string StrRetransmittalIndicator
        {
            get
            {
                string strRetransmittalIndicator = RetransmittalIndicator ? "1" : "0";
                return strRetransmittalIndicator;
            }
        }

        
  

        // continue adding public accessors for the private fields.

        public string RecordLine()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append(recordType.PadRight(2));
            sb.Append(recordType); // (2) - static length
            sb.Append(SduBatchId.PadLeft(20, '0'));
            //sb.Append(SduTranId.PadRight(20));
            sb.Append(SduTranId); //(20) - static length
            sb.Append(ReceiptNumber.ToString().PadLeft(7, '0'));
            sb.Append(StrRetransmittalIndicator.PadRight(1));
            sb.Append(PayorID.PadRight(13));
            sb.Append(PayorSSN.PadRight(9));
            sb.Append(PaidBy.PadRight(15));
            sb.Append(PayorLastName.PadRight(25));
            sb.Append(PayorFirstName.PadRight(20));
            sb.Append(PayorMiddleName.PadRight(20));
            sb.Append(PayorSuffix.PadRight(3));
            sb.Append(StrAmount.PadLeft(15, '0'));
            sb.Append(StrOfcAmount.PadLeft(15, '0'));
            sb.Append(PaymentMode.PadRight(2));
            sb.Append(PaymentSource.PadRight(4));
            sb.Append(ReceiptReceivedDate.ToString().PadRight(8));
            sb.Append(ReceiptEffectiveDate.ToString().PadRight(8));
            sb.Append(CheckNumber.PadRight(18));
            sb.Append(ComplianceExemptionReason.PadRight(1));
            sb.Append(TargetedPaymentIndicator.ToString().PadRight(1));
            sb.Append(Fips.PadRight(7));
            sb.Append(CourtCaseNumber.PadRight(25));
            sb.Append(CourtJudgementNumber.PadLeft(3, '0'));
            sb.Append(CourtGuidelineNumber.PadLeft(3, '0'));
            sb.Append(ReasonCode.PadRight(3));
            sb.Append(filler.PadRight(32));

            return sb.ToString();
        }
                                                                   
    }
}

