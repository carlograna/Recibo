using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReceiptExport.classes
{
    class Receipt
    {
        public int GlobalBatchID { get; set; }
        public int TransactionID { get; set; }
        public int GlobalStubID { get; set; }
        public string ProcessingDate { get; set; }
        public string PersonID { get; set; }
        public string CaseNumber { get; set; }
        public string CourtGuideline { get; set; }
        public string CourtJudgment { get; set; }
        public string EffectiveDate { get; set; }
        public string FIPS { get; set; }
        public string OFCAmount { get; set; }
        public string PaidBy { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentSource { get; set; }
        public string ReasonCode { get; set; }
        public string TargetedPayment { get; set; }
        public string ExportedToCHARTSDate { get; set; }
        public string CHARTSStubPrefix { get; set; }
        public string SDUTranID { get; set; }
        public byte ExportedAsUnidentified { get; set; }
        public string Amount { get; set; }
        public string RTNumber { get; set; }
        public string Serial { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string SSN { get; set; }
    }
}
