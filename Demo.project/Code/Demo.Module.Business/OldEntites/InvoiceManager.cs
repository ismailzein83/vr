using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class InvoiceManager
    {

        #region Public Methods
        public List<Invoice> GetInvoices()
        {
            IInvoiceDataManager invoiceDataManager = DemoModuleFactory.GetDataManager<IInvoiceDataManager>();
            return invoiceDataManager.GetInvoices();
        }
        #endregion
    }
}
