using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Retail.Billing.Entities;

namespace Retail.Billing.Business
{
    public class CustomerMessagesDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("128AA4A3-719F-4C60-8908-AA652ED612D7"); } }
        public Guid CustomerMessagesBeDefinitionId { get; set; }
        public string MessageFieldName { get; set; }
        public string CategoryFieldName { get; set; }

        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var entities = GetAllMessages();
            if (entities == null)
                return null;

            List<CustomerMessage> customerMessagesWithNullCategory = new List<CustomerMessage>();
            List<CustomerMessage> customerMessages = new List<CustomerMessage>();

            foreach (var entity in entities)
            {
                if (entity.FieldValues == null || entity.FieldValues.Count == 0)
                    continue;

                if (entity.FieldValues.GetRecord(CategoryFieldName) == null)
                {
                    customerMessagesWithNullCategory.Add(new CustomerMessage() { Message = entity.FieldValues.GetRecord(MessageFieldName).ToString() });
                }
                else
                {
                    customerMessages.Add(new CustomerMessage() { Message = entity.FieldValues.GetRecord(MessageFieldName).ToString() });
                }
            }

            customerMessages.AddRange(customerMessagesWithNullCategory);
            return customerMessages;
        }

        #region Private

        public List<GenericBusinessEntity> GetAllMessages(RecordFilterGroup filterGroup = null)
        {
            return new GenericBusinessEntityManager().GetAllGenericBusinessEntities(CustomerMessagesBeDefinitionId, null, filterGroup);
        }

        #endregion

    }
}
