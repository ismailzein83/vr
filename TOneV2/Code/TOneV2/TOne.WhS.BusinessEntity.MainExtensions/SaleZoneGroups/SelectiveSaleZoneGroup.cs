﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class SelectiveSaleZoneGroup : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            return this.ZoneIds;
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {
            var validZoneIds = context.GetGroupZoneIds(this);
            if (validZoneIds != null)
            {
                SaleZoneManager manager = new SaleZoneManager();
                return manager.GetDescription(base.SellingNumberPlanId, validZoneIds);
            }
            else
                return null;
        }
    }
}
