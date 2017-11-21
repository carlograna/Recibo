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
        private decimal amount;
        private decimal ofcAmount;
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
            get { return sduBatchId.ToSafeString(); }
            set { sduBatchId = value; }
        }
        public string SduTranId
        {
            get { return sduTranId.ToSafeString(); }
            set { sduTranId = value; }
        }
        public string ReceiptNumber
        {
            get { return receiptNumber.ToSafeString(); }
            set { receiptNumber = value; }
        }
        public bool RetransmittalIndicator
        {
            get { return retransmittalIndicator; }
            set { retransmittalIndicator = value; }
        }
        public string PayorID
        {
            get { return payorID.ToSafeString(); }
            set { payorID = value; }
        }
        public string PayorSSN
        {
            get { return payorSSN.ToSafeString(); }
            set { payorSSN = value; }
        }
        public string PaidBy
        {
            get { return paidBy.ToSafeString(); }
            set { paidBy = value; }
        }
        public string PayorLastName
        {
            get { return payorLastName.ToSafeString(); }
            set { payorLastName = value; }
        }
        public string PayorFirstName
        {
            get { return payorFirstName.ToSafeString(); }
            set { payorFirstName = value; }
        }
        public string PayorMiddleName
        {
            get { return payorMiddleName.ToSafeString(); }
            set { payorMiddleName = value; }
        }
        public string PayorSuffix
        {
            get { return payorSuffix.ToSafeString(); }
            set { payorSuffix = value; }
        }
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public decimal OfcAmount
        {
            get { return ofcAmount; }
            set { ofcAmount = value; }
        }
        public string PaymentMode
        {
            get { return paymentMode.ToSafeString(); }
            set { paymentMode = value; }
        }
        public string PaymentSource
        {
            get { return paymentSource.ToSafeString(); }
            set { paymentSource = value; }
        }
        public string ReceiptReceivedDate
        {
            get {
                DateTime date;
                string yyyyMMdd="";
                if (DateTime.TryParse(receiptReceivedDate, out date))
                {
                    yyyyMMdd = date.ToString("yyyyMMdd");
                }
                else Log.WriteLine("Invalid ReceiptReceivedDate " + receiptReceivedDate.ToSafeString() + ", Stub: " + sduTranId);

                return yyyyMMdd;
            }
            set { receiptReceivedDate = value; }
        }
        public string ReceiptEffectiveDate
        {
            get {
                DateTime date;
                string yyyyMMdd="";
                if (DateTime.TryParse(receiptEffectiveDate, out date))
                {
                    yyyyMMdd = date.ToString("yyyyMMdd");
                }
                else
                {
                    Log.WriteLine("Invalid receiptEffectiveDate:" + receiptEffectiveDate.ToSafeString() + ", Stub: " + sduTranId);
                }

                return yyyyMMdd;
            }
            set { receiptEffectiveDate = value; }
        }
        public string CheckNumber
        {
            get { return checkNumber.ToSafeString(); }
            set { checkNumber = value; }
        }
        public string ComplianceExemptionReason
        {
            get { return complianceExemptionReason.ToSafeString(); }
            set { complianceExemptionReason = value; }
        }
        public string TargetedPaymentIndicator
        {
            get { return targetedPaymentIndicator.ToSafeString(); }
            set { targetedPaymentIndicator = value; }
        }
        public string Fips
        {
            get { return fips.ToSafeString(); }
            set { fips = value; }
        }
        public string CourtCaseNumber
        {
            get { return courtCaseNumber.ToSafeString(); }
            set { courtCaseNumber = value; }
        }
        public string CourtJudgementNumber
        {
            get { return courtJudgementNumber.ToSafeString(); }
            set { courtJudgementNumber = value; }
        }
        public string CourtGuidelineNumber
        {
            get { return courtGuidelineNumber.ToSafeString(); }
            set { courtGuidelineNumber = value; }
        }
        public string ReasonCode
        {
            get { return reasonCode.ToSafeString(); }
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
                string strOfcAmount = ((int)(OfcAmount * 100)).ToString();
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

        public string RecordLine()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(recordType); // (2) - static length
            sb.Append(SduBatchId.PadLeft(20, '0'));
            sb.Append(SduTranId); //(20) - static length
            sb.Append(ReceiptNumber.PadLeft(7, '0'));
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
            sb.Append(ReceiptReceivedDate.PadRight(8));
            sb.Append(ReceiptEffectiveDate.PadRight(8));
            sb.Append(CheckNumber.PadRight(18));
            sb.Append(ComplianceExemptionReason.PadRight(1));
            sb.Append(TargetedPaymentIndicator.PadRight(1, '0'));
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