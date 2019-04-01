using System;

namespace BPMExtended.Main.Entities
{
    public class CreateCustomerRequestOutput
    {
        public int CustomerId { get; set; }
        public string   PublicCustomerId { get; set; }
        public int AddressId { get; set; }
    }
}
