using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanZoneManager
    {
        public IEnumerable<SaleZone> GetRatePlanZones(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, DateTime effectiveOn, char zoneLetter, int fromRow, int toRow)
        {
            IEnumerable<SaleZone> zones = GetZones(ownerType, ownerId, sellingNumberPlanId, effectiveOn);
            zones = GetFilteredZones(zones, zoneLetter);
            return GetPagedZones(zones, fromRow, toRow);
        }

        // GetZones is public because it's invoked by ApplyCalculatedRates of RatePlanManager
        public IEnumerable<SaleZone> GetZones(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, DateTime effectiveOn)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                if (sellingNumberPlanId != null)
                {
                    SaleZoneManager saleZoneManager = new SaleZoneManager();
                    return saleZoneManager.GetSaleZones((int)sellingNumberPlanId, effectiveOn);
                }
            }
            else
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                return manager.GetCustomerSaleZones(ownerId, effectiveOn, false);
            }

            return null;
        }

        IEnumerable<SaleZone> GetFilteredZones(IEnumerable<SaleZone> zones, char zoneLetter)
        {
            return zones.FindAllRecords(itm => itm.Name != null && itm.Name.Length > 0 && char.ToLower(itm.Name.ElementAt(0)) == char.ToLower(zoneLetter));
        }

        IEnumerable<SaleZone> GetPagedZones(IEnumerable<SaleZone> zones, int fromRow, int toRow)
        {
            if (zones == null)
                return zones;

            List<SaleZone> pagedZones = null;
            int count = zones.Count();

            if (count >= fromRow)
            {
                pagedZones = new List<SaleZone>();

                for (int i = fromRow - 1; i < count && i < toRow; i++)
                    pagedZones.Add(zones.ElementAt(i));
            }

            return pagedZones;
        }
    }
}
