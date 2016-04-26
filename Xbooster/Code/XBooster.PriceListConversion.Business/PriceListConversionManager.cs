using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Business
{
    public class PriceListConversionManager
    {
        public byte[] ConvertAndDownloadPriceList(Entities.PriceListConversionInput priceListConversion)
        {
            InputPriceListExecutionContext inPutContext = new InputPriceListExecutionContext();
            inPutContext.InputFileId = priceListConversion.InputFileId;
            PriceList priceListItem = priceListConversion.InputPriceListSettings.Execute(inPutContext);
            OutputPriceListExecutionContext context = new OutputPriceListExecutionContext();
            context.Records = priceListItem.Records;
            PriceListTemplateManager priceListTemplateManager = new Business.PriceListTemplateManager();
            PriceListTemplate priceListTemplate= priceListTemplateManager.GetPriceListTemplate(priceListConversion.OutputPriceListTemplateId);
            if(priceListTemplate == null || priceListTemplate.ConfigDetails == null)
                throw new NullReferenceException("PriceListTemplate");
            OutputPriceListSettings outputPriceListSettings = priceListTemplate.ConfigDetails as OutputPriceListSettings;
            return outputPriceListSettings.Execute(context);
        }
    }
}
