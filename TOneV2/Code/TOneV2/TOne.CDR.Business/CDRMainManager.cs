using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class CDRMainManager
    {
        public void DeleteCDRSale(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.DeleteCDRSale(from, to, customerIds, supplierIds);
        }

        public void DeleteCDRCost(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.DeleteCDRCost(from, to, customerIds, supplierIds);
        }
        public void DeleteCDRMain(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.DeleteCDRMain(from, to, customerIds, supplierIds);
        }

        public void ApplyMainCDRsToDB(Object preparedMainCDRs)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.ApplyMainCDRsToDB(preparedMainCDRs);
        }
        public void SaveMainCDRsToDB(List<BillingCDRMain> cdrsMain)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.SaveMainCDRsToDB(cdrsMain);
        }
        public void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            dataManager.ReserverIdRange(isMain, isNegative, numberOfIds,out id);
        }
    }
}
