using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Data;
using TOne.Billing.Entities;

namespace TOne.Billing.Business
{
    public class SupplierInvoiceManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierInvoices(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierInvoiceDetails(input));
        }
    }
}
