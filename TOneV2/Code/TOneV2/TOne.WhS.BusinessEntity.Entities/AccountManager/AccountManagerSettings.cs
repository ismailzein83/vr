using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AccountManagerSettings : SettingData
    {
        public CustomerFiltering CustomerFiltering { get; set; }
    }
    public class CustomerFiltering
    {
        public bool RatePlan { get; set; }
    }
}
