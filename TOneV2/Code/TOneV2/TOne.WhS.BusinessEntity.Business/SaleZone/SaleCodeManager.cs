using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeManager
    {
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID,DateTime effectiveDate)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID, effectiveDate);
        }
        public void InsertSaleCodes(List<SaleCode> saleCodes)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
            foreach (SaleCode saleCode in saleCodes)
                dataManager.WriteRecordToStream(saleCode, dbApplyStream);
            object prepareToApplySaleCodes = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplySaleCodesForDB(prepareToApplySaleCodes);
        }
        public void DeleteSaleCodes(List<SaleCode> saleCodes)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            dataManager.DeleteSaleCodes(saleCodes);
        }

        public List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
    }
}
