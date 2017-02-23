using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Business
{
    public class InvoiceManager
    {
        public IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceAppQuery> input)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            InvoiceViewerTypeSettings invoiceViewerTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<InvoiceViewerTypeSettings>(input.Query.InvoiceViewerTypeId);

            if (invoiceViewerTypeSettings.InvoiceQueryInterceptor == null)
                throw new NullReferenceException("invoiceViewerTypeSettings.InvoiceContextHandler");

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
                    ToTime =input.Query.ToTime,
                }
            };
            query.Query.InvoiceTypeId = invoiceViewerTypeSettings.InvoiceTypeId;
            InvoiceQueryInterceptorContext context = new InvoiceQueryInterceptorContext { Query = query.Query };
            invoiceViewerTypeSettings.InvoiceQueryInterceptor.PrepareQuery(context);

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(invoiceViewerTypeSettings.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Post<DataRetrievalInput<InvoiceQuery>, BigResult<InvoiceDetail>>("/api/VR_Invoice/Invoice/GetFilteredInvoices", query);
        }
        public IEnumerable<InvoiceQueryInterceptorTemplate> GetInvoiceQueryInterceptorTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<InvoiceQueryInterceptorTemplate>(InvoiceQueryInterceptorTemplate.EXTENSION_TYPE);
        }
    }
}
