using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using Vanrise.Data;

namespace TOne.CDR.Data
{
    public interface ICDRMainDataManager : IDataManager, IBulkApplyDataManager<BillingCDRMain>
    {
        void DeleteCDRMain(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds);
        void DeleteCDRCost(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds);
        void DeleteCDRSale(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds);
        void ApplyMainCDRsToDB(Object preparedMainCDRs);
        void SaveMainCDRsToDB(List<BillingCDRMain> cdrsMain);
        void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id);
    }
}
