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
    public class WhSJazzAccountCodeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid _definitionId = new Guid("005F9C7F-3213-4BBE-B1D0-560423008B30");

        public GenericBusinessEntity GetAccountCodeGenericBusinessEntity(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);

        }

        public List<WhSJazzAccountCode> GetAllAccountCodes()
        {
            var records = GetCachedAccountCodes();
            List<WhSJazzAccountCode> accountCodes = null;

            if (records != null && records.Count > 0)
            {
                accountCodes = new List<WhSJazzAccountCode>();
                foreach (var record in records)
                {
                    accountCodes.Add(record.Value);
                }
            }
            return accountCodes;
        }

        private Dictionary<Guid, WhSJazzAccountCode> GetCachedAccountCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzAccountCode> result = new Dictionary<Guid, WhSJazzAccountCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzAccountCode accountCode = new WhSJazzAccountCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            TransactionTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("TransactionTypeId"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(accountCode.ID, accountCode);
                    }
                }

                return result;
            });
        }

        public IEnumerable<WhSJazzAccountCodeDetail> GetAccountCodesInfo(WhSJazzAccountCodeInfoFilter filter)

        {
        var accountCodes = GetCachedAccountCodes();

            Func<WhSJazzAccountCode, bool> filterFunc = (accountCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzAccountCodeFilterContext
                        {
                            AccountCode = accountCode
                        };
                        foreach (var accountCodeFilter in filter.Filters)
                        {
                            if (!accountCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return accountCodes.MapRecords((record) =>
            {
                return AccountCodeInfoMapper(record);
            }, filterFunc);
        }

        private WhSJazzAccountCodeDetail AccountCodeInfoMapper(WhSJazzAccountCode accountCode)
        {
            return new WhSJazzAccountCodeDetail
            {
                ID = accountCode.ID,
                Name = accountCode.Name
            };
        }

    }

}


