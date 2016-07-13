using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class LoadSupplierPriceListData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> FileId { get; set; }

        [RequiredArgument]
        public InArgument<int> SupplierPriceListTemplateId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }
       
        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            int currencyId = this.CurrencyId.Get(context);

            DateTime startReading = DateTime.Now;

            SupplierPriceListTemplateManager supplierPriceListTemplateManager = new Business.SupplierPriceListTemplateManager();

            var supplierPriceListTemplate = supplierPriceListTemplateManager.GetSupplierPriceListTemplate(SupplierPriceListTemplateId.Get(context));

            var settings  = supplierPriceListTemplate.Draft;
            if (settings == null)
            {
                settings = supplierPriceListTemplate.ConfigDetails;
            }

            SupplierPriceListExecutionContext contextObj = new SupplierPriceListExecutionContext
            {
                InputFileId = FileId.Get(context)
            };
            var convertedPriceList = settings.Execute(contextObj);


            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Finished reading {0} records from the excel file. It took: {1}.", convertedPriceList.PriceListCodes.Count, spent);

        }
    }
}
