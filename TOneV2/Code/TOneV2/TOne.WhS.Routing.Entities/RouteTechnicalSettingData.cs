using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteTechnicalSettingData : SettingData
    {
        public RouteRuleDataTransformation RouteRuleDataTransformation { get; set; }
    }

    public class RouteRuleDataTransformation
        {
        public int CustomerTransformationId { get; set; }
        public int SupplierTransformationId { get; set; }
    }
}
