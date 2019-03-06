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
    public class AccountCodeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid _definitionId = new Guid("005F9C7F-3213-4BBE-B1D0-560423008B30");

        public GenericBusinessEntity GetAccountCodeGenericBusinessEntity(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);

        }

        public List<AccountCode> GetAllAccountCodes()
        {
            return GetCachedAccountCodes().Values.ToList();
        }

        private Dictionary<Guid, AccountCode> GetCachedAccountCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, AccountCode> result = new Dictionary<Guid, AccountCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        AccountCode accountCode = new AccountCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            TransactionTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("TransactionTypeId"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            Carriers=(AccountCodeCarriers)genericBusinessEntity.FieldValues.GetRecord("Carriers"),
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

        public IEnumerable<AccountCodeDetail> GetAccountCodesInfo(AccountCodeInfoFilter filter)

        {
        var accountCodes = GetCachedAccountCodes();

            Func<AccountCode, bool> filterFunc = (accountCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new AccountCodeFilterContext
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

        public bool ValidateAccountCode(GenericBusinessEntity genericBusinessEntity, HandlerOperationType operationType)
        {
            AccountCode accountCode = new AccountCode
            {
                Carriers = (AccountCodeCarriers)genericBusinessEntity.FieldValues.GetRecord("Carriers"),
                SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                TransactionTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("TransactionTypeId")
            };

            if (operationType == HandlerOperationType.Update)
            {
                accountCode.ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID");
            }
       
            var accountCodes = GetCachedAccountCodes();

            if(accountCodes!=null && accountCodes.Count > 0)
            {
                foreach(var item in accountCodes)
                {
                    if (item.Value.ID!=accountCode.ID &&
                        item.Value.TransactionTypeId == accountCode.TransactionTypeId && 
                        item.Value.SwitchId == accountCode.SwitchId &&
                        item.Value.Carriers.Carriers.Any(x => accountCode.Carriers.Carriers.Any(y => y.CarrierAccountId == x.CarrierAccountId)))
                        return false;
                }
            }
            return true;
        }
        public IEnumerable<AccountCode> GetAccountCodes(Guid transactionTypeId,int switchId)
        {
            var accountCodes = GetCachedAccountCodes();

            Func<AccountCode, bool> filterFunc = (accountCode) =>
            {
                {
                    if (accountCode.SwitchId != switchId) return false;
                    if (accountCode.TransactionTypeId != transactionTypeId) return false;
                }
                return true;
            };
            return accountCodes.MapRecords((record) =>
            {
                return record;
            }, filterFunc);

        }
        private AccountCodeDetail AccountCodeInfoMapper(AccountCode accountCode)
        {
            return new AccountCodeDetail
            {
                ID = accountCode.ID,
                Name = accountCode.Name
            };
        }

    }

}


