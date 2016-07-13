using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteConfigurationSettingData : SettingData
    {
        public int CustomerTransformationId { get; set; }
        public int SupplierTransformationId { get; set; }
    }
}
