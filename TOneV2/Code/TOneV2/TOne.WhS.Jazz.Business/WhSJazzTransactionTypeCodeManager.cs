using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Business
{
    public class WhSJazzTransactionTypeCodeManager
    {
        public static Guid _definitionId = new Guid("476CC49C-7FAB-482A-B5F3-F91772AF9EDF");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzTransactionTypeCode> GetAllTransactionTypeCodes()
        {
            var records = GetCachedTransactionTypeCodes();
            List<WhSJazzTransactionTypeCode> transactionTypeCodes = null;

            if (records != null && records.Count > 0)
            {
                transactionTypeCodes = new List<WhSJazzTransactionTypeCode>();
                foreach (var record in records)
                {
                    transactionTypeCodes.Add(record.Value);
                }
            }
            return transactionTypeCodes;
        }


        public IEnumerable<WhSJazzTransactionTypeCodeDetail> GetTransactionTypeCodesInfo(WhSJazzTransactionTypeCodeInfoFilter filter)
        {
            var transactionTypeCodes = GetCachedTransactionTypeCodes();
            Func<WhSJazzTransactionTypeCode, bool> filterFunc = (transactionTypeCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzTransactionTypeCodeFilterContext
                        {
                            TransactionTypeCode = transactionTypeCode
                        };
                        foreach (var transactionTypeCodeFilter in filter.Filters)
                        {
                            if (!transactionTypeCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return transactionTypeCodes.MapRecords((record) =>
            {
                return TransactionTypeCodeInfoMapper(record);
            }, filterFunc);

        }
        private Dictionary<Guid, WhSJazzTransactionTypeCode> GetCachedTransactionTypeCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTransactionTypeCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzTransactionTypeCode> result = new Dictionary<Guid, WhSJazzTransactionTypeCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzTransactionTypeCode transactionType = new WhSJazzTransactionTypeCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(transactionType.ID, transactionType);
                    }
                }

                return result;
            });
        }

        private WhSJazzTransactionTypeCodeDetail TransactionTypeCodeInfoMapper(WhSJazzTransactionTypeCode whSJazzTransactionTypeCode)
        {
            return new WhSJazzTransactionTypeCodeDetail
            {
                ID = whSJazzTransactionTypeCode.ID,
                Name = whSJazzTransactionTypeCode.Name
            };
        }

    }
  
}
