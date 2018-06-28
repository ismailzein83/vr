using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.ProxyAPI
{
    public enum FailureReason { InvalidPin = 0}
    public class AddTopupOutput
    {
        public bool IsSucceeded { get; set; }
        public FailureReason? FailureReason { get; set; }
    }
    public class AddTopupInput
    {
        public string PhoneNumber { get; set; }
        public string PinCode { get; set; }
    }
}
