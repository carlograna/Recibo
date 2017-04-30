//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReceiptExport
{
    using System;
    using System.Collections.Generic;
    
    public partial class StubsDataEntry
    {
        public int GlobalBatchID { get; set; }
        public int TransactionID { get; set; }
        public int GlobalStubID { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<decimal> ACHAddendaBPRMonetaryAmount { get; set; }
        public string ACHAddendaBPRAccountNumber { get; set; }
        public string ACHAddendaRMRReferenceNumber { get; set; }
        public Nullable<decimal> ACHAddendaRMRMonetaryAmount { get; set; }
        public Nullable<decimal> ACHAddendaRMRTotalInvAmount { get; set; }
        public Nullable<decimal> ACHAddendaRMRDiscountAmount { get; set; }
        public string ARP { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentSource { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string PaidBy { get; set; }
        public Nullable<decimal> OFCAmount { get; set; }
        public string FIPS { get; set; }
        public string TargetedPayment { get; set; }
        public string CaseNumber { get; set; }
        public string CourtJudgment { get; set; }
        public string CourtGuideline { get; set; }
        public string ReasonCode { get; set; }
        public byte ExportedToCHARTS { get; set; }
        public Nullable<System.DateTime> ExportedToCHARTSDate { get; set; }
        public Nullable<byte> ExportedAsUnidentified { get; set; }
        public Nullable<System.DateTime> ResolvedDate { get; set; }
        public string CHARTSStubPrefix { get; set; }
        public Nullable<byte> Archived { get; set; }
        public Nullable<System.DateTime> ArchivedStamp { get; set; }
        public string ArchivedBy { get; set; }
        public Nullable<decimal> TotalAmountDue { get; set; }
        public Nullable<byte> Adjusted { get; set; }
        public Nullable<System.DateTime> AdjustedDate { get; set; }
        public string AdjustedBy { get; set; }
        public Nullable<int> AdjustmentReason { get; set; }
        public Nullable<System.DateTime> CHARTSAdjustedDate { get; set; }
        public string SSN { get; set; }
        public Nullable<int> ErrorCode { get; set; }
        public Nullable<System.DateTime> ErrorDate { get; set; }
        public Nullable<byte> RAHoldStatus { get; set; }
        public Nullable<int> RAHoldReason { get; set; }
        public Nullable<System.DateTime> RAHoldDate { get; set; }
        public Nullable<System.DateTime> RAHoldReleaseDate { get; set; }
        public string RAHoldBy { get; set; }
        public string RAReleaseBy { get; set; }
        public string ScanLine { get; set; }
        public string SDUTranID { get; set; }
        public string AddendaData { get; set; }
        public Nullable<byte> ComplianceExemptReason { get; set; }
    }
}
