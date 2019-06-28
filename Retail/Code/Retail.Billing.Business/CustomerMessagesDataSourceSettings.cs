using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Retail.Billing.Entities;
using Vanrise.Invoice.Business;

namespace Retail.Billing.Business
{
    public enum CustomerMessagesType
    {
        Global = 1,
        Specific = 2
    }

    public class CustomerMessagesDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("128AA4A3-719F-4C60-8908-AA652ED612D7"); } }
        public CustomerMessagesType Type { get; set; }

        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var genericBusinessEntityManager = new GenericBusinessEntityManager();
            var invoice = context.InvoiceActionContext.GetInvoice();

            if (invoice == null)
                return null;

            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoice.InvoiceTypeId);
            var invoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<BillingInvoiceSettings>("invoiceTypeExtendedSettings", invoice.InvoiceTypeId);

            if (invoiceSettings == null)
                return null;

            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(invoiceSettings.Configuration);
            var financialAccount = financialAccountManager.GetFinancialAccount(invoice.PartnerId);

            if (financialAccount == null || financialAccount.ExtraFields == null || financialAccount.ExtraFields.Count == 0)
                return null;

            var customerId = financialAccount.ExtraFields.GetRecord("Customer");

            var customerEntity = new CustomerBeManager().GetCustomerEntity(customerId);
            if (customerEntity == null || customerEntity.FieldValues == null || customerEntity.FieldValues.Count == 0)
                return null;

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup()
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>() { }
            };

            switch (Type)
            {
                case CustomerMessagesType.Global:
                    {
                        recordFilterGroup.Filters.Add(new NumberListRecordFilter()
                        {
                            FieldName = "Type",
                            CompareOperator = ListRecordFilterOperator.In,
                            Values = new List<decimal>() { (int)CustomerMessagesType.Global }
                        });
                        break;
                    }
                case CustomerMessagesType.Specific:
                    {
                        var customerTypeId = customerEntity.FieldValues.GetRecord("CustomerType");
                        recordFilterGroup.Filters.Add(new ObjectListRecordFilter()
                        {
                            FieldName = "CustomerType",
                            CompareOperator = ListRecordFilterOperator.In,
                            Values = new List<object>() { customerTypeId.ToString() }
                        });

                        var customerCategory = customerEntity.FieldValues.GetRecord("CustomerCategory");
                        recordFilterGroup.Filters.Add(new ObjectListRecordFilter()
                        {
                            FieldName = "CustomerCategory",
                            CompareOperator = ListRecordFilterOperator.In,
                            Values = new List<object>() { customerCategory.ToString() }
                        });

                        var customerSubCategory = customerEntity.FieldValues.GetRecord("CustomerSubCategory");
                        recordFilterGroup.Filters.Add(new ObjectListRecordFilter()
                        {
                            FieldName = "CustomerSubCategory",
                            CompareOperator = ListRecordFilterOperator.In,
                            Values = new List<object>() { customerSubCategory.ToString() }
                        });
                        break;
                    }
            }

            return new CustomerMessageManager().GetCustomerMessages(recordFilterGroup);
        }

    }
}
