﻿using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
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
            List<string> invalidPartnerMessages = new List<string>();

            ClearInvoiceGenerationDrafts(query.InvoiceGenerationIdentifier);

            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(query.InvoiceTypeId);
            GenerationCustomSection generationCustomSection = invoiceType.Settings.ExtendedSettings.GenerationCustomSection;

            var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();

            PartnerGroupContext partnerGroupContext = new PartnerGroupContext() { InvoiceTypeId = query.InvoiceTypeId };

            IEnumerable<string> partnerIds;
            if (query.PartnerGroup != null)
                partnerIds = query.PartnerGroup.GetPartnerIds(partnerGroupContext);
            else
                partnerIds = invoiceType.Settings.ExtendedSettings.GetPartnerIds(new ExtendedSettingsPartnerIdsContext { InvoiceTypeId = query.InvoiceTypeId, PartnerRetrievalType = Entities.PartnerRetrievalType.GetAll });

            if (partnerIds == null || partnerIds.Count() == 0)
                return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "No partners found." };

            InvoiceManager invoiceManager = new InvoiceManager();

            int count = 0;
            DateTime minimumFrom = DateTime.MaxValue;
            DateTime maximumTo = DateTime.MinValue;
            DateTime now = DateTime.Now;

            foreach (string partnerId in partnerIds)
            {
                if (!IsStatusFilterMatching(query.InvoiceTypeId, partnerId, query.EffectiveDate, query.IsEffectiveInFuture, query.Status, now))
                    continue;

                PartnerNameManagerContext partnerNameManagerContext = new PartnerNameManagerContext { PartnerId = partnerId };
                var partnerName = invoiceTypePartnerManager.GetPartnerName(partnerNameManagerContext);

                DateTime? fromDate = null;
                DateTime? toDate = null;
                switch (query.Period)
                {
                    case InvoicePartnerPeriod.FixedDates:
                        fromDate = query.FromDate.HasValue ? query.FromDate.Value.Date : default(DateTime?);
                        toDate = query.ToDate.HasValue ? new DateTime(query.ToDate.Value.Year, query.ToDate.Value.Month, query.ToDate.Value.Day, 23, 59, 59, 997) : default(DateTime?);
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
                            if (!query.IsAutomatic)
                            {
                                fromDate = query.FromDate.HasValue ? query.FromDate.Value.Date : default(DateTime?);
                                toDate = query.ToDate.HasValue ? new DateTime(query.ToDate.Value.Year, query.ToDate.Value.Month, query.ToDate.Value.Day, 23, 59, 59, 997) : default(DateTime?);
                            }
                        }
                        break;
                    default: throw new NotSupportedException(string.Format("InvoicePartnerPeriod '{0}' is not supported", query.Period));
                }

                if (!fromDate.HasValue || !toDate.HasValue)
                {
                    if (!query.IsAutomatic)
                        return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "Billing cycle is not defined for some partners. Please fill the following fields: 'From' and 'To'" };
                    else
                    {
                        invalidPartnerMessages.Add(string.Format("Billing cycle is not defined for partner '{0}'", partnerName));
                        continue;
                    }
                }

                if (!CheckIFShouldGenerateInvoice(toDate.Value, query.MaximumToDate))
                    continue;

                dynamic generationCustomPayload = null;
                if (generationCustomSection != null)
                {
                    GetGenerationCustomPayloadContext generationCustomPayloadContext = new Entities.GetGenerationCustomPayloadContext() { PartnerId = partnerId, InvoiceTypeId = query.InvoiceTypeId };
                    generationCustomPayload = generationCustomSection.GetGenerationCustomPayload(generationCustomPayloadContext);
                }

                InvoiceGenerationDraft invoiceGenerationDraft = new InvoiceGenerationDraft()
                {
                    InvoiceGenerationIdentifier = query.InvoiceGenerationIdentifier,
                    InvoiceTypeId = query.InvoiceTypeId,
                    From = fromDate.Value,
                    To = toDate.Value,
                    PartnerId = partnerId,
                    PartnerName = partnerName,
                    CustomPayload = generationCustomPayload
                };

                if (minimumFrom > invoiceGenerationDraft.From)
                    minimumFrom = invoiceGenerationDraft.From;

                if (maximumTo < invoiceGenerationDraft.To)
                    maximumTo = invoiceGenerationDraft.To;

                InsertOperationOutput<InvoiceGenerationDraft> insertedInvoiceGenerationDraft = InsertInvoiceGenerationDraft(invoiceGenerationDraft);
                if (insertedInvoiceGenerationDraft.Result != InsertOperationResult.Succeeded)
                    return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "Technical Error occured while trying to Add Records" };

                count++;
            }

            if (count == 0)
                return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Failed, Message = "No partners found." };
            else
                return new InvoiceGenerationDraftOutput() { Result = InvoiceGenerationDraftResult.Succeeded, Count = count, MinimumFrom = minimumFrom, MaximumTo = maximumTo, InvalidPartnerMessages = invalidPartnerMessages.Count > 0 ? invalidPartnerMessages : null };
        }

        private bool CheckIFShouldGenerateInvoice(DateTime toDate, DateTime? maximumToDate)
        {
            return !maximumToDate.HasValue || toDate <= maximumToDate;
        }

        private bool IsStatusFilterMatching(Guid invoiceTypeId, string accountId, DateTime? effectiveDate, bool? isEffectiveInFuture, VRAccountStatus status, DateTime currentDate)
        {
            PartnerManager partnerManager = new PartnerManager();
            VRInvoiceAccountData invoiceAccountData = partnerManager.GetInvoiceAccountData(invoiceTypeId, accountId);
            invoiceAccountData.ThrowIfNull("invoiceAccountData");

            DateTime? bed = invoiceAccountData.BED;
            DateTime? eed = invoiceAccountData.EED;

            if (invoiceAccountData.Status == status
                && (!effectiveDate.HasValue || ((!bed.HasValue || bed <= effectiveDate) && (!eed.HasValue || eed > effectiveDate)))
                && (!isEffectiveInFuture.HasValue || (isEffectiveInFuture.Value && (!eed.HasValue || eed >= currentDate)) || (!isEffectiveInFuture.Value && eed.HasValue && eed <= currentDate)))
            {
                return true;
            }

            return false;
        }

        public List<InvoiceGenerationDraft> GetInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
            return dataManager.GetInvoiceGenerationDrafts(invoiceGenerationIdentifier);
        }

        public IDataRetrievalResult<InvoiceGenerationDraftDetail> GetFilteredInvoiceGenerationDrafts(DataRetrievalInput<InvoiceGenerationDraftQuery> input)
        {
            var result = BigDataManager.Instance.RetrieveData(input, new InvoicePartnerRequestHandler());
            if (input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                var bigResult = result as Vanrise.Entities.BigResult<InvoiceGenerationDraftDetail>;

                foreach (var invoiceGenerationDraftDetail in bigResult.Data)
                {
                    InvoiceGenerationDraftDetailMapper2(invoiceGenerationDraftDetail, input.Query);
                }
                return bigResult;
            }
            return result;
        }

        private void InvoiceGenerationDraftDetailMapper2(InvoiceGenerationDraftDetail invoiceGenerationDraftDetail, InvoiceGenerationDraftQuery query)
        {
            if (query == null)
                return;
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceGeneratorActions = invoiceTypeManager.GetInvoiceGeneratorActions(new GenerateInvoiceInput
            {
                CustomSectionPayload = invoiceGenerationDraftDetail.CustomPayload,
                FromDate = invoiceGenerationDraftDetail.From,
                InvoiceId = null,
                InvoiceTypeId = query.InvoiceTypeId,
                IsAutomatic = query.IsAutomatic,
                IssueDate = query.IssueDate,
                PartnerId = invoiceGenerationDraftDetail.PartnerId,
                ToDate = invoiceGenerationDraftDetail.To
            });

            Dictionary<VRButtonType, List<InvoiceGenerationDraftActionDetail>> actionDetailsByButtonType = null;

            if (invoiceGeneratorActions != null)
            {
                actionDetailsByButtonType = new Dictionary<VRButtonType, List<InvoiceGenerationDraftActionDetail>>();
                foreach (var invoiceGeneratorAction in invoiceGeneratorActions)
                {
                    List<InvoiceGenerationDraftActionDetail> invoiceGenerationDraftActionDetails = actionDetailsByButtonType.GetOrCreateItem(invoiceGeneratorAction.ButtonType);
                    invoiceGenerationDraftActionDetails.Add(new InvoiceGenerationDraftActionDetail
                    {
                        InvoiceGeneratorAction = invoiceGeneratorAction,
                        InvoiceAction = invoiceTypeManager.GetInvoiceAction(query.InvoiceTypeId, invoiceGeneratorAction.InvoiceGeneratorActionId)
                    });
                }
            }
            invoiceGenerationDraftDetail.InvoiceGenerationDraftActionDetails = actionDetailsByButtonType;
        }

        public InsertOperationOutput<InvoiceGenerationDraft> InsertInvoiceGenerationDraft(InvoiceGenerationDraft invoiceGenerationDraft)
        {
            InsertOperationOutput<InvoiceGenerationDraft> insertOperationOutput = new InsertOperationOutput<InvoiceGenerationDraft>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();

            long invoiceGenerationDraftId;
            bool insertActionSucc = dataManager.InsertInvoiceGenerationDraft(invoiceGenerationDraft, out invoiceGenerationDraftId);

            if (insertActionSucc)
            {
                invoiceGenerationDraft.InvoiceGenerationDraftId = invoiceGenerationDraftId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = invoiceGenerationDraft;
            }
            return insertOperationOutput;
        }

        public void ClearInvoiceGenerationDrafts(Guid invoiceGenerationIdentifier)
        {
            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
            dataManager.ClearInvoiceGenerationDrafts(invoiceGenerationIdentifier);
        }


        public InvoiceGenerationDraftSummary ApplyInvoiceGenerationDraftsChanges(List<InvoiceGenerationDraftToEdit> invoiceGenerationDrafts, Guid invoiceGenerationIdentifier)
        {
            IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
            if (invoiceGenerationDrafts != null)
            {
                foreach (InvoiceGenerationDraftToEdit invoiceGenerationDraft in invoiceGenerationDrafts)
                {
                    if (!invoiceGenerationDraft.IsSelected)
                        dataManager.DeleteInvoiceGenerationDraft(invoiceGenerationDraft.InvoiceGenerationDraftId);
                    else
                    {
                        invoiceGenerationDraft.From = new DateTime(invoiceGenerationDraft.From.Year, invoiceGenerationDraft.From.Month, invoiceGenerationDraft.From.Day, 0, 0, 0);
                        invoiceGenerationDraft.To = new DateTime(invoiceGenerationDraft.To.Year, invoiceGenerationDraft.To.Month, invoiceGenerationDraft.To.Day, 23, 59, 59, 997);
                        dataManager.UpdateInvoiceGenerationDraft(invoiceGenerationDraft);
                    }
                }
            }
            return dataManager.GetInvoiceGenerationDraftsSummary(invoiceGenerationIdentifier);

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
                IInvoiceGenerationDraftDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceGenerationDraftDataManager>();
                return dataManager.GetInvoiceGenerationDrafts(input.Query.InvoiceGenerationIdentifier);
            }
        }
    }
}