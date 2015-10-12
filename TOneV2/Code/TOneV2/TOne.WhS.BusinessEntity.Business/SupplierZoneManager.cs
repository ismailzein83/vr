﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager
    {
       
        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.ReserveIDRange(numberOfIDs);
        }

        public List<SupplierWithZones> GetSuppliersWithZoneIds(SuppliersWithZonesGroupSettings suppliersWithZonesGroupSettings)
        {
            TemplateConfigManager templateConfigManager = new TemplateConfigManager();
            SuppliersWithZonesGroupBehavior suppliersWithZonesGroupBehavior = templateConfigManager.GetBehavior<SuppliersWithZonesGroupBehavior>(suppliersWithZonesGroupSettings.ConfigId);
            if (suppliersWithZonesGroupBehavior != null)
                return suppliersWithZonesGroupBehavior.GetSuppliersWithZones(suppliersWithZonesGroupSettings);
            else
                return null;
        }

    }
}
