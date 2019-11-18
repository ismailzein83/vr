using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class PhoneNumberManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
       
        #region PhoneNumberStatus
        static Guid s_freePhoneNumberStatusDefinitionId = new Guid("f6cab3e1-3cfb-4efd-80d1-9feea7c38da4");
        static Guid s_reservedPhoneNumberStatusDefinitionId = new Guid("01014c67-49f8-4959-b56e-d10aaaa21319");
        static Guid s_usedPhoneNumberStatusDefinitionId = new Guid("1c67115b-2347-4f69-a04c-ab3e20a9b4cf");
        static Guid s_errorPhoneNumberStatusDefinitionId = new Guid("571844c1-83c2-4e51-9873-df08a736ea68");
        #endregion

        static Guid s_phoneNumberBEDefinitionId = new Guid("71e1a021-659e-4379-b858-8f6269449894");
        static string s_statusIdFieldName = "Status";

        #region Public Methods
        public ReservePhoneNumberOutput ReservePhoneNumber(ReservePhoneNumberInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_phoneNumberBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_reservedPhoneNumberStatusDefinitionId } },
                GenericBusinessEntityId = input.PhoneNumberId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = s_statusIdFieldName,
                              Values = new List<object> { s_freePhoneNumberStatusDefinitionId }
                        }
                   }
                }
            });

            if (updatedEntity.Result == UpdateOperationResult.Failed)
                return null;

            return new ReservePhoneNumberOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }
        public SetPhoneNumberUsedOutput SetPhoneNumberUsed(SetPhoneNumberUsedInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_phoneNumberBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_usedPhoneNumberStatusDefinitionId } },
                GenericBusinessEntityId = input.PhoneNumberId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = s_statusIdFieldName,
                              CompareOperator = ListRecordFilterOperator.NotIn,
                              Values = new List<object> { s_usedPhoneNumberStatusDefinitionId }
                        }
                   }
                }
            });

            return new SetPhoneNumberUsedOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }
        #endregion
    }
}
