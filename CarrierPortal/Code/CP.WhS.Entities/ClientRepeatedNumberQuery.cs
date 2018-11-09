using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.WhS.Entities
{
    public enum ClientCDRType { All = 0, Successful = 1, Failed = 2, Invalid = 3, PartialPriced = 4 }
    public class ClientRepeatedNumberQuery
    {
        public ClientRepeatedNumberFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int RepeatedMorethan { get; set; }
        public ClientCDRType CDRType { get; set; }
        public string PhoneNumberType { get; set; }
        public string PhoneNumber { get; set; }
    }
}
