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
using Vanrise.GenericData.MainExtensions.DataRecordFields;
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
        PartnerManager _partnerManager = new PartnerManager();


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
            var result = BigDataManager.Instance.RetrieveData(input, new InvoiceRequestHandler());
            if (input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
               var  bigResult = result as Vanrise.Entities.BigResult<InvoiceDetail>;
               if (!getClientInvoices && bigResult.Data != null)
                {
                    foreach (var accountDetail in bigResult.Data)
                    {
                        InvoiceDetailMapper2(accountDetail, invoiceType);
                    }
                }
               return bigResult;
            }
            return result;
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
                    var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                    var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                    IEnumerable<GeneratedInvoiceBillingTransaction> billingTarnsactions;
                    GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod,invoiceAccountData, out billingTarnsactions);

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

                var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);

                IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions;
                GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, fromDate, toDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod,invoiceAccountData, out billingTransactions);


                if (generatedInvoice.InvoiceDetails == null)
                {
                    throw new InvoiceGeneratorException("No data available between the selected period.");
                }


                var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.TimeZoneId, offset, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod);

                var serialNumber = _partnerManager.GetPartnerSerialNumberPattern(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId); InvoiceSerialNumberConcatenatedPartContext serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext
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
            return  _partnerManager.CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerId);
        }
        public BillingInterval GetBillingInterval(Guid invoiceTypeId, string partnerId, DateTime issueDate)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            var billingperiod = _partnerManager.GetPartnerBillingPeriod(invoiceTypeId, partnerId);
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
            }
            var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceTypeId, partnerId);
            invoiceAccountData.ThrowIfNull("invoiceAccountData");
            if (invoiceAccountData.BED.HasValue && billingInterval.FromDate < invoiceAccountData.BED.Value)
            {
                billingInterval.FromDate = invoiceAccountData.BED.Value;
            }
            if ((invoiceAccountData.EED.HasValue && billingInterval.ToDate > invoiceAccountData.EED.Value))
            {
                billingInterval.ToDate = invoiceAccountData.EED.Value;
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


        public VRPopulatedPeriod GetInvoicesPopulatedPeriod(Guid invoiceTypeId, string partnerId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);

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
                            Description = item.UseDescription ? description : fieldValue != null ? fieldValue.ToString() : null,
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
            protected override ResultProcessingHandler<InvoiceDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceQuery> input, BigResult<InvoiceDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceDetail>
                {
                    ExportExcelHandler = new InvoiceExcelExportHandler(input.Query)
                };
            }
        }
        private class InvoiceExcelExportHandler : ExcelExportHandler<InvoiceDetail>
        {
            InvoiceQuery _query;
            public InvoiceExcelExportHandler(InvoiceQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvoiceDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Invoices",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(_query.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType",_query.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                invoiceType.Settings.InvoiceGridSettings.ThrowIfNull("invoiceType.Settings.InvoiceGridSettings");
                invoiceType.Settings.InvoiceGridSettings.MainGridColumns.ThrowIfNull("invoiceType.Settings.InvoiceGridSettings.MainGridColumns");
                var dataRecordType = new DataRecordTypeManager().GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
                sheet.Header.Cells.Add(new ExportExcelHeaderCell {
                Title = "Invoice ID"
                });

                foreach (var gridColumn in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
                {
                    ExportExcelHeaderCell cell = new ExportExcelHeaderCell
                    {
                        Title = gridColumn.Header,
                    };

                    switch (gridColumn.Field)
                    {
                        case InvoiceField.CreatedTime:
                        case InvoiceField.FromDate:
                        case InvoiceField.ToDate:
                            cell.CellType = ExcelCellType.DateTime;
                            cell.DateTimeType = DateTimeType.DateTime;
                            break;
                        case InvoiceField.DueDate:
                        case InvoiceField.IssueDate:
                            cell.CellType = ExcelCellType.DateTime;
                            cell.DateTimeType = DateTimeType.Date;
                            break;
                        case InvoiceField.CustomField:
                            foreach (var field in dataRecordType.Fields)
                            {
                                if (gridColumn.Field == InvoiceField.CustomField && gridColumn.CustomFieldName == field.Name)
                                {

                                    var dateTimeFieldType = field.Type as FieldDateTimeType;
                                    if (dateTimeFieldType != null)
                                    {
                                        cell.CellType = ExcelCellType.DateTime;
                                        cell.DateTimeType = DateTimeType.Date;
                                    }
                                }
                            }
                            break;
                    }
                    sheet.Header.Cells.Add(cell);

                }

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<InvoiceDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach(var item in results.Data)
                        {


                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.InvoiceId });
                            foreach (var gridColumn in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
                            {
                                dynamic value = null;
                                switch (gridColumn.Field)
                                {
                                    case InvoiceField.CreatedTime: value = item.Entity.CreatedTime;
                                        break;

                                    case InvoiceField.DueDate: value = item.Entity.DueDate;
                                        break;
                                    case InvoiceField.FromDate: value = item.Entity.FromDate;
                                        break;
                                    case InvoiceField.InvoiceId: value = item.Entity.InvoiceId;
                                        break;
                                    case InvoiceField.IssueDate: value = item.Entity.IssueDate;
                                        break;
                                    case InvoiceField.Lock: value = item.Lock;
                                        break;
                                    case InvoiceField.Note: value = item.Entity.Note;
                                        break;
                                    case InvoiceField.Paid: value = item.Paid;
                                        break;
                                    case InvoiceField.Partner: value = item.PartnerName;
                                        break;
                                    case InvoiceField.SerialNumber: value = item.Entity.SerialNumber;
                                        break;
                                    case InvoiceField.TimeZone: value = item.TimeZoneName;
                                        break;
                                    case InvoiceField.TimeZoneOffset: value = item.Entity.TimeZoneOffset;
                                        break;
                                    case InvoiceField.ToDate: value = item.Entity.ToDate;
                                        break;
                                    case InvoiceField.UserId: value = item.UserName;
                                        break;
                                    case InvoiceField.CustomField:
                                        foreach (var field in dataRecordType.Fields)
                                        {
                                            if (gridColumn.Field == InvoiceField.CustomField && gridColumn.CustomFieldName == field.Name)
                                            {
                                                var fieldValue = Utilities.GetPropValueReader(gridColumn.CustomFieldName).GetPropertyValue(item.Entity.Details);
                                                string description = fieldValue != null ? field.Type.GetDescription(fieldValue) : null;
                                                value = gridColumn.UseDescription ? description : fieldValue;
                                                 
                                            }
                                        }
                                        break;

                                }
                                row.Cells.Add(new ExportExcelCell { Value = value });
                            
                            }
                             sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
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
        private GeneratedInvoice BuildGeneratedInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic customSectionPayload, long? invoiceId, int duePeriod,VRInvoiceAccountData invoiceAccountData, out IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions)
        {
            invoiceAccountData.ThrowIfNull("invoiceAccountData");
            if ((invoiceAccountData.BED.HasValue && fromDate < invoiceAccountData.BED.Value) || (invoiceAccountData.EED.HasValue && toDate > invoiceAccountData.EED.Value))
                throw new InvoiceGeneratorException("From date and To date should be within the effective date of invoice account.");

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
