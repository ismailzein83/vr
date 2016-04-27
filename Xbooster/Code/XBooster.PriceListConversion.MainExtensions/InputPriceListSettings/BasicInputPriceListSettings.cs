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
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(context.InputFileId, this.ExcelConversionSettings,true);
            return ConvertToPriceListItem(convertedExcel);
        }

        #endregion

        #region Private Methods

        private PriceList ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {

            Dictionary<string, PriceListRate> rateByZone = BuildRateByZone(convertedExcel);
            Dictionary<string, List<PriceListCode>> codesByZone = BuildCodesByZone(convertedExcel);
            PriceList priceListItem = ConvertToPriceList(rateByZone, codesByZone);
            return priceListItem;
           
        }
        private PriceList ConvertToPriceList(Dictionary<string, PriceListRate> rateByZone, Dictionary<string, List<PriceListCode>> codesByZone)
        {
            PriceList priceListItem = new Entities.PriceList();
            priceListItem.Records = new List<PriceListRecord>();
            foreach (var obj in codesByZone)
            {
                PriceListRecord priceListRecord = new Entities.PriceListRecord();
                priceListRecord.Zone = obj.Key;
                List<PriceListCode> codes;
                if (codesByZone.TryGetValue(priceListRecord.Zone, out codes))
                {
                    priceListRecord.Codes = codes;
                };
                PriceListRate rate;
                if (rateByZone.TryGetValue(priceListRecord.Zone, out rate))
                {
                    priceListRecord.Rate = rate.Rate;
                    priceListRecord.RateEffectiveDate = rate.RateEffectiveDate;
                }
                priceListItem.Records.Add(priceListRecord);
            }

    
            return priceListItem;
        }
        private Dictionary<string, List<PriceListCode>> BuildCodesByZone(ConvertedExcel convertedExcel)
        {
            Dictionary<string, List<PriceListCode>> codesByZone = new Dictionary<string, List<PriceListCode>>();
            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField codeField;
                    ConvertedExcelField codeEffectiveDateField;
                    DateTime? result = null;
                    if (obj.Fields.TryGetValue("EffectiveDate", out codeEffectiveDateField))
                    {
                        result = (DateTime)codeEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField) && obj.Fields.TryGetValue("Code", out codeField))
                    {


                        List<PriceListCode> codes;
                        string code = codeField.FieldValue.ToString();
                        List<PriceListCode> resolvedCodes = new List<PriceListCode>();
                        if (this.CodeLayout == CodeLayout.CammaSeparated)
                        {
                            var codesObj = code.Trim().Split(this.Delimiter).ToList();
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
                                                resolvedCodes.Add(new PriceListCode
                                                {
                                                    Code = increasedCode,
                                                    CodeEffectiveDate = result
                                                });
                                            }
                                        }
                                        else
                                        {
                                            throw new NullReferenceException("Error While Parsing Code Range.");
                                        }

                                    }
                                    else
                                    {
                                        resolvedCodes.Add(new PriceListCode
                                        {
                                            Code = codeValue,
                                            CodeEffectiveDate = result
                                        });
                                    }
                                }
                                else
                                {
                                    resolvedCodes.Add(new PriceListCode
                                    {
                                        Code = codeValue,
                                        CodeEffectiveDate = result
                                    });
                                }
                            }
                        }
                        else
                        {
                            resolvedCodes.Add(new PriceListCode
                            {
                                Code = code,
                                CodeEffectiveDate = result
                            });
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
        private Dictionary<string, PriceListRate> BuildRateByZone(ConvertedExcel convertedExcel)
        {
            ConvertedExcelList RateConvertedExcelList;
            Dictionary<string, PriceListRate> rateByZone = new Dictionary<string, PriceListRate>();
            if (convertedExcel.Lists.TryGetValue("RateList", out RateConvertedExcelList))
            {
                foreach (var obj in RateConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField rateField;
                    ConvertedExcelField rateEffectiveDateField;
                    DateTime? result = null;
                    if (obj.Fields.TryGetValue("EffectiveDate", out rateEffectiveDateField))
                    {
                       result = (DateTime)rateEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField) && obj.Fields.TryGetValue("Rate", out rateField))
                    {
                        PriceListRate rate;
                        if (!rateByZone.TryGetValue(zoneField.FieldValue.ToString(), out rate))
                        {
                            rateByZone.Add(zoneField.FieldValue.ToString(), new PriceListRate
                            {
                                Rate = (decimal)rateField.FieldValue,
                                RateEffectiveDate = result
                            });
                        }
                        else
                        {
                            if (rate.Rate == (decimal)rateField.FieldValue)
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
        private class PriceListRate
        {
            public decimal Rate { get; set; }
            public DateTime? RateEffectiveDate { get; set; }
        }
        #endregion

    }
}
