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
    public class TransactionTypeManager
    {
        public static Guid _definitionId = new Guid("476CC49C-7FAB-482A-B5F3-F91772AF9EDF");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<TransactionType> GetAllTransactionTypes()
        {
            var records = GetCachedTransactionTypes();
            List<TransactionType> transactionTypes = null;

            if (records != null && records.Count > 0)
            {
                transactionTypes = new List<TransactionType>();
                foreach (var record in records)
                {
                    transactionTypes.Add(record.Value);
                }
            }
            return transactionTypes;
        }


        public IEnumerable<TransactionTypeDetail> GetTransactionTypesInfo(TransactionTypeInfoFilter filter)
        {
            var transactionTypes = GetCachedTransactionTypes();
            Func<TransactionType, bool> filterFunc = (transactionType) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new TransactionTypeFilterContext
                        {
                            TransactionType = transactionType
                        };
                        foreach (var transactionTypeFilter in filter.Filters)
                        {
                            if (!transactionTypeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return transactionTypes.MapRecords((record) =>
            {
                return TransactionTypeInfoMapper(record);
            }, filterFunc);

        }
        private Dictionary<Guid, TransactionType> GetCachedTransactionTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTransactionTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, TransactionType> result = new Dictionary<Guid, TransactionType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        TransactionType transactionType = new TransactionType()
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

        private TransactionTypeDetail TransactionTypeInfoMapper(TransactionType transactionType)
        {
            return new TransactionTypeDetail
            {
                ID = transactionType.ID,
                Name = transactionType.Name
            };
        }

    }
  
}
