﻿using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
namespace PartnerPortal.Invoice.Business
{
    public class InvoiceManager
    {
        public IDataRetrievalResult<InvoiceAppDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceAppQuery> input)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            InvoiceViewerTypeSettings invoiceViewerTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<InvoiceViewerTypeSettings>(input.Query.InvoiceViewerTypeId);

            if (invoiceViewerTypeSettings.InvoiceQueryInterceptor == null)
                throw new NullReferenceException("invoiceViewerTypeSettings.InvoiceContextHandler");
            IEnumerable<string> partnerIds = null;

            InvoiceViewerTypeExtendedSettingsContext invoiceViewerTypeExtendedSettingsContext = new InvoiceViewerTypeExtendedSettingsContext
            {
                InvoiceViewerTypeSettings = invoiceViewerTypeSettings,
                UserId = SecurityContext.Current.GetLoggedInUserId()
            };
            var invoiceAccounts = invoiceViewerTypeSettings.ExtendedSettings.GetInvoiceAccounts(invoiceViewerTypeExtendedSettingsContext);
            if (invoiceAccounts != null && invoiceAccounts.Count() > 0)
            {
                if (input.Query.PartnerIds != null && input.Query.PartnerIds.Count > 0)
                {
                    if (!input.Query.PartnerIds.All(x=> invoiceAccounts.Any(y=>y.PortalInvoiceAccountId == x)))
                    {
                        throw new Exception("Account not valid.");
                    }
                    partnerIds = input.Query.PartnerIds;
                }else
                {
                    partnerIds = invoiceAccounts.MapRecords(x => x.PortalInvoiceAccountId);
                }
            }
            
            BigResult<InvoiceAppDetail> finalResult = new BigResult<InvoiceAppDetail>();

