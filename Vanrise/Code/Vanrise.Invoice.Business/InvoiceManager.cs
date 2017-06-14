﻿using System;
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
            return GetFilteredInvoices(input, false);
        }

        public IDataRetrievalResult<InvoiceDetail> GetFilteredClientInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            return GetFilteredInvoices(input, true);
        }
        private IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input, bool getClientInvoices)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(input.Query.InvoiceTypeId);

            var bigResult = BigDataManager.Instance.RetrieveData(input, new InvoiceRequestHandler()) as Vanrise.Entities.BigResult<InvoiceDetail>;
            if (!getClientInvoices && bigResult.Data != null && input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                foreach (var accountDetail in bigResult.Data)
                {
                    InvoiceDetailMapper2(accountDetail, invoiceType);
                }
            }
            return bigResult;
        }
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoice(invoiceId);
        }

        public Entities.Invoice GetInvoiceBySourceId(Guid invoiceTypeId, string sourceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoiceBySourceId(invoiceTypeId, sourceId);
        }

        public bool UpdateInvoicePaidDateBySourceId(Guid invoiceTypeId, string sourceId, DateTime paidDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.UpdateInvoicePaidDateBySourceId(invoiceTypeId, sourceId, paidDate);
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
                    string offset = null;

                    if (createInvoiceInput.TimeZoneId.HasValue)
                    {
                        VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(createInvoiceInput.TimeZoneId.Value);
                        if (timeZone != null)
                        {
                            offset = timeZone.Settings.Offset.ToString();
                            createInvoiceInput.FromDate = createInvoiceInput.FromDate.Add(-timeZone.Settings.Offset);
                            createInvoiceInput.ToDate = createInvoiceInput.ToDate.Add(-timeZone.Settings.Offset);
                        }
                    }
                    PartnerManager partnerManager = new PartnerManager();
                    var duePeriod = partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);

                    IEnumerable<GeneratedInvoiceBillingTransaction> billingTarnsactions;
                    GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod, out billingTarnsactions);

                    Entities.Invoice invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.TimeZoneId, offset, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod);
                    invoice.SerialNumber = currentInvocie.SerialNumber;
                    invoice.Note = currentInvocie.Note;

                    if (SaveInvoice(generatedInvoice.InvoiceItemSets, invoice, createInvoiceInput.InvoiceId, billingTarnsactions, out insertedInvoiceId))
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId), invoiceType);
                        updateOperationOutput.UpdatedObject = invoiceDetail;
                        updateOperationOutput.Message = "Invoice Generated Successfully.";
                        updateOperationOutput.ShowExactMessage = true;
                    }
                    else
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                    }

                }
                else
                {
                    throw new InvoiceGeneratorException(string.Format("Invoice {0} is Locked.", createInvoiceInput.InvoiceId));
                }
            }
            catch (InvoiceGeneratorException e)
            {
                updateOperationOutput.Message = e.Message;
                updateOperationOutput.ShowExactMessage = true;
            }
            return updateOperationOutput;
        }
        public DateTime? CheckGeneratedInvoicePeriodGaP(DateTime fromDate, Guid invoiceTypeId, string partnerId)
        {
            BillingPeriodInfoManager billingPeriodInfoManager = new BillingPeriodInfoManager();
            var billingPeriodInfo = billingPeriodInfoManager.GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            if (billingPeriodInfo == null)
                return null;
            return (billingPeriodInfo.NextPeriodStart < fromDate) ? billingPeriodInfo.NextPeriodStart : default(DateTime?);
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
                string offset = null;
                DateTime fromDate = createInvoiceInput.FromDate;
                DateTime toDate = createInvoiceInput.ToDate;
                if (createInvoiceInput.TimeZoneId.HasValue)
                {
                    VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(createInvoiceInput.TimeZoneId.Value);
                    if (timeZone != null)
                    {
                        offset = timeZone.Settings.Offset.ToString();
                        fromDate = createInvoiceInput.FromDate.Add(-timeZone.Settings.Offset);
                        toDate = createInvoiceInput.ToDate.Add(-timeZone.Settings.Offset);
                    }
                }

                PartnerManager partnerManager = new PartnerManager();
                var duePeriod = partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);


                IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions;
                GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, fromDate, toDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod, out billingTransactions);


                if (generatedInvoice.InvoiceDetails == null)
                {
                    throw new InvoiceGeneratorException("No data available between the selected period.");
                }


                var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.TimeZoneId, offset, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod);

                var serialNumber = new PartnerManager().GetPartnerSerialNumberPattern(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId); InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext
                {
                    Invoice = invoice,
                    InvoiceTypeId = createInvoiceInput.InvoiceTypeId
                };
                foreach (var part in invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberParts)
                {
                    if (serialNumber != null && serialNumber.Contains(string.Format("#{0}#", part.VariableName)))
                    {
                        serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
                    }
                }

                invoice.SerialNumber = serialNumber;

                if (SaveInvoice(generatedInvoice.InvoiceItemSets, invoice, null, billingTransactions, out insertedInvoiceId))
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId), invoiceType);
                    insertOperationOutput.InsertedObject = invoiceDetail;
                    insertOperationOutput.Message = "Invoice Generated Successfully.";
                    insertOperationOutput.ShowExactMessage = true;

                    BillingPeriodInfoManager billingPeriodInfoManager = new BillingPeriodInfoManager();
                    BillingPeriodInfo billingPeriodInfo = new BillingPeriodInfo
                    {
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                        PartnerId = createInvoiceInput.PartnerId,
                        NextPeriodStart = createInvoiceInput.ToDate.AddDays(1)
                    };
                    billingPeriodInfoManager.InsertOrUpdateBillingPeriodInfo(billingPeriodInfo);
                    var invoiceAccountData = partnerManager.GetInvoiceAccountData(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId);
                    if (invoiceAccountData == null)
                        throw new NullReferenceException("invoiceAccountData");
                    long invoiceAccountId;
                    new InvoiceAccountManager().TryAddInvoiceAccount(new VRInvoiceAccount
                    {
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                        Status = VRAccountStatus.Active,
                        IsDeleted = false,
                        PartnerId = createInvoiceInput.PartnerId,
                        BED = invoiceAccountData.BED,
                        EED = invoiceAccountData.EED
                    }, out invoiceAccountId);
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

        public UpdateOperationOutput<InvoiceDetail> UpdateInvoice(Invoice.Entities.Invoice invoice)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);

            if (TryUpdateInvoice(invoice))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = InvoiceDetailMapper(invoice, invoiceType);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        internal bool TryUpdateInvoice(Invoice.Entities.Invoice invoice)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.Update(invoice);
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
            var invoice = dataManager.GetInvoice(invoiceId);
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoice.InvoiceTypeId);
            return InvoiceDetailMapper(invoice, invoiceType);
        }

        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            return new PartnerManager().CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerId);
        }
        public BillingInterval GetBillingInterval(Guid invoiceTypeId, string partnerId, DateTime issueDate)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            var billingperiod = new PartnerManager().GetPartnerBillingPeriod(invoiceTypeId, partnerId);
            if (billingperiod == null)
                return null;
            BillingInterval billingInterval = new Entities.BillingInterval();

            var billingPeriodInfo = new BillingPeriodInfoManager().GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            if (billingPeriodInfo != null)
            {
                BillingPeriodContext billingPeriodContext = new Context.BillingPeriodContext
                {
                    IssueDate = issueDate,
                    PreviousPeriodEndDate = billingPeriodInfo.NextPeriodStart
                };
                var billingInertval = billingperiod.GetPeriod(billingPeriodContext);
                billingInterval.FromDate = billingInertval.FromDate;
                billingInterval.ToDate = billingInertval.ToDate;
            }
            else
            {
                BillingPeriodContext billingPeriodContext = new Context.BillingPeriodContext
                {
                    IssueDate = issueDate,
                };
                billingInterval = billingperiod.GetPeriod(billingPeriodContext);

                //if(invoiceType.Settings.StartDateCalculationMethod != null)
                //{
                //    InitialPeriodInfoContext initialPeriodInfoContext = new Context.InitialPeriodInfoContext
                //    {
                //        PartnerId = partnerId
                //    };
                //    invoiceType.Settings.ExtendedSettings.GetInitialPeriodInfo(initialPeriodInfoContext);
                //    //StartDateCalculationMethodContext startDateCalculationMethodContext = new StartDateCalculationMethodContext
                //    //{
                //    //    //InitialStartDate = initialPeriodInfoContext.InitialStartDate,
                //    //    PartnerCreatedDate = initialPeriodInfoContext.PartnerCreationDate,
                //    //};
                //    //invoiceType.Settings.StartDateCalculationMethod.CalculateDate(startDateCalculationMethodContext);
                //    //if (startDateCalculationMethodContext.StartDate > billingInterval.FromDate && startDateCalculationMethodContext.StartDate < billingInterval.ToDate)
                //    //    billingInterval.FromDate = startDateCalculationMethodContext.StartDate;
                //}


            }
            return billingInterval;
        }

        public void LoadInvoicesAfterImportedId(Guid invoiceTypeId, long lastImportedId, Action<Entities.Invoice> onInvoiceReady)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            dataManager.LoadInvoicesAfterImportedId(invoiceTypeId, lastImportedId, onInvoiceReady);
        }
        public InvoiceEditorRuntime GetInvoiceEditorRuntime(long invoiceId)
        {
            InvoiceEditorRuntime invoiceEditorRuntime = null;
            var invoice = GetInvoice(invoiceId);
            if (invoice != null)
            {
                invoiceEditorRuntime = new InvoiceEditorRuntime();
                invoiceEditorRuntime.Invoice = invoice;
                //if(invoice.TimeZoneOffset != null)
                //{
                //    //TimeSpan invoiceTimeOffset;
                //    //if(TimeSpan.TryParse(invoice.TimeZoneOffset, out invoiceTimeOffset))
                //    //{

                //    //}
                //}
                //invoiceEditorRuntime.FromDate = invoice.FromDate;
                //invoiceEditorRuntime.ToDate = invoice.ToDate;
            }
            return invoiceEditorRuntime;
        }
        public byte[] DownloadAttachment(Guid invoiceTypeId, Guid invoiceAttachmentId, long invoiceId)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            if (invoiceType.Settings.InvoiceAttachments != null)
            {
                var invoiceAttachment = invoiceType.Settings.InvoiceAttachments.FirstOrDefault(x => x.InvoiceAttachmentId == invoiceAttachmentId);
                if (invoiceAttachment != null)
                {
                    InvoiceRDLCFileConverterContext context = new InvoiceRDLCFileConverterContext
                    {
                        InvoiceId = invoiceId
                    };
                    var invoiceFile = invoiceAttachment.InvoiceFileConverter.ConvertToInvoiceFile(context);
                    return invoiceFile.Content;
                }
            }
            return null;
        }

        public Dictionary<InvoiceByPartnerInvoiceType, List<Entities.Invoice>> GetUnPaidPartnerInvoicesDic(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var invoices = dataManager.GetUnPaidPartnerInvoices(partnerInvoiceTypes);
            if (invoices == null)
                return null;
            Dictionary<InvoiceByPartnerInvoiceType, List<Entities.Invoice>> invoicesByPartnerInvoiceType = new Dictionary<InvoiceByPartnerInvoiceType, List<Entities.Invoice>>();
            foreach (var invoice in invoices)
            {
                var partnerInvoices = invoicesByPartnerInvoiceType.GetOrCreateItem(new InvoiceByPartnerInvoiceType { InvoiceTypeId = invoice.InvoiceTypeId, PartnerId = invoice.PartnerId }, () =>
                {
                    return new List<Entities.Invoice>();
                });
                partnerInvoices.Add(invoice);
            }
            return invoicesByPartnerInvoiceType;
        }
        public IEnumerable<Entities.Invoice> GetUnPaidPartnerInvoices(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetUnPaidPartnerInvoices(partnerInvoiceTypes);
        }
        public InvoiceClientDetail GetLastInvoice(Guid invoiceTypeId, string partnerId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var invoice = dataManager.GetLastInvoice(invoiceTypeId, partnerId);
            if (invoice == null)
                return null;
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoice.InvoiceTypeId);
            return InvoiceDetailMapper1(invoice, invoiceType);
        }
        public Guid GetInvoiceTypeId(long invoiceId)
        {
            var invoice = GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoice);
            return invoice.InvoiceTypeId;

        }

        public IEnumerable<Entities.Invoice> GetLasInvoices(Guid invoiceTypeId, string partnerId, DateTime? beforeDate, int lastInvoices)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetLasInvoices(invoiceTypeId, partnerId, beforeDate, lastInvoices);
        }
        #endregion

        #region Mappers
        private static InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice, InvoiceType invoiceType)
        {

            var invoiceDetail = InvoiceDetailMapper1(invoice, invoiceType);
            InvoiceDetailMapper2(invoiceDetail, invoiceType);
            return invoiceDetail;
        }

        private static InvoiceDetail InvoiceDetailMapper1(Entities.Invoice invoice, InvoiceType invoiceType)
        {



            string partnerName = null;
            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            if (partnerSettings != null)
            {
                PartnerNameManagerContext context = new PartnerNameManagerContext
                {
                    PartnerId = invoice.PartnerId
                };
                partnerName = partnerSettings.GetPartnerName(context);
            }
            UserManager userManager = new UserManager();
            VRTimeZoneManager timeZoneManager = new VRTimeZoneManager();
            InvoiceDetail invoiceDetail = new InvoiceDetail
            {
                Entity = invoice,
                PartnerName = partnerName,
                Paid = invoice.PaidDate.HasValue,
                Lock = invoice.LockDate.HasValue,
                UserName = userManager.GetUserName(invoice.UserId),
                HasNote = invoice.Note != null,
            };
            if (invoice.TimeZoneId.HasValue)
                invoiceDetail.TimeZoneName = timeZoneManager.GetVRTimeZoneName(invoice.TimeZoneId.Value);
            FillNeededDetailData(invoiceDetail, invoiceType);


            return invoiceDetail;
        }
        private static void FillNeededDetailData(InvoiceDetail invoiceDetail, InvoiceType invoiceType)
        {

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordType = dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            invoiceDetail.Items = new List<InvoiceDetailObject>();

            foreach (var item in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
            {
                foreach (var field in dataRecordType.Fields)
                {
                    if (item.Field == InvoiceField.CustomField && item.CustomFieldName == field.Name)
                    {
                        var fieldValue = invoiceDetail.Entity.Details.GetType().GetProperty(field.Name).GetValue(invoiceDetail.Entity.Details, null);
                        //Vanrise.Common.Utilities.GetPropValue(field.Name, invoiceDetail.Entity.Details);
                        //Vanrise.Common.Utilities.GetPropValueReader(field.Name).GetPropertyValue(invoiceDetail.Entity.Details);
                        string description = fieldValue != null ? field.Type.GetDescription(fieldValue) : null;
                        invoiceDetail.Items.Add(new InvoiceDetailObject
                        {
                            FieldName = field.Name,
                            Description = description,
                            Value = fieldValue
                        });
                    }

                }
            }

        }
        private static InvoiceDetail InvoiceDetailMapper2(InvoiceDetail invoiceDetail, InvoiceType invoiceType)
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
                return null;
            }
            protected override Vanrise.Entities.BigResult<InvoiceDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input, IEnumerable<Entities.Invoice> allRecords)
            {
                InvoiceType invoiceType = new InvoiceTypeManager().GetInvoiceType(input.Query.InvoiceTypeId);

                return allRecords.ToBigResult(input, null, (entity) => InvoiceManager.InvoiceDetailMapper1(entity, invoiceType));
            }
            public override IEnumerable<Entities.Invoice> RetrieveAllData(DataRetrievalInput<InvoiceQuery> input)
            {
                IInvoiceDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
                return _dataManager.GetFilteredInvoices(input);
            }
        }


        #endregion

        #region Private Methods
        private Entities.Invoice BuildInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, int? timeZoneId, string timeZoneOffset, DateTime issueDate, dynamic invoiceDetails, int duePeriod)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                UserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId(),
                Details = invoiceDetails,
                InvoiceTypeId = invoiceType.InvoiceTypeId,
                FromDate = fromDate,
                PartnerId = partnerId,
                ToDate = toDate,
                IssueDate = issueDate,
                TimeZoneId = timeZoneId,
                TimeZoneOffset = timeZoneOffset
            };

            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            invoice.DueDate = issueDate.AddDays(duePeriod);
            return invoice;
        }
        private GeneratedInvoice BuildGeneratedInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic customSectionPayload, long? invoiceId, int duePeriod, out IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions)
        {
            if (CheckInvoiceOverlaping(invoiceType.InvoiceTypeId, partnerId, fromDate, toDate, invoiceId))
                throw new InvoiceGeneratorException("Invoices must not overlap.");

            var context = new InvoiceGenerationContext
            {
                CustomSectionPayload = customSectionPayload,
                FromDate = fromDate,
                PartnerId = partnerId,
                IssueDate = issueDate,
                ToDate = toDate,
                GeneratedToDate = toDate,
                InvoiceTypeId = invoiceType.InvoiceTypeId,
                DuePeriod = duePeriod
            };

            InvoiceGenerator generator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
            generator.GenerateInvoice(context);

            billingTransactions = context.BillingTransactions;
            return context.Invoice;
        }


        private static bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId();
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

        private bool SaveInvoice(List<GeneratedInvoiceItemSet> invoiceItemSets, Entities.Invoice invoice, long? invoiceIdToDelete, IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions, out long invoiceId)
        {
            IEnumerable<string> itemSetNames = invoiceItemSets.MapRecords(x => x.SetName);
            Dictionary<string, List<string>> itemSetNameStorageDic = new InvoiceItemManager().GetItemSetNamesByStorageConnectionString(invoice.InvoiceTypeId, itemSetNames);

            var dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();

            IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> mappedTransactions = null;
            if (billingTransactions != null)
                mappedTransactions = MapGeneratedInvoiceBillingTransactions(billingTransactions, invoice.SerialNumber, invoice.FromDate, invoice.ToDate, invoice.IssueDate);

            return dataManager.SaveInvoices(invoiceItemSets, invoice, invoiceIdToDelete, itemSetNameStorageDic, mappedTransactions, out invoiceId);
        }

        private IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> MapGeneratedInvoiceBillingTransactions(IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions, string serialNumber, DateTime fromDate, DateTime toDate, DateTime issueDate)
        {
            string transactionNotes = string.Format("Billing Period: {0} - {1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
            DateTime usageOverrideToDate = toDate.Date.AddDays(1);

            return billingTransactions.MapRecords(x =>
            {
                var billingTransaction = new AccountBalance.Entities.BillingTransaction()
                {
                    AccountTypeId = x.AccountTypeId,
                    AccountId = x.AccountId,
                    TransactionTypeId = x.TransactionTypeId,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    TransactionTime = issueDate,
                    Reference = serialNumber,
                    Notes = transactionNotes
                };

                if (x.Settings != null)
                {
                    billingTransaction.Settings = new AccountBalance.Entities.BillingTransactionSettings();

                    if (x.Settings.UsageOverrides != null)
                    {
                        billingTransaction.Settings.UsageOverrides = new List<AccountBalance.Entities.BillingTransactionUsageOverride>();

                        foreach (GeneratedInvoiceBillingTransactionUsageOverride usageOverride in x.Settings.UsageOverrides)
                        {
                            billingTransaction.Settings.UsageOverrides.Add(new AccountBalance.Entities.BillingTransactionUsageOverride()
                            {
                                TransactionTypeId = usageOverride.TransactionTypeId,
                                FromDate = fromDate,
                                ToDate = usageOverrideToDate
                            });
                        }
                    }
                }

                return billingTransaction;
            });
        }

        #endregion
    }
}
