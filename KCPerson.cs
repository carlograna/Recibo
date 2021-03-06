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
    
    public partial class KCPerson
    {
        public System.Guid KCPersonID { get; set; }
        public string SSN { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string WorkPhone { get; set; }
        public string WorkExtension { get; set; }
        public string eMail { get; set; }
        public string Country { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> DateOfDeath { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> ModifiedStamp { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<int> PINNumber { get; set; }
        public string PersonID { get; set; }
        public string Pager { get; set; }
        public string AlternateEmail { get; set; }
        public string BillingEmail { get; set; }
        public byte KCDeliveryMethodID { get; set; }
        public bool FVI { get; set; }
        public Nullable<byte> IsActive { get; set; }
        public Nullable<System.DateTime> InactiveDate { get; set; }
        public bool InvalidAddress { get; set; }
        public bool DenyWebAccess { get; set; }
        public Nullable<byte> ChangeReasonCode { get; set; }
        public bool PaymentNotification { get; set; }
        public Nullable<System.DateTime> CreationStamp { get; set; }
        public Nullable<byte> KCPaymentMethodID { get; set; }
        public Nullable<System.DateTime> AddressStamp { get; set; }
        public Nullable<System.DateTime> PhoneStamp { get; set; }
        public Nullable<System.DateTime> EmailStamp { get; set; }
        public Nullable<System.DateTime> BillingEmailStamp { get; set; }
        public Nullable<System.DateTime> DeliveryMethodStamp { get; set; }
        public Nullable<System.DateTime> DODStamp { get; set; }
        public Nullable<System.DateTime> PaymentMethodStamp { get; set; }
        public Nullable<byte> SignMeUpForEbs { get; set; }
        public Nullable<bool> IVDStatus { get; set; }
    }
}
