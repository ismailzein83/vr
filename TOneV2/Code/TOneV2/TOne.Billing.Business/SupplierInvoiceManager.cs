using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Data;
using TOne.Billing.Entities;
using Vanrise.Entities;

namespace TOne.Billing.Business
{
    public class SupplierInvoiceManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierInvoices(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailQuery> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierInvoiceDetails(input));
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierInvoiceDetailGroupedByDay> GetFilteredSupplierInvoiceDetailsGroupedByDay(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailGroupedByDayQuery> input)
        {
            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSupplierInvoiceDetailsGroupedByDay(input));
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteInvoice(int invoiceID)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;

            ISupplierInvoiceDataManager dataManager = BillingDataManagerFactory.GetDataManager<ISupplierInvoiceDataManager>();

            bool deleted = dataManager.DeleteInvoice(invoiceID);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
