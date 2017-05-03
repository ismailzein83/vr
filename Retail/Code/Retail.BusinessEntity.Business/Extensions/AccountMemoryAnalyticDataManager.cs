using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public Guid AccountBEDefinitionId { get; set; }

        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Entities.DataRetrievalInput<Vanrise.Analytic.Entities.AnalyticQuery> input, List<string> neededFieldNames)
        {
            neededFieldNames.ThrowIfNull("neededFieldNames");
            AccountBEManager accountManager = new AccountBEManager();
            var cachedAccounts = accountManager.GetCachedAccounts(this.AccountBEDefinitionId);
            cachedAccounts.ThrowIfNull("cachedAccounts", this.AccountBEDefinitionId);
            List<RawMemoryRecord> records = new List<RawMemoryRecord>();
            foreach(var account in cachedAccounts.Values)
            {
                RawMemoryRecord record = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
                foreach(var fieldName in neededFieldNames)
                {
                    record.FieldValues.Add(fieldName, accountManager.GetAccountGenericFieldValue(this.AccountBEDefinitionId, account, fieldName));
                }
                records.Add(record);
            }
            return records;
        }
    }
}
