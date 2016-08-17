using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceItemManager
    {
        #region Public Methods
        public IDataRetrievalResult<InvoiceItemDetail> GetFilteredInvoiceItems(DataRetrievalInput<InvoiceItemQuery> input)
        {
            IInvoiceItemDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
            return BigDataManager.Instance.RetrieveData(input, new InvoiceItemRequestHandler(dataManager));
        }
        public IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(long invoiceId, List<string> itemSetNames)
        {
            IInvoiceItemDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
            return dataManager.GetInvoiceItemsByItemSetNames(invoiceId, itemSetNames);
        }
      
        #endregion

        #region Mappers

        public InvoiceItemDetail InvoiceItemDetailMapper(Entities.InvoiceItem invoiceItem)
        {
            InvoiceItemDetail invoiceItemDetail = new InvoiceItemDetail();
            invoiceItemDetail.Entity = invoiceItem;
            return invoiceItemDetail;
        }

        #endregion

        #region Private Classes

        private class InvoiceItemRequestHandler : BigDataRequestHandler<InvoiceItemQuery, Entities.InvoiceItem, Entities.InvoiceItemDetail>
        {
            IInvoiceItemDataManager _dataManager;
            public InvoiceItemRequestHandler(IInvoiceItemDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override InvoiceItemDetail EntityDetailMapper(Entities.InvoiceItem entity)
            {
                return new InvoiceItemDetail
                {
                    Entity = entity
                };
            }

            public override IEnumerable<Entities.InvoiceItem> RetrieveAllData(DataRetrievalInput<InvoiceItemQuery> input)
            {
                return _dataManager.GetFilteredInvoiceItems(input);
            }
        }
        #endregion

    }
}
