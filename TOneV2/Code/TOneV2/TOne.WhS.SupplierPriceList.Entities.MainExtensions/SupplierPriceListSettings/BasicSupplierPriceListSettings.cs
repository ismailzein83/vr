using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions.SupplierPriceListSettings
{
    public enum CodeLayout { CodeOnEachRow = 0, Delimitedcode = 1 }
    public class BasicSupplierPriceListSettings : Entities.SupplierPriceListSettings
    {
     
        #region Properties
     
        public ExcelConversionSettings ExcelConversionSettings { get; set; }
        public CodeLayout CodeLayout { get; set; }
        public char Delimiter { get; set; }
        public bool HasCodeRange { get; set; }
        public char RangeSeparator { get; set; }
        public bool IsCommaDecimalSeparator { get; set; }
       
        #endregion

        #region Override
        public override ConvertedPriceList Execute(ISupplierPriceListExecutionContext context)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(context.InputFileId, this.ExcelConversionSettings, true, this.IsCommaDecimalSeparator);
            return ConvertToPriceListItem(convertedExcel);
        }
      
        #endregion

        #region Private Methods
        private ConvertedPriceList ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {
            return new ConvertedPriceList
            {
                PriceListCodes = BuildPriceListCodes(convertedExcel),
                PriceListRates = BuildPriceListRates(convertedExcel),
            };
        }
        private List<PriceListCode> BuildPriceListCodes(ConvertedExcel convertedExcel)
        {
            List<PriceListCode> priceListCodes = new List<PriceListCode>();
            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField codeField;
                    ConvertedExcelField codeEffectiveDateField;
                    DateTime? result = null;
                    ConvertedExcelField codeGroupField;
                    string codeGroup = null;
                    if (obj.Fields.TryGetValue("CodeGroup", out codeGroupField))
                    {
                        if (codeGroupField.FieldValue != null)
                            codeGroup = codeGroupField.FieldValue.ToString();
                    };
                    if (obj.Fields.TryGetValue("EffectiveDate", out codeEffectiveDateField))
                    {
                        if (codeEffectiveDateField.FieldValue != null)
                            result = (DateTime)codeEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        string zone = null;
                        if (zoneField.FieldValue != null)
                            zone = zoneField.FieldValue.ToString();

                        if (obj.Fields.TryGetValue("Code", out codeField))
                        {
                            if (codeField.FieldValue == null)
                            {
                                priceListCodes.Add(new PriceListCode
                                {
                                    ZoneName = zone,
                                    Code = codeGroup != null? codeGroup:null,
                                    EffectiveDate = result
                                });
                                continue;
                            }
                            string code = codeField.FieldValue.ToString();
                            if (this.CodeLayout == CodeLayout.Delimitedcode)
                            {
                                var codesObj = code.Trim().Split(this.Delimiter).ToList();
                                foreach (string codeValue in codesObj)
                                {
                                    string codeValueTrimmed = codeValue.Trim();
                                    if (codeValueTrimmed == string.Empty)
                                        continue;
                                    if (this.HasCodeRange)
                                    {
                                        var rangeCode = codeValueTrimmed.Split(this.RangeSeparator);
                                        if (rangeCode.Length > 0)
                                        {
                                            long firstCode;
                                            long lastCode;
                                            if (long.TryParse(rangeCode.FirstOrDefault(), out firstCode) && long.TryParse(rangeCode.LastOrDefault(), out lastCode))
                                            {
                                                while (firstCode <= lastCode)
                                                {
                                                    string increasedCode = (firstCode++).ToString().Trim();
                                                    priceListCodes.Add(new PriceListCode
                                                    {
                                                        Code = codeGroup != null ? string.Concat(codeGroup, increasedCode) : increasedCode,
                                                        EffectiveDate = result,
                                                        ZoneName = zone,
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                throw new NullReferenceException("Invalid code due to a wrong range separator.");
                                            }

                                        }
                                        else
                                        {
                                            priceListCodes.Add(new PriceListCode
                                            {
                                                Code = codeGroup != null ? string.Concat(codeGroup, codeValueTrimmed) : codeValueTrimmed,
                                                EffectiveDate = result,
                                                ZoneName = zone,
                                            });
                                        }
                                    }
                                    else
                                    {
                                        priceListCodes.Add(new PriceListCode
                                        {
                                            Code = codeGroup != null ? string.Concat(codeGroup, codeValueTrimmed) : codeValueTrimmed,
                                            EffectiveDate = result,
                                            ZoneName = zone,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                priceListCodes.Add(new PriceListCode
                                {
                                    Code = codeGroup != null ? string.Concat(codeGroup, code) : code,
                                    EffectiveDate = result,
                                    ZoneName = zone,
                                });
                            }
                        }
                    }
                }
            }
            return priceListCodes;
        }
        private List<PriceListRate> BuildPriceListRates(ConvertedExcel convertedExcel)
        {
            List<PriceListRate> priceListRates = new List<PriceListRate>();
            ConvertedExcelList RateConvertedExcelList;
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
                        if (rateEffectiveDateField.FieldValue != null)
                            result = (DateTime)rateEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        if (obj.Fields.TryGetValue("Rate", out rateField))
                        {
                            decimal? rate = null;
                            if(rateField.FieldValue != null)
                                rate = (decimal)rateField.FieldValue;
                            priceListRates.Add(new PriceListRate
                            {
                                ZoneName = zoneField.FieldValue !=null? zoneField.FieldValue.ToString() : null,
                                Rate = rateField.FieldValue != null? (decimal)rateField.FieldValue : default(decimal?),
                                EffectiveDate = result
                            });
                        }
                    }
                }
            }
            return priceListRates;
        }
        #endregion

    }
}
