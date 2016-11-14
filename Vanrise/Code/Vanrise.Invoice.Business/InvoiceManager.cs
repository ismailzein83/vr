using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Invoice.Business
{
    public class InvoiceManager
    {

        #region Public Methods
        public IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(input.Query.InvoiceTypeId);
        
            var result = BigDataManager.Instance.RetrieveData(input,  new InvoiceRequestHandler()) as Vanrise.Entities.BigResult<InvoiceDetail>;

            if(result != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                foreach(var data in result.Data)
                {
                    FillNeededDetailData(data, invoiceType);
                }
            }
            return result;
        }
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoice(invoiceId);
        }
        public InsertOperationOutput<InvoiceDetail> GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long insertedInvoiceId = -1;


            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);

            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                CustomSectionPayload = createInvoiceInput.CustomSectionPayload,
                FromDate = createInvoiceInput.FromDate,
                PartnerId = createInvoiceInput.PartnerId,
                ToDate = createInvoiceInput.ToDate,
                InvoiceTypeId = createInvoiceInput.InvoiceTypeId
            };
            invoiceType.Settings.InvoiceGenerator.GenerateInvoice(context);
            
            Entities.Invoice invoice = new Entities.Invoice
            {
                UserId = new SecurityContext().GetLoggedInUserId(),
                Details = context.Invoice.InvoiceDetails,
                InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                FromDate = createInvoiceInput.FromDate,
                PartnerId = createInvoiceInput.PartnerId,
                ToDate = createInvoiceInput.ToDate,
                IssueDate = createInvoiceInput.IssueDate,
            };
            PartnerManager partnerManager = new PartnerManager();
            var duePeriod = partnerManager.GetPartnerDuePeriod(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId);
            invoice.DueDate = createInvoiceInput.IssueDate.AddDays(duePeriod);
            var serialNumber = invoiceType.Settings.SerialNumberPattern;
            InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext{
                Invoice = invoice,
                InvoiceTypeId = createInvoiceInput.InvoiceTypeId
            };
            foreach(var part in invoiceType.Settings.SerialNumberParts)
            {
                if(invoiceType.Settings.SerialNumberPattern != null && invoiceType.Settings.SerialNumberPattern.Contains(string.Format("#{0}#", part.VariableName)))
                {
                    serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
                }
            }
            invoice.SerialNumber = serialNumber;
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            if (dataManager.SaveInvoices(context.Invoice.InvoiceItemSets,invoice,out insertedInvoiceId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = InvoiceDetailMapper(GetInvoice(insertedInvoiceId));
                insertOperationOutput.Message = "Invoice Generated Successfully.";
                insertOperationOutput.ShowExactMessage = true;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public bool SetInvoicePaid(long invoiceId, bool isInvoicePaid)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            DateTime? paidDate = null;
            if (isInvoicePaid)
                paidDate = DateTime.Now;
            return dataManager.SetInvoicePaid(invoiceId, paidDate);
        }
        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoiceCount(InvoiceTypeId, partnerId, fromDate, toDate);
        }
        public Entities.InvoiceDetail GetInvoiceDetail(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var invoiceDetail = InvoiceDetailMapper(dataManager.GetInvoice(invoiceId));
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceDetail.Entity.InvoiceTypeId);
            FillNeededDetailData(invoiceDetail, invoiceType);
            return invoiceDetail;
        }

        private void FillNeededDetailData(InvoiceDetail invoiceDetail, InvoiceType invoiceType)
        {
            DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(invoiceDetail.Entity.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            foreach (var section in invoiceType.Settings.UISettings.SubSections)
            {
                if (recordFilterManager.IsFilterGroupMatch(section.FilterGroup, context))
                {
                    if (invoiceDetail.SectionsTitle == null)
                        invoiceDetail.SectionsTitle = new List<string>();
                    invoiceDetail.SectionsTitle.Add(section.SectionTitle);
                }
            }
            InvoiceFilterConditionContext invoiceFilterConditionContext = new InvoiceFilterConditionContext { Invoice = invoiceDetail.Entity, InvoiceType = invoiceType };
            foreach (var action in invoiceType.Settings.UISettings.InvoiceGridActions)
            {
                if (action.InvoiceFilterCondition == null || action.InvoiceFilterCondition.IsFilterMatch(invoiceFilterConditionContext))
                {
                    if (invoiceDetail.ActionTypeNames == null)
                        invoiceDetail.ActionTypeNames = new List<InvoiceGridAction>();
                    invoiceDetail.ActionTypeNames.Add(action);
                }
            }
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordType = dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            invoiceDetail.Items = new List<InvoiceDetailObject>();
            foreach (var field in dataRecordType.Fields)
            {
                var fieldValue = Vanrise.Common.Utilities.GetPropValueReader(field.Name).GetPropertyValue(invoiceDetail.Entity.Details);
                if (fieldValue != null)
                {
                    invoiceDetail.Items.Add(new InvoiceDetailObject
                    {
                        FieldName = field.Name,
                        Description = field.Type.GetDescription(fieldValue),
                        Value = fieldValue
                    });
                }
            }
        }
        #endregion

        #region Mappers

        private static InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);
            string partnerName = null;
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.UISettings != null && invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN != null)
            {
                PartnerManagerContext context = new PartnerManagerContext
                {
                    PartnerId = invoice.PartnerId
                };
                partnerName = invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN.GetPartnerName(context);
            }
            UserManager userManager = new UserManager();
            return new InvoiceDetail
            {
                Entity = invoice,
                PartnerName = partnerName,
                Paid = invoice.PaidDate.HasValue,
                UserName = userManager.GetUserName(invoice.UserId)
            };
        }

        #endregion

        #region Private Classes

        private class InvoiceRequestHandler : BigDataRequestHandler<InvoiceQuery, Entities.Invoice, Entities.InvoiceDetail>
        {
            public InvoiceRequestHandler()
            {
               
            }
            public override InvoiceDetail EntityDetailMapper(Entities.Invoice entity)
            {
                return InvoiceManager.InvoiceDetailMapper(entity);
            }
            public override IEnumerable<Entities.Invoice> RetrieveAllData(DataRetrievalInput<InvoiceQuery> input)
            {
                IInvoiceDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
                return _dataManager.GetFilteredInvoices(input);
            }
        }
        #endregion
    }
}
