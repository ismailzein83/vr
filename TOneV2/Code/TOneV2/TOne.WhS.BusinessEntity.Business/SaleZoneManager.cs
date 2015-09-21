using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneManager
    {
        public List<SaleZone> GetSaleZones(int packageId)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            return dataManager.GetSaleZones(packageId);
        }

        public Dictionary<string, List<SaleCode>> GetSaleZonesWithCodes(int packageId)
        {
            Dictionary<string, List<SaleCode>> saleZoneDictionary = new Dictionary<string, List<SaleCode>>();
            List<SaleZone> salezones = GetSaleZones(packageId);
            if (salezones != null && salezones.Count>0)
            {
                SaleCodeManager manager = new SaleCodeManager();
                foreach (SaleZone saleZone in salezones)
                {

                    List<SaleCode> saleCodes = manager.GetSaleCodesByZoneID(saleZone.SaleZoneId);
                    List<SaleCode> saleCodesOut;
                    if (!saleZoneDictionary.TryGetValue(saleZone.Name, out saleCodesOut))
                        saleZoneDictionary.Add(saleZone.Name, saleCodes);
                }
            }
           

            return saleZoneDictionary;
        }
        public void InsertSaleZones(List<SaleZone> saleZones)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
            foreach (SaleZone saleZone in saleZones)
               dataManager.WriteRecordToStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones = dataManager.FinishDBApplyStream(dbApplyStream);
             dataManager.ApplySaleZonesForDB(prepareToApplySaleZones);
        }
        public void DeleteSaleZones(List<SaleZone> saleZones)
        {
            ISaleZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
            dataManager.DeleteSaleZones(saleZones);
        }
    }
}
