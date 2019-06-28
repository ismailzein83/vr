using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class CustomerBeManager
    {
        static Guid s_customerGenericBeDefinitionId = new Guid("61017ff8-3e6d-48f4-83bc-a8b4733ab5ee");
        private GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        #region Public Methods

        public GenericBusinessEntity GetCustomerEntity(Object customerId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(customerId, s_customerGenericBeDefinitionId);
        }

        public Object GetCustomerObject(Object customerId)
        {
            return _genericBusinessEntityManager.GetGenericBEObject(s_customerGenericBeDefinitionId, customerId);
        }
        #endregion
    }
}
