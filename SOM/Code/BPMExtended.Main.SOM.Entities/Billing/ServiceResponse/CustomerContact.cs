using BPMExtended.Main.Entities;
using System;

namespace BPMExtended.Main.SOMAPI
{
    public class CustomerContract
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RateplanId { get; set; }
        public int Status { get; set; }
        public string LinePathId { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public Address ContractAddress { get; set; }
        public string ContractStatusId { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime LastStatusChangeDate { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class TelephonyContract
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public long RateplanId { get; set; }
        public string RateplanName { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
    }

    public class TelephonyContractData
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public long RateplanId { get; set; }
        public string RateplanName { get; set; }
        public string PhoneNumber { get; set; }
        public string LinePathId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public Decimal? Balance { get; set; }
        public int Status { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
    }
}
