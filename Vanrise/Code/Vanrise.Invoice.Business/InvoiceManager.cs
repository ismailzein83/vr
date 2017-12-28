using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Invoice.BP.Arguments;
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
                var bigResult = result as Vanrise.Entities.BigResult<InvoiceDetail>;
                if (!getClientInvoices && bigResult.Data != null)
                {

                    var partnerIds = bigResult.Data.Select(x => x.Entity.PartnerId);

                    var invoiceAccounts = new InvoiceAccountManager().GetInvoiceAccountsByPartnerIds(input.Query.InvoiceTypeId, partnerIds);
                    invoiceAccounts.ThrowIfNull("invoiceAccounts");
                    foreach (var invoice in bigResult.Data)
                    {
                        var invoiceAccount = invoiceAccounts.FindRecord(x => x.PartnerId == invoice.Entity.PartnerId && x.InvoiceTypeId == invoice.Entity.InvoiceTypeId);
                        invoiceAccount.ThrowIfNull("invoiceAccount");
                        InvoiceDetailMapper2(invoice, invoiceType, invoiceAccount);
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
                string datesValidationmessage;
                if (!AreInvoiceDatesValid(createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, out datesValidationmessage))
                    throw new InvoiceGeneratorException(datesValidationmessage);

                var currentInvocie = GetInvoice(createInvoiceInput.InvoiceId.Value);
                if (!currentInvocie.LockDate.HasValue)
                {
                    InvoiceTypeManager manager = new InvoiceTypeManager();
                    var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);

                    var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                    var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                    IEnumerable<GeneratedInvoiceBillingTransaction> billingTarnsactions;
                    GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod, invoiceAccountData, out billingTarnsactions);

                    Entities.Invoice invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod, createInvoiceInput.IsAutomatic);
                    invoice.SerialNumber = currentInvocie.SerialNumber;
                    invoice.Note = currentInvocie.Note;

                    if (SaveInvoice(generatedInvoice.InvoiceItemSets, invoice, createInvoiceInput.InvoiceId, billingTarnsactions, out insertedInvoiceId))
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        var invoiceAccounts = new InvoiceAccountManager().GetInvoiceAccountsByPartnerIds(invoice.InvoiceTypeId, new List<string> { invoice.PartnerId });
                        invoiceAccounts.ThrowIfNull("invoiceAccounts");
                        var invoiceAccount = invoiceAccounts.FirstOrDefault();

                        var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId), invoiceType, invoiceAccount, false);
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

        bool AreInvoiceDatesValid(DateTime from, DateTime to, DateTime issueDate, out string message)
        {
            message = null;

            StringBuilder strBuilder = new StringBuilder();
            if (issueDate.Date != issueDate)
                strBuilder.AppendFormat("Invalid Issue Date: {0}. ", issueDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (from.Date != from)
                strBuilder.AppendFormat("Invalid From Date: {0}. ", from.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (to < new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, 995))
                strBuilder.AppendFormat("Invalid To Date: {0}. ", to.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (strBuilder.Length > 0)
            {
                message = strBuilder.ToString();
                return false;
            }
            return true;
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
                string datesValidationmessage;
                if (!AreInvoiceDatesValid(createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, out datesValidationmessage))
                    throw new InvoiceGeneratorException(datesValidationmessage);

                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);
                DateTime fromDate = createInvoiceInput.FromDate;
                DateTime toDate = createInvoiceInput.ToDate;


                var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                if (invoiceAccountData.Status == VRAccountStatus.InActive)
                    throw new InvoiceGeneratorException("Cannot generate invoice for inactive account.");

                IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions;
                GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, fromDate, toDate, createInvoiceInput.IssueDate, createInvoiceInput.CustomSectionPayload, createInvoiceInput.InvoiceId, duePeriod, invoiceAccountData, out billingTransactions);


                if (generatedInvoice == null || generatedInvoice.InvoiceDetails == null)
                {
                    throw new InvoiceGeneratorException("No data available between the selected period.");
                }


                var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, createInvoiceInput.FromDate, createInvoiceInput.ToDate, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod, createInvoiceInput.IsAutomatic);

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
                    long invoiceAccountId;
                    InvoiceAccountManager invoiceAccountManager = new InvoiceAccountManager();
                    VRInvoiceAccount vrInvoiceAccount = new Entities.VRInvoiceAccount
                    {
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                        Status = invoiceAccountData.Status,
                        IsDeleted = false,
                        PartnerId = createInvoiceInput.PartnerId,
                        BED = invoiceAccountData.BED,
                        EED = invoiceAccountData.EED
                    };
                    invoiceAccountManager.TryAddInvoiceAccount(vrInvoiceAccount, out invoiceAccountId);
                    vrInvoiceAccount.InvoiceAccountId = invoiceAccountId;
                    var invoiceDetail = InvoiceDetailMapper(GetInvoice(insertedInvoiceId), invoiceType, vrInvoiceAccount, false);
                    insertOperationOutput.InsertedObject = invoiceDetail;
                    insertOperationOutput.Message = "Invoice Generated Successfully.";
                    insertOperationOutput.ShowExactMessage = true;

                    BillingPeriodInfoManager billingPeriodInfoManager = new BillingPeriodInfoManager();
                    var todate = createInvoiceInput.ToDate.AddDays(1);
                    var nextPeriodStart = new DateTime(todate.Year, todate.Month, todate.Day, 0, 0, 0);
                    BillingPeriodInfo billingPeriodInfo = new BillingPeriodInfo
                    {
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                        PartnerId = createInvoiceInput.PartnerId,
                        NextPeriodStart = nextPeriodStart
                    };
                    billingPeriodInfoManager.InsertOrUpdateBillingPeriodInfo(billingPeriodInfo);

                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            catch (InvoiceGeneratorException invoiceGeneratorException)
            {
                insertOperationOutput.Message = invoiceGeneratorException.Message;
                insertOperationOutput.ShowExactMessage = true;
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
                var invoiceAccounts = new InvoiceAccountManager().GetInvoiceAccountsByPartnerIds(invoice.InvoiceTypeId, new List<string> { invoice.PartnerId });
                invoiceAccounts.ThrowIfNull("invoiceAccounts");
                var invoiceAccount = invoiceAccounts.FirstOrDefault();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = InvoiceDetailMapper(invoice, invoiceType, invoiceAccount, false);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public bool TryUpdateInvoice(Invoice.Entities.Invoice invoice)
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
        public bool SetInvoiceSentDate(long invoiceId, bool isInvoiceSent)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            DateTime? sentDate = null;
            if (isInvoiceSent)
                sentDate = DateTime.Now;
            return dataManager.SetInvoiceSentDate(invoiceId, sentDate);
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
            var invoiceAccounts = new InvoiceAccountManager().GetInvoiceAccountsByPartnerIds(invoice.InvoiceTypeId, new List<string> { invoice.PartnerId });
            invoiceAccounts.ThrowIfNull("invoiceAccounts");
            var invoiceAccount = invoiceAccounts.FirstOrDefault();
            return InvoiceDetailMapper(invoice, invoiceType, invoiceAccount, false);
        }

        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            return _partnerManager.CheckInvoiceFollowBillingPeriod(invoiceTypeId, partnerId);
        }
        public BillingInterval GetBillingInterval(Guid invoiceTypeId, string partnerId, DateTime issueDate)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            var billingperiod = _partnerManager.GetPartnerBillingPeriod(invoiceTypeId, partnerId);
            if (billingperiod == null)
                return null;
            BillingInterval billingInterval = new Entities.BillingInterval();


            BillingPeriodContext billingPeriodContext = new Context.BillingPeriodContext
            {
                IssueDate = issueDate,
            };
            var billingPeriodInfo = new BillingPeriodInfoManager().GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            if (billingPeriodInfo != null)
            {
                billingPeriodContext.PreviousPeriodEndDate = billingPeriodInfo.NextPeriodStart;
            }
            billingInterval = billingperiod.GetPeriod(billingPeriodContext);
            billingInterval.ToDate = new DateTime(billingInterval.ToDate.Year, billingInterval.ToDate.Month, billingInterval.ToDate.Day, 23, 59, 59, 997);
            var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceTypeId, partnerId);
            invoiceAccountData.ThrowIfNull("invoiceAccountData");

            if (Utilities.AreTimePeriodsOverlapped(billingInterval.FromDate, billingInterval.ToDate, invoiceAccountData.BED, invoiceAccountData.EED))
            {
                if (invoiceAccountData.BED.HasValue && billingInterval.FromDate < invoiceAccountData.BED.Value)
                {
                    billingInterval.FromDate = invoiceAccountData.BED.Value;
                }
                if ((invoiceAccountData.EED.HasValue && billingInterval.ToDate > invoiceAccountData.EED.Value))
                {
                    var toDate = invoiceAccountData.EED.Value.AddDays(-1);
                    billingInterval.ToDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59, 997);
                }
            }
            else
            {
                return null;
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

        public IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<PartnerInvoiceType> partnerInvoiceTypes)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetLastInvoicesByPartners(partnerInvoiceTypes);
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
            return InvoiceDetailMapper1(invoice, invoiceType, false);
        }
        public DateTime? GetLastInvoiceToDate(Guid invoiceTypeId, string partnerId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            var invoice = dataManager.GetLastInvoice(invoiceTypeId, partnerId);
            if (invoice == null)
                return null;
            return invoice.ToDate;
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

        public bool CheckPartnerIfHasInvoices(Guid invoiceTypeId, string partnerId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.CheckPartnerIfHasInvoices(invoiceTypeId, partnerId);
        }

        public List<Entities.Invoice> GetInvoicesBySerialNumbers(Guid invoiceTypeId, IEnumerable<string> serialNumbers)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoicesBySerialNumbers(invoiceTypeId, serialNumbers);
        }
        public bool UpdateInvoicePaidDateById(Guid invoiceTypeId, long invoiceId, DateTime paidDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.UpdateInvoicePaidDateById(invoiceTypeId, invoiceId, paidDate);
        }

        public IEnumerable<PartnerGroupConfig> GetPartnerGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PartnerGroupConfig>(PartnerGroupConfig.EXTENSION_TYPE);
        }

        public bool DeleteGeneratedInvoice(long invoiceId)
        {
            var invoice = GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.DeleteGeneratedInvoice(invoice.InvoiceId, invoice.InvoiceTypeId, invoice.PartnerId, invoice.FromDate,invoice.ToDate);
        }

      
        #endregion

        #region Mappers
        private static InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice, InvoiceType invoiceType, VRInvoiceAccount invoiceAccount, bool includeAllFields)
        {

            var invoiceDetail = InvoiceDetailMapper1(invoice, invoiceType, includeAllFields);
            InvoiceDetailMapper2(invoiceDetail, invoiceType, invoiceAccount);
            return invoiceDetail;
        }

        private static InvoiceDetail InvoiceDetailMapper1(Entities.Invoice invoice, InvoiceType invoiceType, bool includeAllFields)
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
                IsSent = invoice.SentDate.HasValue
            };

            FillNeededDetailData(invoiceDetail, invoiceType, includeAllFields);


            return invoiceDetail;
        }
        private static void FillNeededDetailData(InvoiceDetail invoiceDetail, InvoiceType invoiceType, bool includeAllFields)
        {

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            invoiceDetail.Items = new List<InvoiceDetailObject>();
            if (includeAllFields)
            {
                foreach (var field in dataRecordTypeFields)
                {
                    invoiceDetail.Items.Add(GetInvoiceDetailObject(invoiceDetail, field.Value, true));
                }
            }
            else
            {
                foreach (var item in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
                {
                    foreach (var field in dataRecordTypeFields)
                    {
                        if (item.Field == InvoiceField.CustomField && item.CustomFieldName == field.Value.Name)
                        {
                            invoiceDetail.Items.Add(GetInvoiceDetailObject(invoiceDetail, field.Value, item.UseDescription));
                        }
                    }
                }
            }
        }
        private static InvoiceDetailObject GetInvoiceDetailObject(InvoiceDetail invoiceDetail, DataRecordField dataRecordField, bool useDescription)
        {
            var fieldValue = invoiceDetail.Entity.Details.GetType().GetProperty(dataRecordField.Name).GetValue(invoiceDetail.Entity.Details, null);
            string description = fieldValue != null ? dataRecordField.Type.GetDescription(fieldValue) : null;
            return new InvoiceDetailObject
            {
                FieldName = dataRecordField.Name,
                Description = useDescription ? description : fieldValue != null ? fieldValue.ToString() : null,
                Value = fieldValue
            };
        }
        private static InvoiceDetail InvoiceDetailMapper2(InvoiceDetail invoiceDetail, InvoiceType invoiceType, VRInvoiceAccount invoiceAccount)
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
            InvoiceGridActionFilterConditionContext invoiceFilterConditionContext = new InvoiceGridActionFilterConditionContext { Invoice = invoiceDetail.Entity, InvoiceType = invoiceType, InvoiceAccount = invoiceAccount };
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

                return allRecords.ToBigResult(input, null, (entity) => InvoiceManager.InvoiceDetailMapper1(entity, invoiceType, input.Query.IncludeAllFields));
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
                invoiceType.ThrowIfNull("invoiceType", _query.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                invoiceType.Settings.InvoiceGridSettings.ThrowIfNull("invoiceType.Settings.InvoiceGridSettings");
                invoiceType.Settings.InvoiceGridSettings.MainGridColumns.ThrowIfNull("invoiceType.Settings.InvoiceGridSettings.MainGridColumns");
                var dataRecordType = new DataRecordTypeManager().GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
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
                        foreach (var item in results.Data)
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
                                    case InvoiceField.IsAutomatic: value = item.Entity.IsAutomatic;
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

                                    case InvoiceField.ToDate: value = item.Entity.ToDate;
                                        break;
                                    case InvoiceField.UserId: value = item.UserName;
                                        break;
                                    case InvoiceField.IsSent: value = item.IsSent;
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
        private bool ActionBeforeGenerateInvoice(Entities.Invoice invoice)
        {
            var invoiceSettings = invoice.Settings;
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
            if (invoiceType.Settings != null && invoiceType.Settings.InvoiceFileSettings != null && invoiceType.Settings.InvoiceFileSettings.FilesAttachments != null)
            {
                VRFileManager fileManager = new VRFileManager();
                foreach (var fileAttachment in invoiceType.Settings.InvoiceFileSettings.FilesAttachments)
                {

                    var attachment = invoiceTypeManager.GetInvoiceAttachment(invoiceType, fileAttachment.AttachmentId);
                    attachment.ThrowIfNull("attachment", fileAttachment.AttachmentId);
                    InvoiceRDLCFileConverterContext context = new InvoiceRDLCFileConverterContext
                    {
                        InvoiceId = invoice.InvoiceId
                    };
                    var invoiceFile = attachment.InvoiceFileConverter.ConvertToInvoiceFile(context);
                    var fileId = fileManager.AddFile(new VRFile
                    {
                        Content = invoiceFile.Content,
                        Name = string.Format("{0}.{1}", invoiceFile.Name, invoiceFile.ExtensionType),
                        Extension = invoiceFile.ExtensionType,
                        IsTemp = false,
                        Settings = new VRFileSettings
                        {
                            ExtendedSettings = new InvoiceFileExtendedSetting { InvoiceId = invoice.InvoiceId, InvoiceTypeId = invoiceType.InvoiceTypeId }
                        }
                    });
                    IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
                    if (invoiceSettings == null)
                        invoiceSettings = new InvoiceSettings();
                    invoiceSettings.FileId = fileId;
                    dataManager.UpdateInvoiceSettings(invoice.InvoiceId, invoiceSettings);
                    return true;
                }
            }
            return false;
        }
        private Entities.Invoice BuildInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic invoiceDetails, int duePeriod, bool isAutomatic)
        {
            var invoiceSetting = _partnerManager.GetInvoicePartnerSetting(invoiceType.InvoiceTypeId, partnerId);
            invoiceSetting.ThrowIfNull("invoiceSetting");
            Entities.Invoice invoice = new Entities.Invoice
            {
                UserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId(),
                Details = invoiceDetails,
                InvoiceTypeId = invoiceType.InvoiceTypeId,
                FromDate = fromDate,
                PartnerId = partnerId,
                ToDate = toDate,
                IssueDate = issueDate,
                IsAutomatic = isAutomatic,
                InvoiceSettingId = invoiceSetting.InvoiceSetting.InvoiceSettingId
            };
            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            invoice.DueDate = issueDate.AddDays(duePeriod);
            return invoice;
        }
        private GeneratedInvoice BuildGeneratedInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic customSectionPayload, long? invoiceId, int duePeriod, VRInvoiceAccountData invoiceAccountData, out IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions)
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

            return dataManager.SaveInvoices(invoiceItemSets, invoice, invoiceIdToDelete, itemSetNameStorageDic, mappedTransactions, ActionBeforeGenerateInvoice, out invoiceId);
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
                                FromDate = x.FromDate.HasValue ? x.FromDate.Value : fromDate,
                                ToDate = x.ToDate.HasValue ? x.ToDate.Value : usageOverrideToDate
                            });
                        }
                    }
                }

                return billingTransaction;
            });
        }

        #endregion

        public GenerateInvoicesOutput GenerateInvoices(Guid invoiceTypeId, Guid invoiceGenerationIdentifier, DateTime issueDate, List<InvoiceGenerationDraftToEdit> changedItems, InvoiceGapAction invoiceGapAction)
        {
            InvoiceGenerationDraftManager invoiceGenerationDraftManager = new InvoiceGenerationDraftManager();
            InvoiceGenerationDraftSummary invoiceGenerationDraftSummary = invoiceGenerationDraftManager.ApplyInvoiceGenerationDraftsChanges(changedItems, invoiceGenerationIdentifier);

            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            if (invoiceGenerationDraftSummary.TotalCount == 0)
                return new GenerateInvoicesOutput { Succeed = false, OutputMessage = "At least one invoice should be selected for generation process" };

            if (!invoiceGenerationDraftSummary.MinimumFrom.HasValue)
                throw new NullReferenceException("invoiceGenerationDraftSummary.MinimumFrom");

            if (!invoiceGenerationDraftSummary.MaximumTo.HasValue)
                throw new NullReferenceException("invoiceGenerationDraftSummary.MaximumTo");

            InvoiceGenerationProcessInput invoiceGenerationProcessInput = new InvoiceGenerationProcessInput()
            {
                InvoiceGenerationIdentifier = invoiceGenerationIdentifier,
                InvoiceTypeId = invoiceTypeId,
                UserId = userId,
                IssueDate = issueDate.Date,
                MinimumFrom = invoiceGenerationDraftSummary.MinimumFrom.Value,
                MaximumTo = invoiceGenerationDraftSummary.MaximumTo.Value,
                InvoiceGapAction = invoiceGapAction
            };

            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = invoiceGenerationProcessInput
            };

            var result = new BPInstanceManager().CreateNewProcess(createProcessInput);
            return new GenerateInvoicesOutput { Succeed = true, ProcessInstanceId = result.ProcessInstanceId };
        }


        public ExecuteMenualInvoiceActionsOutput ExecuteMenualInvoiceActions(ExecuteMenualInvoiceActionsInput input)
        {
            InvoiceBulkActionsDraftManager invoiceBulkActionsDraftManager = new InvoiceBulkActionsDraftManager();
            InvoiceBulkActionsDraftSummary invoiceBulkActionsDraftSummary = invoiceBulkActionsDraftManager.UpdateInvoiceBulkActionDraft(input.InvoiceBulkActionIdentifier, input.InvoiceTypeId, input.IsAllInvoicesSelected, input.TargetInvoicesIds);

            invoiceBulkActionsDraftSummary.ThrowIfNull("invoiceBulkActionsDraftSummary");
            if (invoiceBulkActionsDraftSummary.TotalCount == 0)
                return new ExecuteMenualInvoiceActionsOutput { Succeed = false, OutputMessage = "At least one invoice should be selected for action process" };

            if (!invoiceBulkActionsDraftSummary.MinimumFrom.HasValue)
                throw new NullReferenceException("invoiceBulkActionsDraftSummary.MinimumFrom");

            if (!invoiceBulkActionsDraftSummary.MaximumTo.HasValue)
                throw new NullReferenceException("invoiceBulkActionsDraftSummary.MaximumTo");

            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            if (input.InvoiceBulkActions == null || input.InvoiceBulkActions.Count == 0)
                return new ExecuteMenualInvoiceActionsOutput { Succeed = false, OutputMessage = "At least one invoice action should be selected." };

            InvoiceBulkActionProcessInput invoiceBulkActionProcessInput = new InvoiceBulkActionProcessInput()
            {
                InvoiceBulkActionIdentifier = input.InvoiceBulkActionIdentifier,
                InvoiceTypeId = input.InvoiceTypeId,
                UserId = userId,
                InvoiceBulkActions = input.InvoiceBulkActions,
                MinimumFrom = invoiceBulkActionsDraftSummary.MinimumFrom.Value,
                MaximumTo = invoiceBulkActionsDraftSummary.MaximumTo.Value,
                HandlingErrorOption = input.HandlingErrorOption
            };

            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = invoiceBulkActionProcessInput
            };

            var result = new BPInstanceManager().CreateNewProcess(createProcessInput);
            return new ExecuteMenualInvoiceActionsOutput { Succeed = true, ProcessInstanceId = result.ProcessInstanceId };
        }

    }
}