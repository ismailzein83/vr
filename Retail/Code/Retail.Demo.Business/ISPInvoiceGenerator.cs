using Retail.Demo.Data;
using Retail.Demo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Demo.Business
{
    public class ISPInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId { get; set; }
        public ISPInvoiceGenerator(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet();
            context.Invoice = new GeneratedInvoice
            {
                InvoiceItemSets = generatedInvoiceItemSets,
                InvoiceDetails = new InvoiceDetails()
                {
                    Amount = 0
                }
            };

        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet()
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            IDemoDataManager dataManager = RetailDemoDataManagerFactory.GetDataManager<IDemoDataManager>();

            List<NewOrders> newOrders = new List<NewOrders>();
            List<ActiveServices> activeServices = new List<ActiveServices>();

            GeneratedInvoiceItemSet newOrdersInvoiceItemSet = new GeneratedInvoiceItemSet();
            newOrdersInvoiceItemSet.SetName = "NewOrders";
            newOrdersInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

            newOrders = dataManager.GetNewOrders();
            if (newOrders != null && newOrders.Count > 0)
            {
                foreach (var newOrder in newOrders)
                {
                    newOrdersInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = newOrder,
                        Name = " "
                    });
                }
            }

            generatedInvoiceItemSets.Add(newOrdersInvoiceItemSet);

            GeneratedInvoiceItemSet activeServicesInvoiceItemSet = new GeneratedInvoiceItemSet();
            activeServicesInvoiceItemSet.SetName = "ActiveServices";
            activeServicesInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

            activeServices = dataManager.GetActiveServices();

            foreach (var activeService in activeServices)
            {
                activeServicesInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = activeService,
                    Name = " "
                });
            }
            generatedInvoiceItemSets.Add(activeServicesInvoiceItemSet);

            return generatedInvoiceItemSets;
        }

    }
}
