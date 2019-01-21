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
    public class WhsJazzAccountCodeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid _definitionId = new Guid("005F9C7F-3213-4BBE-B1D0-560423008B30");

        public GenericBusinessEntity GetAccountCodeGenericBusinessEntity(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);

        }

        public List<WhSJazzAccountCode> GetAllAccountCode()
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

    }


}

