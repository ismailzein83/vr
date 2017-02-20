using PartnerPortal.CustomerAccess.Business.Extensions;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class InvoiceManager
    {
        public IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceAppQuery> input)
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            InvoiceViewerTypeSettings invoiceViewerTypeSettings = vrComponentTypeManager.GetComponentTypeSettings<InvoiceViewerTypeSettings>(input.Query.InvoiceViewerTypeId);

            if (invoiceViewerTypeSettings.InvoiceContextHandler == null)
                throw new NullReferenceException("invoiceViewerTypeSettings.InvoiceContextHandler");

            InvoiceContextHandlerContext context = new InvoiceContextHandlerContext {Query = input.Query };
            invoiceViewerTypeSettings.InvoiceContextHandler.PrepareQuery(context);

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(invoiceViewerTypeSettings.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Post<DataRetrievalInput<InvoiceAppQuery>, IDataRetrievalResult<InvoiceDetail>>("/api/VR_Invoice/Invoice/GetFilteredInvoices", input);
        }
        public IEnumerable<InvoiceContextHandlerTemplate> GetInvoiceContextHandlerTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<InvoiceContextHandlerTemplate>(InvoiceContextHandlerTemplate.EXTENSION_TYPE);
        }
    }
}
