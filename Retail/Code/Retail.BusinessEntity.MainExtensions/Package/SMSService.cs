using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public class SMSService : PackageService
    {
        public int NbofCharPerMessage { get; set; }
        public bool Unicode { get; set; }
        public bool MMSSupport { get; set; }
    }
}
