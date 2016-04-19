﻿using Aspose.Cells;
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
            ConvertedExcel convertedExcel =  priceListConversion.InputPriceListSettings.Execute(inPutContext);
            PriceList priceListItem = ConvertToPriceListItem(convertedExcel);
            OutputPriceListExecutionContext context = new OutputPriceListExecutionContext();
            context.Records = priceListItem.Records;
            PriceListTemplateManager priceListTemplateManager = new Business.PriceListTemplateManager();
            PriceListTemplate priceListTemplate= priceListTemplateManager.GetPriceListTemplate(priceListConversion.OutputPriceListTemplateId);
            if(priceListTemplate == null || priceListTemplate.ConfigDetails == null)
                throw new NullReferenceException("PriceListTemplate");
            OutputPriceListSettings outputPriceListSettings = priceListTemplate.ConfigDetails as OutputPriceListSettings;
            return outputPriceListSettings.Execute(context);
        }

        private PriceList ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {
            PriceList priceListItem = new Entities.PriceList();
            priceListItem.Records = new List<PriceListRecord>();
            Dictionary<string, decimal> rateByZone = new Dictionary<string, decimal>();
            ConvertedExcelList RateConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("RateList", out RateConvertedExcelList))
            {
                foreach (var obj in RateConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField rateField;
                    if (obj.Fields.TryGetValue("Zone", out zoneField) && obj.Fields.TryGetValue("Rate", out rateField))
                    {
                        decimal rate;
                        if (!rateByZone.TryGetValue(zoneField.FieldValue.ToString(), out rate))
                        {
                            rateByZone.Add(zoneField.FieldValue.ToString(), Convert.ToDecimal(rateField.FieldValue));
                        }
                        else
                        {
                            if (rate == Convert.ToDecimal(rateField.FieldValue))
                            {
                                continue;

                            }
                            else
                            {
                                throw new Exception(string.Format("Same zone {0} of different rate exists.", zoneField.FieldValue));
                            }
                        }

                    };
                }

            }

            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    PriceListRecord priceListRecord = new Entities.PriceListRecord();
                    ConvertedExcelField zoneField;

                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        priceListRecord.Zone = zoneField.FieldValue.ToString();
                    };
                    ConvertedExcelField codeField;
                    if (obj.Fields.TryGetValue("Code", out codeField))
                    {
                        priceListRecord.Code = codeField.FieldValue.ToString();
                    };
                    ConvertedExcelField bEDField;
                    if (obj.Fields.TryGetValue("EffectiveDate", out bEDField))
                    {
                        DateTime result;
                        DateTime.TryParse(bEDField.FieldValue.ToString(),out result);
                        priceListRecord.EffectiveDate = result;
                    };
                    decimal rate;
                    if (rateByZone.TryGetValue(priceListRecord.Zone, out rate))
                    {
                        priceListRecord.Rate = rate;
                    }
                    priceListItem.Records.Add(priceListRecord);
                }

            }
            return priceListItem;
        }

    }
}
