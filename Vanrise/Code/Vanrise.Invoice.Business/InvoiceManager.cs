using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceManager
    {

        #region Public Methods
        public IDataRetrievalResult<InvoiceDetail> GetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return BigDataManager.Instance.RetrieveData(input, new InvoiceRequestHandler(dataManager));
        }
        public Entities.Invoice GetInvoice(long invoiceId)
        {
            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            return dataManager.GetInvoice(invoiceId);
        }
        public void GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(createInvoiceInput.InvoiceTypeId);

            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                CustomSectionPayload = createInvoiceInput.CustomSectionPayload,
                FromDate = createInvoiceInput.FromDate,
                PartnerId = createInvoiceInput.PartnerId,
                ToDate = createInvoiceInput.ToDate
            };
            invoiceType.Settings.InvoiceGenerator.GenerateInvoice(context);

            IInvoiceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceDataManager>();
            dataManager.SaveInvoices(createInvoiceInput,context.Invoice);
        }
        #endregion


        #region Mappers

        public InvoiceDetail InvoiceDetailMapper(Entities.Invoice invoice)
        {
            InvoiceDetail invoiceDetail = new InvoiceDetail();
            invoiceDetail.Entity = invoice;
            return invoiceDetail;
        }

        #endregion


        #region Private Classes

        private class InvoiceRequestHandler : BigDataRequestHandler<InvoiceQuery, Entities.Invoice, Entities.InvoiceDetail>
        {
            IInvoiceDataManager _dataManager;
            public InvoiceRequestHandler(IInvoiceDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override InvoiceDetail EntityDetailMapper(Entities.Invoice entity)
            {
                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(entity.InvoiceTypeId);
                string partnerName = null;
                if(invoiceType != null && invoiceType.Settings !=null && invoiceType.Settings.UISettings != null && invoiceType.Settings.UISettings.PartnerManagerFQTN != null)
                {
                    PartnerManagerContext context = new PartnerManagerContext
                    {
                         PartnerId = entity.PartnerId
                    };
                    partnerName = invoiceType.Settings.UISettings.PartnerManagerFQTN.GetPartnerName(context);
                }
                return new InvoiceDetail
                {
                    Entity = entity,
                    PartnerName = partnerName
                };
            }
            public override IEnumerable<Entities.Invoice> RetrieveAllData(DataRetrievalInput<InvoiceQuery> input)
            {
                return _dataManager.GetGetFilteredInvoices(input);
            }
        }
        #endregion
    }
}
