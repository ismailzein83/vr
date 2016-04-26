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

namespace XBooster.PriceListConversion.MainExtensions.InputPriceListSettings
{
    public enum CodeLayout { CodeOnEachRow = 0, CammaSeparated = 1 }
    public class BasicInputPriceListSettings : Entities.InputPriceListSettings
    {
        #region Properties
        public ExcelConversionSettings ExcelConversionSettings { get; set; }
        public CodeLayout CodeLayout { get; set; }
        public char Delimiter { get; set; }
        public bool HasCodeRange { get; set; }
        public char RangeSeparator { get; set; }
        #endregion

        #region Override

        public override PriceList Execute(IInputPriceListExecutionContext context)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(context.InputFileId, this.ExcelConversionSettings);
            return ConvertToPriceListItem(convertedExcel);
        }

        #endregion

        #region Private Methods

        private PriceList ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {
            
            Dictionary<string, decimal> rateByZone =  BuildRateByZone(convertedExcel);
            Dictionary<string, List<string>> codesByZone = BuildCodesByZone(convertedExcel);
            PriceList priceListItem = ConvertToPriceList(convertedExcel, rateByZone, codesByZone);
            return priceListItem;
           
        }
        private PriceList ConvertToPriceList(ConvertedExcel convertedExcel, Dictionary<string, decimal> rateByZone, Dictionary<string, List<string>> codesByZone)
        {
            PriceList priceListItem = new Entities.PriceList();
            priceListItem.Records = new List<PriceListRecord>();
            ConvertedExcelList convertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out convertedExcelList))
            {
                foreach (var obj in convertedExcelList.Records)
                {
                    PriceListRecord priceListRecord = new Entities.PriceListRecord();
                    ConvertedExcelField zoneField;

                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        priceListRecord.Zone = zoneField.FieldValue.ToString();
                    };
                    List<string> codes;
                    if (codesByZone.TryGetValue(priceListRecord.Zone, out codes))
                    {
                        priceListRecord.Codes = codes;
                    };
                    ConvertedExcelField bEDField;
                    if (obj.Fields.TryGetValue("EffectiveDate", out bEDField))
                    {
                        DateTime result;
                        DateTime.TryParse(bEDField.FieldValue.ToString(), out result);
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
        private Dictionary<string, List<string>> BuildCodesByZone(ConvertedExcel convertedExcel)
        {
            Dictionary<string, List<string>> codesByZone = new Dictionary<string, List<string>>();
            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField codeField;


                    if (obj.Fields.TryGetValue("Zone", out zoneField) && obj.Fields.TryGetValue("Code", out codeField))
                    {
                        List<string> codes;
                        string code = codeField.FieldValue.ToString();
                        var codesObj = code.Trim().Split(this.Delimiter).ToList();
                        List<string> resolvedCodes = new List<string>();
                        foreach (string codeValue in codesObj)
                        {
                            if (this.HasCodeRange)
                            {
                                var rangeCode = codeValue.Split(this.RangeSeparator);
                                if (rangeCode.Length > 0)
                                {
                                    long firstCode;
                                    long lastCode;
                                    if (long.TryParse(rangeCode.FirstOrDefault(), out firstCode) && long.TryParse(rangeCode.LastOrDefault(), out lastCode))
                                    {
                                        while (firstCode <= lastCode)
                                        {
                                            string increasedCode = (firstCode++).ToString();
                                            resolvedCodes.Add(increasedCode);
                                        }
                                    }
                                    else
                                    {
                                        throw new NullReferenceException("Error While Parsing Code Range.");
                                    }

                                }
                                else
                                {
                                    resolvedCodes.Add(codeValue);
                                }
                            }
                            else
                            {
                                resolvedCodes.Add(codeValue);
                            }
                        }


                        if (!codesByZone.TryGetValue(zoneField.FieldValue.ToString(), out codes))
                        {
                            codesByZone.Add(zoneField.FieldValue.ToString(), resolvedCodes);
                        }
                        else
                        {
                            codes.AddRange(resolvedCodes);
                            codesByZone[zoneField.FieldValue.ToString()] = codes;
                        }

                    };
                }
            }
            return codesByZone;
        }
        private Dictionary<string, decimal> BuildRateByZone(ConvertedExcel convertedExcel)
        {
            ConvertedExcelList RateConvertedExcelList;
            Dictionary<string, decimal> rateByZone = new Dictionary<string, decimal>();
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
            return rateByZone;
        }
        #endregion

    }
}
