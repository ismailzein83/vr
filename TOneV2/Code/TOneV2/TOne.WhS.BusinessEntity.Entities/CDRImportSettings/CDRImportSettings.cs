using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CDRImportSettings : SettingData
    {
        public int CustomerCDPNId { get; set; }

        public int SupplierCDPNId { get; set; }        
        
        public int CDPNId { get; set; }
        
        public int SaleZoneCDPNId { get; set; }

        public int SupplierZoneCDPNId { get; set; }
    }
}
