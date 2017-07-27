using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class PaymentConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_PaymentConfig";
        public string Editor { get; set; }
    }
}
