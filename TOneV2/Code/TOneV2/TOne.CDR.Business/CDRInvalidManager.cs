using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class CDRInvalidManager
    {
        public void DeleteCDRInvalid(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            dataManager.DeleteCDRInvalid(from, to, customerIds, supplierIds);
        }
       public void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            dataManager.ReserverIdRange(isMain, isNegative, numberOfIds,out id);
        }
       public void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs)
       {
           ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
           dataManager.ApplyInvalidCDRsToDB(preparedInvalidCDRs);

        }
       public void SaveInvalidCDRsToDB(List<BillingCDRInvalid> cdrsInvalid)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            dataManager.SaveInvalidCDRsToDB(cdrsInvalid);
        }
    }
}
