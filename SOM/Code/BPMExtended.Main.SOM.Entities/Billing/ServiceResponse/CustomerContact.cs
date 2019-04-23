using System;

namespace BPMExtended.Main.SOMAPI
{
    public class CustomerContract
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RateplanId { get; set; }
        public int Status { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ContractStatusId { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime LastStatusChangeDate { get; set; }
    }
}
