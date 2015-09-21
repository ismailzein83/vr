using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Data;
using TOne.Billing.Entities;

namespace TOne.Billing.Business
{
    public class CustomerInvoiceManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerInvoice> GetFilteredCustomerInvoices(Vanrise.Entities.DataRetrievalInput<CustomerInvoiceQuery> input)
        {
            ICustomerInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ICustomerInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCustomerInvoices(input));
        }
    }
}
