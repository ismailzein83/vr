using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.Jazz.Business
{
    public class WhSJazzTransactionTypeManager
    {
        public static Guid _definitionId = new Guid("476CC49C-7FAB-482A-B5F3-F91772AF9EDF");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzTransactionType> GetAllTransactionTypes()
        {
            var records = GetCachedTransacionTypes();
            List<WhSJazzTransactionType> transactionTypes = null;

            if (records != null && records.Count > 0)
            {
                transactionTypes = new List<WhSJazzTransactionType>();
                foreach (var record in records)
                {
                    transactionTypes.Add(record.Value);
                }
            }
            return transactionTypes;
        }


        public IEnumerable<WhSJazzTransactionTypeDetail> GetTransactionTypesInfo(WhSJazzTransactionTypeInfoFilter filter)
        {
            var transactionTypes = GetCachedTransactionTypes();
            Func<WhSJazztransactionType, bool> filterFunc = (transactionType) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzTransactionTypeFilterContext
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
        private Dictionary<Guid, WhSJazzTransactionType> GetCachedTransactionTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTransactionTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzTransactionType> result = new Dictionary<Guid, WhSJazzTransactionType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzTransactionType transactionType = new WhSJazzTransactionType()
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

    }

    public class WhSJazzTransactionType
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
    public class WhSJazzTransactionTypeDetail
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

    }
    public class WhSJazzTransactionTypeInfoFilter
    {
        public IEnumerable<IWhSJazzTransactionTypeFilter> Filters { get; set; }

    }


    public interface IWhSJazzTransactionTypeFilter
    {
        bool IsMatch(IWhSJazzTransactionTypeFilterContext context);
    }

    public interface IWhSJazzTransactionTypeFilterContext
    {
        WhSJazzTransactionType SwitchCode { get; }
    }

    public class WhSJazzTransactionTypeFilterContext : IWhSJazzTransactionTypeFilterContext
    {
        public WhSJazzTransactionType SwitchCode { get; set; }
    }
}
