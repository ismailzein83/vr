using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerAssignmentManager
    {
        static Guid businessEntityDefinitionId = new Guid("a2e4b8e6-2605-40e6-8184-8798add51bab");

        #region Public Methods

        public IEnumerable<AccountManagerAssignment> GetAllAccountManagersAssignmentByCarrierAccountId(int carrierAccountId)
        {
            return GetCachedAccountManagerAssignment().FindAllRecords(itm => itm.CarrierAccountId == carrierAccountId);
        }

        public IEnumerable<int> GetAffectedCarrierAccountIds()
        {
            List<int> affectedCarrierAccountIds = new List<int>();
            var allAccountManagerAssignments = GetCachedAccountManagerAssignment().Values;
            foreach (var assignment in allAccountManagerAssignments)
            {
                affectedCarrierAccountIds.Add(assignment.CarrierAccountId);
            }
            return affectedCarrierAccountIds;
        }

        #endregion

        #region Private Methods
        private Dictionary<int, AccountManagerAssignment> GetCachedAccountManagerAssignment()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManagerAssignment", businessEntityDefinitionId, () =>
            {
                Dictionary<int, AccountManagerAssignment> result = new Dictionary<int, AccountManagerAssignment>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        AccountManagerAssignment accountManagerAssignment = new AccountManagerAssignment()
                        {
                            AccountManagerAssignmentId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            AccountManagerId = (int)genericBusinessEntity.FieldValues.GetRecord("AccountManagerId"),
                            CarrierAccountId = (int)genericBusinessEntity.FieldValues.GetRecord("CarrierAccountId"),
                            CustomerAssigned = (bool)genericBusinessEntity.FieldValues.GetRecord("CustomerAssigned"),
                            SupplierAssigned = (bool)genericBusinessEntity.FieldValues.GetRecord("SupplierAssigned"),
                            BED = (DateTime)genericBusinessEntity.FieldValues.GetRecord("BED"),
                            EED = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("EED"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                        };
                        result.Add(accountManagerAssignment.AccountManagerAssignmentId, accountManagerAssignment);
                    }
                }
                return result;
            });
        }
        #endregion
    }
}
