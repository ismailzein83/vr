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

        public static string ExecuteItemSetNameParts(List<InvoiceItemConcatenatedPart> itemSetNameParts, dynamic invoiceItemDetails,string currentItemSetName)
        {
            if(itemSetNameParts !=null && itemSetNameParts.Count>0)
            {
                StringBuilder itemSetName = new StringBuilder();
                InvoiceItemConcatenatedPartContext context = new InvoiceItemConcatenatedPartContext
                    {
                        InvoiceItemDetails = invoiceItemDetails,
                        CurrentItemSetName = currentItemSetName
                    };
                foreach(var part in itemSetNameParts)
                {
                   itemSetName.Append(part.Settings.GetPartText(context));
                }
                return itemSetName.ToString();
            }
            return null;
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
                if (input.Query.ItemSetNameParts != null && input.Query.ItemSetNameParts.Count>0)
                {
                    input.Query.ItemSetName = InvoiceItemManager.ExecuteItemSetNameParts(input.Query.ItemSetNameParts, input.Query.InvoiceItemDetails,input.Query.ItemSetName);
                }

                return _dataManager.GetFilteredInvoiceItems(input);
            }
        }
        #endregion

    }
}
