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

            return  BigDataManager.Instance.RetrieveData(input, new InvoiceRequestHandler()) as Vanrise.Entities.BigResult<InvoiceDetail>;
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

            try
            {
                var fromDate =createInvoiceInput.FromDate;
                var toDate =  createInvoiceInput.ToDate.AddDays(1);
                if (CheckInvoiceOverlaping(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId, fromDate, toDate))
                {
                    throw new InvoiceGeneratorException("Invoices must not overlapped.");
                }
                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);
                InvoiceGenerationContext context = new InvoiceGenerationContext
                {
                    CustomSectionPayload = createInvoiceInput.CustomSectionPayload,
                    FromDate = fromDate,
                    PartnerId = createInvoiceInput.PartnerId,
                    ToDate = toDate,
                    InvoiceTypeId = createInvoiceInput.InvoiceTypeId
                };
                var generator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
                generator.GenerateInvoice(context);

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
                var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerSettings();
                PartnerManager partnerManager = new PartnerManager();
                var duePeriod = partnerManager.GetPartnerDuePeriod(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId);
                invoice.DueDate = createInvoiceInput.IssueDate.AddDays(duePeriod);
                var serialNumber = invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberPattern;
                InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext
                {
                    Invoice = invoice,
                    InvoiceTypeId = createInvoiceInput.InvoiceTypeId
                };
                foreach (var part in invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberParts)
                {
                    if (invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberPattern != null && invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberPattern.Contains(string.Format("#{0}#", part.VariableName)))
                    {
                        serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
                    }
                }
                invoice.SerialNumber = serialNumber;
                IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
                if (dataManager.SaveInvoices(context.Invoice.InvoiceItemSets, invoice, out insertedInvoiceId))
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId));
                    FillNeededDetailData(invoiceDetail, invoiceType);
                    insertOperationOutput.InsertedObject = invoiceDetail;
                    insertOperationOutput.Message = "Invoice Generated Successfully.";
                    insertOperationOutput.ShowExactMessage = true;
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            catch (Exception e)
            {
                if (e as InvoiceGeneratorException != null)
                {
                    insertOperationOutput.Message = e.Message;
                    insertOperationOutput.ShowExactMessage = true;
                }


                else
                    throw e;
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
        public bool SetInvoiceLocked(long invoiceId, bool setLocked)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            DateTime? lockedDate = null;
            if (setLocked)
                lockedDate = DateTime.Now;
            return dataManager.SetInvoiceLocked(invoiceId, lockedDate);
        }
        public int GetInvoiceCount(Guid InvoiceTypeId, string partnerId, DateTime? fromDate, DateTime? toDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoiceCount(InvoiceTypeId, partnerId, fromDate, toDate);
        }
        public Entities.InvoiceDetail GetInvoiceDetail(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return InvoiceDetailMapper(dataManager.GetInvoice(invoiceId));
        }

        private static void FillNeededDetailData(InvoiceDetail invoiceDetail, InvoiceType invoiceType)
        {
            DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(invoiceDetail.Entity.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            foreach (var section in invoiceType.Settings.SubSections)
            {
                if (recordFilterManager.IsFilterGroupMatch(section.SubSectionFilter, context))
                {
                    if (invoiceDetail.SectionsTitle == null)
                        invoiceDetail.SectionsTitle = new List<string>();
                    invoiceDetail.SectionsTitle.Add(section.SectionTitle);
                }
            }
            InvoiceGridActionFilterConditionContext invoiceFilterConditionContext = new InvoiceGridActionFilterConditionContext { Invoice = invoiceDetail.Entity, InvoiceType = invoiceType };
            foreach (var action in invoiceType.Settings.InvoiceGridSettings.InvoiceGridActions)
            {
                if (action.FilterCondition == null || action.FilterCondition.IsFilterMatch(invoiceFilterConditionContext))
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
        private bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId,fromDate, toDate);
        }
        #endregion

        #region Mappers

        private static InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice)
        {

            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);


            string partnerName = null;
            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerSettings();
            if (partnerSettings != null)
            {
                PartnerNameManagerContext context = new PartnerNameManagerContext
                {
                    PartnerId = invoice.PartnerId
                };
                partnerName = partnerSettings.GetPartnerName(context);
            }
            UserManager userManager = new UserManager();
            InvoiceDetail invoiceDetail = new InvoiceDetail
            {
                Entity = invoice,
                PartnerName = partnerName,
                Paid = invoice.PaidDate.HasValue,
                Lock = invoice.LockDate.HasValue,
                UserName = userManager.GetUserName(invoice.UserId)
            };
            FillNeededDetailData(invoiceDetail, invoiceType);
            return invoiceDetail;
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
