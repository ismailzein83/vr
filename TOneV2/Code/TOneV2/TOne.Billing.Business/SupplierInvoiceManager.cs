using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Data;
using TOne.Billing.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Billing.Business
{
    public class SupplierInvoiceManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();

            Vanrise.Entities.BigResult<SupplierInvoice> invoices = dataManager.GetFilteredSupplierInvoices(input);
            GetNames(invoices);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, invoices);
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            Vanrise.Entities.BigResult<SupplierInvoiceDetail> details = dataManager.GetFilteredSupplierInvoiceDetails(input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, details);
        }

        private void GetNames(Vanrise.Entities.BigResult<SupplierInvoice> invoices)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

            foreach (SupplierInvoice invoice in invoices.Data)
            {
                if (invoice.SupplierID != null)
                    invoice.SupplierName = manager.GetCarrirAccountName(invoice.SupplierID);
            }
        }
    }
}
