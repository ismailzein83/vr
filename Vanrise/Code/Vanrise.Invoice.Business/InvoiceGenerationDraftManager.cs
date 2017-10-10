using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business.Context;

namespace Vanrise.Invoice.Business
{
    public class InvoiceGenerationDraftManager
    {
        public InvoiceGenerationDraftOutput GenerateFilteredInvoiceGenerationDrafts(InvoiceGenerationDraftQuery query)
        {
            DeleteInvoiceGenerationDraft(query.InvoiceTypeId);

            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(query.InvoiceTypeId);
            GenerationCustomSection generationCustomSection = invoiceType.Settings.ExtendedSettings.GenerationCustomSection;

            var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();

            PartnerGroupContext partnerGroupContext = new PartnerGroupContext() { InvoiceTypeId = query.InvoiceTypeId, Status = query.Status, EffectiveDate = query.EffectiveDate, IsEffectiveInFuture = query.IsEffectiveInFuture };

            List<string> partnerIds = query.PartnerGroup.GetPartnerIds(partnerGroupContext);
            if (partnerIds == null || partnerIds.Count == 0)
                return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "No Partners were found." };

            InvoiceManager invoiceManager = new InvoiceManager();

            foreach (string partnerId in partnerIds)
            {
                DateTime? fromDate;
                DateTime? toDate;
                switch (query.Period)
                {
                    case InvoicePartnerPeriod.FixedDates:
                        fromDate = query.FromDate;
                        toDate = query.ToDate;
                        break;

                    case InvoicePartnerPeriod.FollowBillingCycle:
                        BillingInterval billingInterval = invoiceManager.GetBillingInterval(query.InvoiceTypeId, partnerId, query.IssueDate);
                        if (billingInterval != null)
                        {
                            fromDate = billingInterval.FromDate;
                            toDate = billingInterval.ToDate;
                        }
                        else
                        {
                            fromDate = query.FromDate;
                            toDate = query.ToDate;
                        }
                        break;
                    default: throw new NotSupportedException(string.Format("InvoicePartnerPeriod '{0}' is not supported", query.Period));
                }

                if (!fromDate.HasValue || !toDate.HasValue)
                    return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "Invalid Dates" };

                PartnerNameManagerContext partnerNameManagerContext = new PartnerNameManagerContext { PartnerId = partnerId };
                var partnerName = invoiceTypePartnerManager.GetPartnerName(partnerNameManagerContext);

                dynamic generationCustomPayload = null;
                if (generationCustomSection != null)
                {
                    GetGenerationCustomPayloadContext generationCustomPayloadContext = new Entities.GetGenerationCustomPayloadContext() { PartnerId = partnerId };
                    generationCustomPayload = generationCustomSection.GetGenerationCustomPayload(generationCustomPayloadContext);
                }

                InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
                {
                    InvoiceTypeId = query.InvoiceTypeId,
                    From = fromDate.Value,
                    To = toDate.Value,
                    PartnerId = partnerId,
                    PartnerName = partnerName,
                    CustomPayload = generationCustomPayload
                };

                InsertOperationOutput<InvoiceGenerationDraft> insertedInvoiceGenerationDraft = InsertInvoiceGenerationDraft(invoiceGenerationDraft);
                if (insertedInvoiceGenerationDraft.Result != InsertOperationResult.Succeeded)
                    return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "Failed to Add Records" };
            }
            return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Succeeded };
        }

        public IDataRetrievalResult<InvoiceGenerationDraftDetail> GetFilteredInvoiceGenerationDrafts(DataRetrievalInput<InvoiceGenerationDraftQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new InvoicePartnerRequestHandler());
        }

        public InsertOperationOutput<InvoiceGenerationDraft> InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft)
        {
            InsertOperationOutput<InvoiceGenerationDraft> insertOperationOutput = new InsertOperationOutput<InvoiceGenerationDraft>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();

            long invoiceGenerationDraftId;
            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool insertActionSucc = dataManager.InsertInvoiceGenerationDraft(userId, invoiceGenerationDraft, out invoiceGenerationDraftId);

            if (insertActionSucc)
            {
                invoiceGenerationDraft.InvoiceGenerationDraftId = invoiceGenerationDraftId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = invoiceGenerationDraft;
            }
            return insertOperationOutput;
        }

        public void DeleteInvoiceGenerationDraft(Guid invoiceTypeId)
        {
            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            dataManager.DeleteInvoiceGenerationDraft(userId, invoiceTypeId);
        }

        private class InvoicePartnerRequestHandler : BigDataRequestHandler<InvoiceGenerationDraftQuery, InvoiceGenerationDraft, InvoiceGenerationDraftDetail>
        {
            public override InvoiceGenerationDraftDetail EntityDetailMapper(InvoiceGenerationDraft entity)
            {
                InvoiceGenerationDraftDetail detail = new InvoiceGenerationDraftDetail()
                {
                    CustomPayload = entity.CustomPayload,
                    From = entity.From,
                    InvoiceGenerationDraftId = entity.InvoiceGenerationDraftId,
                    PartnerId = entity.PartnerId,
                    PartnerName = entity.PartnerName,
                    To = entity.To
                };
                return detail;
            }

            public override IEnumerable<InvoiceGenerationDraft> RetrieveAllData(DataRetrievalInput<InvoiceGenerationDraftQuery> input)
            {
                var query = input.Query;
                IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
                int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
                return dataManager.GetInvoiceGenerationDrafts(userId, input.Query.InvoiceTypeId);
            }
        }
    }
}