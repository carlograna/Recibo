﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ReceiptDBContext : DbContext
    {
        public ReceiptDBContext()
            : base("name=ReceiptDBContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<BatchTrace> BatchTraces { get; set; }
        public virtual DbSet<Check> Checks { get; set; }
        public virtual DbSet<ChecksDataEntry> ChecksDataEntries { get; set; }
        public virtual DbSet<KCPerson> KCPersons { get; set; }
        public virtual DbSet<Stub> Stubs { get; set; }
        public virtual DbSet<StubsDataEntry> StubsDataEntries { get; set; }
    
        public virtual int ReceiptExport()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ReceiptExport");
        }
    }
}