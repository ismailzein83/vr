using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using Vanrise.Data;

namespace TOne.CDR.Data
{
    public interface ICDRInvalidDataManager : IDataManager, IBulkApplyDataManager<BillingCDRInvalid>
    {
        void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id);
        void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs);
        void SaveInvalidCDRsToDB(List<BillingCDRInvalid> cdrsInvalid);
    }
}
