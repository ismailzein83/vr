using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class CustomerMessageManager
    {
        static Guid s_customerMessagesBeDefinitionId = new Guid("41283ade-a5b1-403d-886d-724390812fe0");
        private GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        #region Public Methods

        public List<CustomerMessage> GetCustomerMessages(RecordFilterGroup filterGroup = null)
        {
            List<CustomerMessage> customerMessages = new List<CustomerMessage>();
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(s_customerMessagesBeDefinitionId, null, filterGroup);

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (entity.FieldValues == null || entity.FieldValues.Count == 0)
                        continue;

                    customerMessages.Add(new CustomerMessage() { Message = entity.FieldValues.GetRecord("Message").ToString() });
                }
            }

            return customerMessages;
        }

        #endregion
    }
}
