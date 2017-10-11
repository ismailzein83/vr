﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingRateGroup
    {
        public string ZoneName { get; set; }

        private ExistingRatesByOwner _normalRates = new ExistingRatesByOwner();

        public ExistingRatesByOwner NormalRates
        {
            get
            {
                return this._normalRates;
            }
        }
    }

    public class ExistingZoneRoutingProductGroup
    {
        public string ZoneName { get; set; }

        private ExistingZonesRoutingProductsByOwner _zoneRoutingProducts = new ExistingZonesRoutingProductsByOwner();

        public ExistingZonesRoutingProductsByOwner ZoneRoutingProducts
        {
            get
            {
                return this._zoneRoutingProducts;
            }
        }
    }
}
