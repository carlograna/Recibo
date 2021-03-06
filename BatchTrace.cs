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
    
    public partial class BatchTrace
    {
        public int GlobalBatchID { get; set; }
        public Nullable<System.DateTime> TypeOneCutoff { get; set; }
        public Nullable<System.DateTime> IBMLFEI { get; set; }
        public string TransportType { get; set; }
        public string TransportName { get; set; }
        public Nullable<System.Guid> CheckFileID { get; set; }
        public string TPRBatchName { get; set; }
        public Nullable<System.DateTime> OpexImport { get; set; }
        public Nullable<System.DateTime> ImageExchangeExtract { get; set; }
        public Nullable<System.DateTime> ArchiveExpressExtrStatus { get; set; }
        public Nullable<System.DateTime> WebDDLImporter { get; set; }
        public Nullable<System.DateTime> OpexBackendImport { get; set; }
        public Nullable<System.DateTime> ImageRPSImporter { get; set; }
        public Nullable<System.DateTime> RMSExporter { get; set; }
        public Nullable<System.DateTime> CWDBBatchStatus { get; set; }
        public Nullable<System.DateTime> CWDBImageStatus { get; set; }
        public Nullable<byte> CWDBConsolidationPriority { get; set; }
        public Nullable<byte> CWDBErrorCountAttempts { get; set; }
        public Nullable<System.DateTime> CeresoftExporter { get; set; }
        public Nullable<System.DateTime> ACHImporter { get; set; }
        public string RTNumber { get; set; }
    }
}
