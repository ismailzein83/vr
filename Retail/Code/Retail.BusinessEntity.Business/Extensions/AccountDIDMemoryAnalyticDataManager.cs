using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountDIDMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> records = new List<RawMemoryRecord>();
            var didManager = new DIDManager();
            var dids = didManager.GetCachedDIDs();
            var didAccountsByDIDId = didManager.GetDIDAccountsByDIDId();
            if (dids != null)
            {
                foreach (var did in dids.Values)
                {
                    bool anyRecordAdded = false;
                    List<DIDAccount> matchDIDAccounts;
                    if (didAccountsByDIDId != null && didAccountsByDIDId.TryGetValue(did.DIDId, out matchDIDAccounts))
                    {
                        foreach (var didAccount in matchDIDAccounts)
                        {
                            if (Utilities.AreTimePeriodsOverlapped(didAccount.BED, didAccount.EED, query.FromTime, query.ToTime))
                            {
                                RawMemoryRecord record = CreateRecordFromDID(did);
                                record.FieldValues.Add("AccountId", didAccount.Account.AccountId);
                                record.FieldValues.Add("AccountStatusId", didAccount.Account.StatusId);
                                record.FieldValues.Add("BED", didAccount.BED);
                                record.FieldValues.Add("EED", didAccount.EED);
                                records.Add(record);
                                anyRecordAdded = true;
                            }
                        }
                    }
                    if (!anyRecordAdded)
                    {
                        RawMemoryRecord record = CreateRecordFromDID(did);
                        record.FieldValues.Add("AccountId", null);
                        record.FieldValues.Add("AccountStatusId", null);
                        record.FieldValues.Add("BED", null);
                        record.FieldValues.Add("EED", null);
                        records.Add(record);
                    }
                }
            }

            return records;
        }

        private static RawMemoryRecord CreateRecordFromDID(DID did)
        {
            RawMemoryRecord record = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            record.FieldValues.Add("DIDId", did.DIDId);
            return record;
        }
    }
}
