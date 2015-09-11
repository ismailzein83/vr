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
        void ApplyMainCDRsToDB(Object preparedMainCDRs);
        void SaveMainCDRsToDB(List<BillingCDRMain> cdrsMain);
        void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id);
    }
}