            if (partnerIds != null)
            {
                DataRetrievalInput<InvoiceQuery> query = new DataRetrievalInput<InvoiceQuery>
                {
                    DataRetrievalResultType = input.DataRetrievalResultType,
                    FromRow = input.FromRow,
                    SortByColumnName = input.SortByColumnName,
                    GetSummary = input.GetSummary,
                    IsSortDescending = input.IsSortDescending,
                    ResultKey = input.ResultKey,
                    ToRow = input.ToRow,
                    Query = new InvoiceQuery
                    {
                        FromTime = input.Query.FromTime,
                        ToTime = input.Query.ToTime,
                        PartnerIds = partnerIds.ToList()
                    }
                };
                query.Query.InvoiceTypeId = invoiceViewerTypeSettings.InvoiceTypeId;
                InvoiceQueryInterceptorContext context = new InvoiceQueryInterceptorContext { Query = query.Query };
                invoiceViewerTypeSettings.InvoiceQueryInterceptor.PrepareQuery(context);

                VRConnectionManager connectionManager = new VRConnectionManager();
                var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(invoiceViewerTypeSettings.VRConnectionId);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                var bigResult = connectionSettings.Post<DataRetrievalInput<InvoiceQuery>, BigResult<InvoiceClientDetail>>("/api/VR_Invoice/Invoice/GetFilteredClientInvoices", query);
                finalResult.ResultKey = bigResult.ResultKey;
                finalResult.TotalCount = bigResult.TotalCount;
                if (bigResult != null && bigResult.Data != null)
                {
                    List<InvoiceAppDetail> result = new List<InvoiceAppDetail>();
                    foreach (var invoiceItem in bigResult.Data)
                    {
                        var invoiceAppDetail = ConvertInvoiceClientDetailToInoviceAppDetail(invoiceItem);
                        FillClientInvoiceDataNeeded(invoiceAppDetail, invoiceViewerTypeSettings.GridSettings.InvoiceGridActions);
                        result.Add(invoiceAppDetail);
                    }
                    finalResult.Data = result;
                }
            }
            return finalResult;
        }
        private InvoiceAppDetail ConvertInvoiceClientDetailToInoviceAppDetail(InvoiceClientDetail invoiceClientDetail)
        {
            InvoiceAppDetail invoiceAppDetail = null;
            if(invoiceClientDetail != null)
            {
                invoiceAppDetail = new InvoiceAppDetail
                {
                    Entity = invoiceClientDetail.Entity,
                    HasNote = invoiceClientDetail.HasNote,
                    Lock = invoiceClientDetail.Lock,
                    Paid = invoiceClientDetail.Paid,
                    PartnerName = invoiceClientDetail.PartnerName,
                    UserName = invoiceClientDetail.UserName,
                };
                if (invoiceClientDetail.Items != null && invoiceClientDetail.Items.Count > 0)
                {
                    invoiceAppDetail.Items = new List<Entities.InvoiceDetailObject>();
                    foreach (var item in invoiceClientDetail.Items)
                    {
                        invoiceAppDetail.Items.Add(new Entities.InvoiceDetailObject
                        {
                            Description = item.Description,
                            FieldName = item.FieldName,
                            Value = item.Value
                        });
                    }
                }
            }
            return invoiceAppDetail;
        }
        private void FillClientInvoiceDataNeeded(InvoiceAppDetail invoiceAppDetail, List<InvoiceViewerTypeGridAction> invoiceGridActions)
        {
            if (invoiceGridActions != null)
            {
                invoiceAppDetail.ActionsIds = new List<Guid>();
                foreach (var invoiceGridAction in invoiceGridActions)
                {
                    invoiceAppDetail.ActionsIds.Add(invoiceGridAction.InvoiceViewerTypeGridActionId);
                }
            }
        }
        public Vanrise.Invoice.Entities.Invoice GetRemoteInvoice(Guid connectionId, long invoiceId)
        {
            VRInterAppRestConnection connectionSettings = new InvoiceTypeManager().GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<Vanrise.Invoice.Entities.Invoice>(string.Format("/api/VR_Invoice/Invoice/GetInvoice?invoiceId={0}", invoiceId));
        }
        public InvoiceTile GetRemoteLastInvoice(Guid connectionId, Guid invoiceTypeId, Guid? viewId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
           
            string partnerId = manager.GetRetailAccountId(userId).ToString();


            VRInterAppRestConnection connectionSettings = new InvoiceTypeManager().GetVRInterAppRestConnection(connectionId);
            var invoiceDetail = connectionSettings.Get<InvoiceClientDetail>(string.Format("/api/VR_Invoice/Invoice/GetLastInvoice?invoiceTypeId={0}&partnerId={1}", invoiceTypeId, partnerId));
            InvoiceTile invoiceTile = new Entities.InvoiceTile
            {
                InvoiceDetail = invoiceDetail,
            };
            if (invoiceDetail != null && invoiceDetail.Entity != null)
                invoiceTile.FormattedDate = invoiceDetail.Entity.ToDate.ToString(new GeneralSettingsManager().GetDateFormat()); 
            if(viewId.HasValue)
            {
                ViewManager viewManager = new ViewManager();
                var view = viewManager.GetView(viewId.Value);
                invoiceTile.ViewURL = view.Url;
            }
            return invoiceTile;
        }


        public IEnumerable<PortalInvoiceAccount> GetInvoiceAccounts(Guid invoiceViewerTypeId)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            InvoiceViewerTypeSettings invoiceViewerTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<InvoiceViewerTypeSettings>(invoiceViewerTypeId);
            invoiceViewerTypeSettings.ThrowIfNull("invoiceViewerTypeSettings",invoiceViewerTypeId);
            invoiceViewerTypeSettings.ExtendedSettings.ThrowIfNull("invoiceViewerTypeSettings.ExtendedSettings");
            InvoiceViewerTypeExtendedSettingsContext context = new InvoiceViewerTypeExtendedSettingsContext{
                InvoiceViewerTypeSettings = invoiceViewerTypeSettings,
                UserId = SecurityContext.Current.GetLoggedInUserId()
            };
            return invoiceViewerTypeSettings.ExtendedSettings.GetInvoiceAccounts(context);

        }
    }
}
