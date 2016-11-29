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
using Vanrise.Invoice.Business.Extensions;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

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
        public UpdateOperationOutput<InvoiceDetail> ReGenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            long insertedInvoiceId = -1;
            try
            {
                var currentInvocie = GetInvoice(createInvoiceInput.InvoiceId.Value);
                if (!currentInvocie.LockDate.HasValue)
                {
                    InvoiceTypeManager manager = new InvoiceTypeManager();
                    var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);
                    GeneratedInvoice generatedInvoice = BuidGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId);
                    var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails);
                    invoice.SerialNumber = currentInvocie.SerialNumber;
                    if (SaveInvoice(generatedInvoice.InvoiceItemSets, invoice, createInvoiceInput.InvoiceId, out insertedInvoiceId))
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId));
                        updateOperationOutput.UpdatedObject = invoiceDetail;
                        updateOperationOutput.Message = "Invoice Generated Successfully.";
                        updateOperationOutput.ShowExactMessage = true;
                    }
                    else
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                    }

                }else
                {
                    throw new InvoiceGeneratorException(string.Format("Invoice {0} is Locked.", createInvoiceInput.InvoiceId));
                }
            }
            catch (Exception e)
            {
                if (e as InvoiceGeneratorException != null)
                {
                    updateOperationOutput.Message = e.Message;
                    updateOperationOutput.ShowExactMessage = true;
                }
                else
                    throw e;
            }
            return updateOperationOutput;
        }
        public InsertOperationOutput<InvoiceDetail> GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long insertedInvoiceId = -1;

            try
            {
                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);
                GeneratedInvoice generatedInvoice = BuidGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId);
                var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails);

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

                if (SaveInvoice(generatedInvoice.InvoiceItemSets,invoice,null,out insertedInvoiceId))
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId));
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
        public bool UpdateInvoiceNote(long invoiceId, string invoiceNote)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.UpdateInvoiceNote(invoiceId, invoiceNote);
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
        public void SendEmail(VRMailEvaluatedTemplate invoiceTemplate)
        {
            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(invoiceTemplate.To, invoiceTemplate.CC, invoiceTemplate.Subject, invoiceTemplate.Body);
        }
        public VRMailEvaluatedTemplate GetInvoiceTemplate(long invoiceId)
        {
            var invoice = GetInvoice(invoiceId);
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);
            InvoiceTypeExtendedSettingsInfoContext context = new InvoiceTypeExtendedSettingsInfoContext
            {
                InfoType = "CustomerMailTemplate",
                Invoice = invoice

            };
          return invoiceType.Settings.ExtendedSettings.GetInfo(context);
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
                UserName = userManager.GetUserName(invoice.UserId),
                HasNote = invoice.Note != null
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

        #region Private Methods
        private Entities.Invoice BuildInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic invoiceDetails)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                UserId = new SecurityContext().GetLoggedInUserId(),
                Details = invoiceDetails,
                InvoiceTypeId = invoiceType.InvoiceTypeId,
                FromDate = fromDate,
                PartnerId = partnerId,
                ToDate = toDate,
                IssueDate = issueDate,
            };

            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerSettings();
            PartnerManager partnerManager = new PartnerManager();
            var duePeriod = partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, partnerId);
            invoice.DueDate = issueDate.AddDays(duePeriod);
            return invoice;
        }
        private GeneratedInvoice BuidGeneratedInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic customSectionPayload, long? invoiceId)
        {
            if (CheckInvoiceOverlaping(invoiceType.InvoiceTypeId, partnerId, fromDate, toDate,invoiceId))
            {
                throw new InvoiceGeneratorException("Invoices must not overlapped.");
            }

            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                CustomSectionPayload = customSectionPayload,
                FromDate = fromDate,
                PartnerId = partnerId,
                ToDate = toDate,
                InvoiceTypeId = invoiceType.InvoiceTypeId,

            };
            var generator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
            generator.GenerateInvoice(context);
            return context.Invoice;

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
                    var invoiceAction = invoiceType.Settings.InvoiceActions.FirstOrDefault(x => x.InvoiceActionId == action.InvoiceGridActionId);

                    if (invoiceDetail.ActionTypeNames == null)
                        invoiceDetail.ActionTypeNames = new List<InvoiceGridAction>();
                    if (DoesUserHaveAccess(invoiceAction.RequiredPermission))
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

        private static bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);
        }

        private bool SaveInvoice(List<GeneratedInvoiceItemSet> invoiceItemSets,Entities.Invoice invoice,long? invoiceIdToDelete,out long invoiceId)
        {
             IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
             return dataManager.SaveInvoices(invoiceItemSets, invoice,invoiceIdToDelete, out invoiceId);
        }
        #endregion
    }
}
