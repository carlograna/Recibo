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
    
    public partial class Stub
    {
        public int GlobalBatchID { get; set; }
        public int TransactionID { get; set; }
        public int GlobalStubID { get; set; }
        public Nullable<int> TransactionSequence { get; set; }
        public Nullable<int> BatchSequence { get; set; }
        public Nullable<int> StubSequence { get; set; }
        public Nullable<int> SplitNumber { get; set; }
        public Nullable<byte> Status { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> OCRAmount { get; set; }
        public string RawOCR1 { get; set; }
        public string RawOCR2 { get; set; }
        public string RawOCR3 { get; set; }
        public Nullable<int> DEBillingKeys { get; set; }
        public Nullable<int> DEDataKeys { get; set; }
        public Nullable<int> ItemDataKeys { get; set; }
        public Nullable<int> ItemRawKeys { get; set; }
        public Nullable<int> ItemBillingKeys { get; set; }
        public Nullable<bool> IsGhost { get; set; }
        public Nullable<bool> IsOMRDetected { get; set; }
        public Nullable<int> Tray { get; set; }
        public string ICRTranKey { get; set; }
        public Nullable<bool> IsOMRZone1Detected { get; set; }
        public Nullable<bool> IsOMRZone2Detected { get; set; }
        public Nullable<bool> IsOMRZone3Detected { get; set; }
        public Nullable<bool> IsOMRZone4Detected { get; set; }
        public int PageNo { get; set; }
        public bool IsSingleOccurrence { get; set; }
        public Nullable<int> OriginalGlobalStubID { get; set; }
    }
}
