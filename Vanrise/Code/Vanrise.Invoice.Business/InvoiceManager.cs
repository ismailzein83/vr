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
            var invoices = GetInvoices(new List<long>() { invoiceId });
            return invoices.FirstOrDefault();
        }

        public List<Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoices(invoiceIds);
        }
        public IEnumerable<Entities.InvoiceDetail> GetInvoicesDetails(Guid invoiceTypeId, string partnerId, List<long> invoiceIds)
        {
            var invoices = GetInvoices(invoiceIds);
            if (invoices == null || invoices.Count == 0)
                return null;
            else
            {
                var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
                var invoiceAccounts = new InvoiceAccountManager().GetInvoiceAccountsByPartnerIds(invoiceTypeId, new List<string> { partnerId });
                invoiceAccounts.ThrowIfNull("invoiceAccounts");
                var invoiceAccount = invoiceAccounts.FirstOrDefault();
                return invoices.MapRecords(x=> InvoiceDetailMapper(x, invoiceType, invoiceAccount, true));
            }
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
        public UpdateOperationOutput<InvoiceDetail> ReGenerateInvoice(ReGenerateInvoiceInput reGenerateInvoiceInput)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;


            string datesValidationmessage;
            if (!AreInvoiceDatesValid(reGenerateInvoiceInput.FromDate, reGenerateInvoiceInput.ToDate, reGenerateInvoiceInput.IssueDate, out datesValidationmessage))
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = datesValidationmessage;
                updateOperationOutput.ShowExactMessage = true;
                return updateOperationOutput;
            }
            var currentInvocie = GetInvoice(reGenerateInvoiceInput.InvoiceId.Value);
            if (!currentInvocie.LockDate.HasValue)
            {
                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(reGenerateInvoiceInput.InvoiceTypeId);

                var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, reGenerateInvoiceInput.PartnerId);
                var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, reGenerateInvoiceInput.PartnerId);
                IEnumerable<GeneratedInvoiceBillingTransaction> billingTarnsactions;
                List<long> invoiceToSettleIds;
                string errorMessage;
                GenerateInvoiceResult generateInvoiceResult;
                bool needApproval;
                Func<Entities.Invoice, bool> actionAfterGenerateInvoice;
                GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, reGenerateInvoiceInput.PartnerId, reGenerateInvoiceInput.FromDate, reGenerateInvoiceInput.ToDate, reGenerateInvoiceInput.IssueDate, reGenerateInvoiceInput.CustomSectionPayload, reGenerateInvoiceInput.InvoiceId, duePeriod, invoiceAccountData, out billingTarnsactions, out invoiceToSettleIds, out errorMessage, out generateInvoiceResult, out needApproval,out actionAfterGenerateInvoice);

                switch (generateInvoiceResult)
                {
                    case GenerateInvoiceResult.Succeeded:
                        break;
                    case GenerateInvoiceResult.Failed:
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                        updateOperationOutput.Message = errorMessage;
                        updateOperationOutput.ShowExactMessage = true;
                        return updateOperationOutput;
                    case GenerateInvoiceResult.NoData:
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                        updateOperationOutput.Message = errorMessage != null ? errorMessage : "No data available between the selected period.";
                        updateOperationOutput.ShowExactMessage = true;
                        return updateOperationOutput;
                }

                generatedInvoice.ThrowIfNull("generatedInvoice");
                generatedInvoice.ThrowIfNull("generatedInvoice.InvoiceDetails");

                Entities.Invoice invoice = BuildInvoice(invoiceType, reGenerateInvoiceInput.PartnerId, reGenerateInvoiceInput.FromDate, reGenerateInvoiceInput.ToDate, reGenerateInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod, reGenerateInvoiceInput.IsAutomatic, needApproval);
                invoice.SerialNumber = currentInvocie.SerialNumber;
                invoice.Note = currentInvocie.Note;

                List<PreparedGenerateInvoiceInput> preparedGenerateInvoiceInputs = new List<PreparedGenerateInvoiceInput>();
                preparedGenerateInvoiceInputs.Add(new PreparedGenerateInvoiceInput
                {
                    BillingTransactions = billingTarnsactions,
                    InvoiceToSettleIds = invoiceToSettleIds,
                    InvoiceItemSets = generatedInvoice.InvoiceItemSets,
                    Invoice = invoice,
                    InvoiceIdToDelete = reGenerateInvoiceInput.InvoiceId,
                    SplitInvoiceGroupId = currentInvocie.SplitInvoiceGroupId,
                    ActionAfterGenerateInvoice = actionAfterGenerateInvoice
                });
                List<long> insertedInvoiceIds = null;
                if (SaveInvoice(preparedGenerateInvoiceInputs, out insertedInvoiceIds))
                {
                    var insertedInvoiceId = insertedInvoiceIds.First();
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
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                updateOperationOutput.Message = string.Format("Invoice {0} is Locked.", reGenerateInvoiceInput.InvoiceId);
                updateOperationOutput.ShowExactMessage = true;
            }
            return updateOperationOutput;
        }

        public List<Entities.Invoice> GetPartnerInvoicesByDate(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);

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

        private List<GenerateInvoiceOutput> GetErrorListOutput(List<GenerateInvoiceInputItem> items,  string errorMessage ,int? index)
        {
            var generateInvoiceOutputs = new List<GenerateInvoiceOutput>();

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var generateInvoiceOutput = new GenerateInvoiceOutput
                {
                    FromDate = item.FromDate,
                    IsSucceeded = false,
                    ToDate = item.ToDate,
                };
                if (index.HasValue && i != index.Value && errorMessage != null)
                {
                    generateInvoiceOutput.Message = new InvoiceGenerationMessageOutput
                    {
                        LogEntryType = LogEntryType.Warning,
                        Message = "Stopped due to an error occurred in the generation of related invoice"
                    };
                }else
                {
                    generateInvoiceOutput.Message = new InvoiceGenerationMessageOutput
                    {
                        LogEntryType = LogEntryType.Warning,
                        Message = errorMessage
                    };
                   
                }
                generateInvoiceOutputs.Add(generateInvoiceOutput);
            }
            return generateInvoiceOutputs;
        }

        public List<GenerateInvoiceOutput> GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {

            var generateInvoiceOutputs = new List<GenerateInvoiceOutput>();
            if (createInvoiceInput.Items != null)
            {
                var manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);
                var duePeriod = _partnerManager.GetPartnerDuePeriod(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceType.InvoiceTypeId, createInvoiceInput.PartnerId);
                if (invoiceAccountData.Status == VRAccountStatus.InActive)
                {
                    foreach (var item in createInvoiceInput.Items)
                    {
                        generateInvoiceOutputs.Add(new GenerateInvoiceOutput
                        {
                            FromDate = item.FromDate,
                            Message = new InvoiceGenerationMessageOutput
                            {
                                LogEntryType = LogEntryType.Warning,
                                Message = "Cannot generate invoice for inactive account",
                            },
                            IsSucceeded = false,
                            ToDate = item.ToDate,
                        });
                    }
                    return generateInvoiceOutputs;
                }

                var preparedGenerateInvoiceInputs = new List<PreparedGenerateInvoiceInput>();

                for (var i = 0; i < createInvoiceInput.Items.Count; i++)
                {

                    var item = createInvoiceInput.Items[i];
                    string datesValidationmessage;
                    if (!AreInvoiceDatesValid(item.FromDate, item.ToDate, createInvoiceInput.IssueDate, out datesValidationmessage))
                    {
                        return GetErrorListOutput(createInvoiceInput.Items, datesValidationmessage, i);
                    }
                    DateTime fromDate = item.FromDate;
                    DateTime toDate = item.ToDate;

                    IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions;

                    List<long> invoiceToSettleIds;
                    string errorMessage;
                    GenerateInvoiceResult generateInvoiceResult;
                    bool needApproval;
                    Func<Entities.Invoice, bool> actionAfterGenerateInvoice;
                    GeneratedInvoice generatedInvoice = BuildGeneratedInvoice(invoiceType, createInvoiceInput.PartnerId, fromDate, toDate, createInvoiceInput.IssueDate, item.CustomSectionPayload, item.InvoiceId, duePeriod, invoiceAccountData, out billingTransactions, out invoiceToSettleIds, out errorMessage, out generateInvoiceResult, out needApproval, out actionAfterGenerateInvoice);

                    switch (generateInvoiceResult)
                    {
                        case GenerateInvoiceResult.Succeeded:
                            break;
                        case GenerateInvoiceResult.Failed:
                            return GetErrorListOutput(createInvoiceInput.Items, errorMessage, i);
                        case GenerateInvoiceResult.NoData:
                            generateInvoiceOutputs.Add(new GenerateInvoiceOutput
                            {
                                FromDate = item.FromDate,
                                Message = new InvoiceGenerationMessageOutput
                                {
                                    LogEntryType = LogEntryType.Warning,
                                    Message = errorMessage != null ? errorMessage : "No data available between the selected period."
                                },
                                IsSucceeded = false,
                                ToDate = item.ToDate,
                            });
                            continue;
                    }

                    generatedInvoice.ThrowIfNull("generatedInvoice");
                    generatedInvoice.ThrowIfNull("generatedInvoice.InvoiceDetails");

                    var invoice = BuildInvoice(invoiceType, createInvoiceInput.PartnerId, item.FromDate, item.ToDate, createInvoiceInput.IssueDate, generatedInvoice.InvoiceDetails, duePeriod, createInvoiceInput.IsAutomatic, needApproval);

                    preparedGenerateInvoiceInputs.Add(new PreparedGenerateInvoiceInput
                    {
                        Invoice = invoice,
                        BillingTransactions = billingTransactions,
                        InvoiceToSettleIds = invoiceToSettleIds,
                        InvoiceItemSets = generatedInvoice.InvoiceItemSets,
                        ActionAfterGenerateInvoice = actionAfterGenerateInvoice
                    });

                }

                foreach (var preparedGenerateInvoiceInput in preparedGenerateInvoiceInputs)
                {

                    var serialNumber = _partnerManager.GetPartnerSerialNumberPattern(createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId);
                    var serialNumberContext = new InvoiceSerialNumberConcatenatedPartContext
                    {
                        Invoice = preparedGenerateInvoiceInput.Invoice,
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId
                    };
                    foreach (var part in invoiceType.Settings.InvoiceSerialNumberSettings.SerialNumberParts)
                    {
                        if (serialNumber != null && serialNumber.Contains(string.Format("#{0}#", part.VariableName)))
                        {
                            serialNumber = serialNumber.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(serialNumberContext));
                        }
                    }
                    preparedGenerateInvoiceInput.Invoice.SerialNumber = serialNumber;
                }

                List<long> insertedInvoiceIds = null;
                if (SaveInvoice(preparedGenerateInvoiceInputs, out  insertedInvoiceIds))
                {
                    long invoiceAccountId;
                    var invoiceAccountManager = new InvoiceAccountManager();
                    var vrInvoiceAccount = new Entities.VRInvoiceAccount
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


                    foreach (var insertedInvoiceId in insertedInvoiceIds)
                    {
                        var invoice = GetInvoice(insertedInvoiceId);
                        generateInvoiceOutputs.Add(new GenerateInvoiceOutput
                        {
                            IsSucceeded = true,
                            FromDate = invoice.FromDate,
                            Invoice = invoice,
                            ToDate = invoice.ToDate,
                        });
                    }

                    var billingPeriodInfoManager = new BillingPeriodInfoManager();

                    var orderedItems = createInvoiceInput.Items.OrderBy(x => x.ToDate);

                    var todate = orderedItems.Last().ToDate.AddDays(1);
                    var nextPeriodStart = new DateTime(todate.Year, todate.Month, todate.Day, 0, 0, 0);
                    var billingPeriodInfo = new BillingPeriodInfo
                    {
                        InvoiceTypeId = createInvoiceInput.InvoiceTypeId,
                        PartnerId = createInvoiceInput.PartnerId,
                        NextPeriodStart = nextPeriodStart
                    };
                    billingPeriodInfoManager.InsertOrUpdateBillingPeriodInfo(billingPeriodInfo);

                }
            }
            return generateInvoiceOutputs;
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
            var invoice = GetInvoice(invoiceId);
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

        public List<BillingInterval> GetBillingInterval(Guid invoiceTypeId, string partnerId, DateTime issueDate)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            var billingperiod = _partnerManager.GetPartnerBillingPeriod(invoiceTypeId, partnerId);
            if (billingperiod == null)
                return null;
            List<BillingInterval> billingIntervals = null;


            BillingPeriodContext billingPeriodContext = new Context.BillingPeriodContext
            {
                IssueDate = issueDate.Date,
            };
            var billingPeriodInfo = new BillingPeriodInfoManager().GetBillingPeriodInfoById(partnerId, invoiceTypeId);
            if (billingPeriodInfo != null)
            {
                billingPeriodContext.PreviousPeriodEndDate = billingPeriodInfo.NextPeriodStart;
            }
            billingIntervals = billingperiod.GetPeriod(billingPeriodContext);



            if (billingIntervals != null && billingIntervals.Count > 0)
            {

                var invoiceAccountData = _partnerManager.GetInvoiceAccountData(invoiceTypeId, partnerId);
                invoiceAccountData.ThrowIfNull("invoiceAccountData");

                foreach (var billingInterval in billingIntervals)
                {

                    billingInterval.ToDate = new DateTime(billingInterval.ToDate.Year, billingInterval.ToDate.Month, billingInterval.ToDate.Day, 23, 59, 59, 997);

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
                }
            }

            return billingIntervals;
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
            return dataManager.DeleteGeneratedInvoice(invoice.InvoiceId, invoice.InvoiceTypeId, invoice.PartnerId, invoice.FromDate, invoice.ToDate);
        }

        public bool ApproveInvoice(long invoiceId, bool isApproved)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            DateTime? ApprovedDate = null;
            int? ApprovedBy = null;
            if (isApproved)
            {
                ApprovedDate = DateTime.Now;
                int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
                ApprovedBy = loggedInUserId;
            }
            return dataManager.ApproveInvoice(invoiceId, ApprovedDate, ApprovedBy);
        }

        #endregion

        #region Security

        public bool DoesUserHaveGenerateAccess(long invoiceId)
        {
            Guid invoiceTypeId = GetInvoiceTypeId(invoiceId);
            return new InvoiceTypeManager().DoesUserHaveGenerateAccess(invoiceTypeId);
        }

        public bool DoesUserHaveViewAccess(long invoiceId)
        {
            Guid invoiceTypeId = GetInvoiceTypeId(invoiceId);
            return new InvoiceTypeManager().DoesUserHaveViewAccess(invoiceTypeId);
        }

        public bool DosesUserHaveActionAccess(InvoiceActionType type, long invoiceId, Guid ActionTypeId)
        {
            Guid invoiceTypeId = GetInvoiceTypeId(invoiceId);
            return new InvoiceTypeManager().DosesUserHaveActionAccess(type, invoiceTypeId, ActionTypeId);
        }

        public bool DosesUserHaveActionAccess(InvoiceActionType type, int userId, long invoiceId, Guid ActionTypeId)
        {
            Guid invoiceTypeId = GetInvoiceTypeId(invoiceId);
            return new InvoiceTypeManager().DosesUserHaveActionAccess(type, userId, invoiceTypeId, ActionTypeId);
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
                ApprovedByName = invoice.ApprovedBy.HasValue? userManager.GetUserName(invoice.ApprovedBy.Value):null,
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
            InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(invoiceDetail.Entity);
            invoiceRecordObject.ThrowIfNull("invoiceRecordObject");
            invoiceRecordObject.InvoiceDataRecordObject.ThrowIfNull("invoiceRecordObject.InvoiceDataRecordObject");
            var fieldValue = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(dataRecordField.Name);
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
            var actionTypes = new InvoiceTypeManager().GetInvoiceActionsByActionId(invoiceType.InvoiceTypeId);
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
                    var invoiceAction = actionTypes.GetRecord(action.InvoiceGridActionId);
                    invoiceAction.ThrowIfNull("Invoice Action ", action.InvoiceGridActionId);
                    invoiceAction.Settings.ThrowIfNull("Invoice Action Settings");

                    var actionCheckAccessContext = new InvoiceActionSettingsCheckAccessContext
                    {
                        UserId = ContextFactory.GetContext().GetLoggedInUserId(),
                        InvoiceAction = invoiceAction
                    };
                    if (invoiceDetail.ActionTypeNames == null)
                        invoiceDetail.ActionTypeNames = new List<InvoiceGridAction>();
                    if (invoiceAction.Settings.DoesUserHaveAccess(actionCheckAccessContext))
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
                                                InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(item.Entity);
                                                invoiceRecordObject.ThrowIfNull("invoiceRecordObject");
                                                invoiceRecordObject.InvoiceDataRecordObject.ThrowIfNull("invoiceRecordObject.InvoiceDataRecordObject");
                                                var fieldValue = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(gridColumn.CustomFieldName);
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

        private Entities.Invoice BuildInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic invoiceDetails, int duePeriod, bool isAutomatic, bool needApproval)
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
                InvoiceSettingId = invoiceSetting.InvoiceSetting.InvoiceSettingId,
                NeedApproval = needApproval
            };
            var partnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            invoice.DueDate = issueDate.AddDays(duePeriod);
            return invoice;
        }

        private GeneratedInvoice BuildGeneratedInvoice(InvoiceType invoiceType, string partnerId, DateTime fromDate, DateTime toDate, DateTime issueDate, dynamic customSectionPayload, long? invoiceId, int duePeriod, VRInvoiceAccountData invoiceAccountData, out IEnumerable<GeneratedInvoiceBillingTransaction> billingTransactions, out List<long> invoiceToSettleIds, out string errorMessage, out GenerateInvoiceResult generateInvoiceResult, out bool needApproval, out Func<Entities.Invoice, bool> actionAfterGenerateInvoice)
        {
            generateInvoiceResult = GenerateInvoiceResult.Succeeded;
            errorMessage = null;
            billingTransactions = null;
            invoiceToSettleIds = null;
            needApproval = false;
            actionAfterGenerateInvoice = null;
            invoiceAccountData.ThrowIfNull("invoiceAccountData");
            if ((invoiceAccountData.BED.HasValue && fromDate < invoiceAccountData.BED.Value) || (invoiceAccountData.EED.HasValue && toDate > invoiceAccountData.EED.Value))
            {
                errorMessage = "From date and To date should be within the effective date of invoice account";
                generateInvoiceResult = GenerateInvoiceResult.Failed;
                return null;
            }

            if (CheckInvoiceOverlaping(invoiceType.InvoiceTypeId, partnerId, fromDate, toDate, invoiceId))
            {
                generateInvoiceResult = GenerateInvoiceResult.Failed;
                errorMessage = "Invoices must not overlap";
                return null;
            }

            var context = new InvoiceGenerationContext
            {
                CustomSectionPayload = customSectionPayload,
                FromDate = fromDate,
                PartnerId = partnerId,
                IssueDate = issueDate,
                ToDate = toDate,
                InvoiceTypeId = invoiceType.InvoiceTypeId,
                DuePeriod = duePeriod,

            };

            InvoiceGenerator generator = invoiceType.Settings.ExtendedSettings.GetInvoiceGenerator();
            generator.GenerateInvoice(context);
            if(context.GenerateInvoiceResult != GenerateInvoiceResult.Succeeded)
            {
                errorMessage = context.ErrorMessage;
                generateInvoiceResult = context.GenerateInvoiceResult;
                return null;
            }
            actionAfterGenerateInvoice = context.ActionAfterGenerateInvoice;
            billingTransactions = context.BillingTransactions;
           
            invoiceToSettleIds = context.InvoiceToSettleIds;
            needApproval = context.NeedApproval;

            return context.Invoice;
        }

        private bool CheckInvoiceOverlaping(Guid invoiceTypeId, string partnerId, DateTime fromDate, DateTime toDate, long? invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.CheckInvoiceOverlaping(invoiceTypeId, partnerId, fromDate, toDate, invoiceId);
        }

        private bool SaveInvoice(List<PreparedGenerateInvoiceInput> preparedGenerateInvoiceInputs, out List<long> insertedInvoiceIds)
        {

            List<GenerateInvoiceInputToSave> generateInvoicesInputToSave = new List<GenerateInvoiceInputToSave>();
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            foreach (var preparedGenerateInvoiceInput in preparedGenerateInvoiceInputs)
            {
                IEnumerable<string> itemSetNames = preparedGenerateInvoiceInput.InvoiceItemSets.MapRecords(x => x.SetName);
                GenerateInvoiceInputToSave generateInvoiceInputToSave = new GenerateInvoiceInputToSave {
                    InvoiceIdToDelete = preparedGenerateInvoiceInput.InvoiceIdToDelete,
                    Invoice = preparedGenerateInvoiceInput.Invoice,
                    InvoiceToSettleIds = preparedGenerateInvoiceInput.InvoiceToSettleIds,
                    InvoiceItemSets = preparedGenerateInvoiceInput.InvoiceItemSets,
                    SplitInvoiceGroupId = preparedGenerateInvoiceInput.SplitInvoiceGroupId,
                    ActionAfterGenerateInvoice = preparedGenerateInvoiceInput.ActionAfterGenerateInvoice
                };
                generateInvoiceInputToSave.ItemSetNameStorageDic = invoiceItemManager.GetItemSetNamesByStorageConnectionString(preparedGenerateInvoiceInput.Invoice.InvoiceTypeId, itemSetNames);

                if (preparedGenerateInvoiceInput.BillingTransactions != null)
                    generateInvoiceInputToSave.MappedTransactions = MapGeneratedInvoiceBillingTransactions(preparedGenerateInvoiceInput.BillingTransactions, preparedGenerateInvoiceInput.Invoice.SerialNumber, preparedGenerateInvoiceInput.Invoice.FromDate, preparedGenerateInvoiceInput.Invoice.ToDate, preparedGenerateInvoiceInput.Invoice.IssueDate);

                generateInvoiceInputToSave.ActionBeforeGenerateInvoice = ActionBeforeGenerateInvoice;
                generateInvoicesInputToSave.Add(generateInvoiceInputToSave);
            }
            var dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();

            return dataManager.SaveInvoices(generateInvoicesInputToSave, out insertedInvoiceIds);
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