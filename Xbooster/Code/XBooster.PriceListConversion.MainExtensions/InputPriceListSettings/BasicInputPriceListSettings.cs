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
    public enum CodeLayout { CodeOnEachRow = 0, Delimitedcode = 1 }
    public class BasicInputPriceListSettings : Entities.InputPriceListSettings
    {
        public override Guid ConfigId { get { return new Guid("613bf2aa-0f36-44f7-b311-9c34f023b273"); } }

        #region Properties
        public ExcelConversionSettings ExcelConversionSettings { get; set; }
        public CodeLayout CodeLayout { get; set; }
        public char Delimiter { get; set; }
        public bool HasCodeRange { get; set; }
        public char RangeSeparator { get; set; }

        public bool IsCommaDecimalSeparator { get; set; }
        #endregion

        #region Override

        public override PriceList Execute(IInputPriceListExecutionContext context)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(context.InputFileId, this.ExcelConversionSettings, true, this.IsCommaDecimalSeparator);
            return ConvertToPriceListItem(convertedExcel);
        }
        #endregion

        #region Private Methods
        private PriceList ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {
            Dictionary<string, string> zoneByZone = BuildZoneByZone(convertedExcel);
            Dictionary<string, PriceListRate> rateByZone = BuildRateByZone(convertedExcel);
            Dictionary<string, List<PriceListCode>> codesByZone = BuildCodesByZone(convertedExcel);
            ValidateCode(codesByZone, rateByZone);
            PriceList priceListItem = ConvertToPriceList(rateByZone, codesByZone, zoneByZone);
            return priceListItem;
        }
        private PriceList ConvertToPriceList(Dictionary<string, PriceListRate> rateByZone, Dictionary<string, List<PriceListCode>> codesByZone, Dictionary<string, string>  zoneByZone)
        {
            PriceList priceListItem = new Entities.PriceList();
            priceListItem.Records = new List<PriceListRecord>();
            foreach (var obj in codesByZone)
            {
                PriceListRecord priceListRecord = new Entities.PriceListRecord();
                string zoneName;
                if (zoneByZone.TryGetValue(obj.Key, out zoneName))
                {
                    priceListRecord.Zone = zoneName;
                }
                List<PriceListCode> codes;
                if (codesByZone.TryGetValue(obj.Key, out codes))
                {
                    priceListRecord.Codes = codes;
                };
                PriceListRate rate;
                if (rateByZone.TryGetValue(obj.Key, out rate))
                {
                    priceListRecord.Rate = rate.Rate;
                    priceListRecord.RateEffectiveDate = rate.RateEffectiveDate;
                }
                priceListItem.Records.Add(priceListRecord);
            }

    
            return priceListItem;
        }
        private Dictionary<string, string> BuildZoneByZone(ConvertedExcel convertedExcel)
        {
            Dictionary<string, string> zoneByZone = new Dictionary<string, string>();
            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    string zoneName;
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        if (!zoneByZone.TryGetValue(zoneField.FieldValue.ToString().Trim().ToLower(), out zoneName))
                        {
                            zoneByZone.Add(zoneField.FieldValue.ToString().Trim().ToLower(), zoneField.FieldValue.ToString());
                        }   
                    }
                }
            }
            return zoneByZone;
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
                    ConvertedExcelField codeGroupField;
                    string codeGroup = null;
                    if (obj.Fields.TryGetValue("CodeGroup", out codeGroupField))
                    {
                        if (codeGroupField.FieldValue != null)
                            codeGroup = codeGroupField.FieldValue.ToString().Trim();
                    };
                    if (obj.Fields.TryGetValue("EffectiveDate", out codeEffectiveDateField))
                    {
                        if (codeEffectiveDateField.FieldValue !=null)
                         result = (DateTime)codeEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {

                        if (obj.Fields.TryGetValue("Code", out codeField))
                        {

                           
                            List<PriceListCode> codes;
                            if (codeField.FieldValue == null)
                            {
                                throw new Exception(string.Format("Zone {0} has no code defined.", zoneField.FieldValue));
                            }
                            string code = codeField.FieldValue.ToString().Trim();
                            List<PriceListCode> resolvedCodes = new List<PriceListCode>();
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
                                                    resolvedCodes.Add(new PriceListCode
                                                    {
                                                        Code = codeGroup != null ? string.Concat(codeGroup, increasedCode) : increasedCode,
                                                        CodeEffectiveDate = result
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
                                            resolvedCodes.Add(new PriceListCode
                                            {
                                                Code = codeGroup != null ? string.Concat(codeGroup, codeValueTrimmed) : codeValueTrimmed,
                                                CodeEffectiveDate = result
                                            });
                                        }
                                    }
                                    else
                                    {
                                        resolvedCodes.Add(new PriceListCode
                                        {
                                            Code = codeGroup != null ? string.Concat(codeGroup, codeValueTrimmed) : codeValueTrimmed,
                                            CodeEffectiveDate = result
                                        });
                                    }
                                }
                            }
                            else
                            {
                                resolvedCodes.Add(new PriceListCode
                                {
                                    Code = codeGroup != null ? string.Concat(codeGroup, code) : code,
                                    CodeEffectiveDate = result
                                });
                            }
                            var zoneName = zoneField.FieldValue.ToString().Trim().ToLower();
                            if (!codesByZone.TryGetValue(zoneName, out codes))
                            {
                                codesByZone.Add(zoneName, resolvedCodes);
                            }
                            else
                            {
                                codes.AddRange(resolvedCodes);
                                codesByZone[zoneName] = codes;
                            }

                        }
                        else
                        {
                            throw new Exception(string.Format("Zone {0} has no code defined.", zoneField.FieldValue));
                        };
                    }


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
                        if (rateEffectiveDateField.FieldValue !=null)
                          result = (DateTime)rateEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        PriceListRate rate;
                        if (obj.Fields.TryGetValue("Rate", out rateField))
                        {
                            if (rateField.FieldValue == null)
                            {
                                throw new Exception(string.Format("Zone {0} has no rate defined.", zoneField.FieldValue));
                            }
                            var zoneName = zoneField.FieldValue.ToString().Trim().ToLower();

                            if (!rateByZone.TryGetValue(zoneName, out rate))
                            {
                                rateByZone.Add(zoneName, new PriceListRate
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
                                    throw new Exception(string.Format("Zone {0} can have only one Rate.", zoneField.FieldValue));
                                }
                            }
                        }else
                        {
                            throw new Exception(string.Format("Zone {0} has no rate defined.", zoneField.FieldValue));
                        }


                    }
                }
            }
            return rateByZone;
        }

        private void ValidateCode(Dictionary<string, List<PriceListCode>> codesByZone, Dictionary<string, PriceListRate> rateByZone)
        {
            foreach(var zone in codesByZone)
            {
                var codes = zone.Value;
                foreach(var zoneObj in codesByZone)
                {
                    if (zone.Key != zoneObj.Key)
                    {
                        var code = zoneObj.Value.FirstOrDefault(x => codes.Any(y => y.Code == x.Code));
                        if(code != null)
                        {
                          throw new Exception(string.Format("Code {0} must be assigned to only one zone.", code.Code));

                        }
                    }
                  
                }
                foreach (var code in codes)
                {
                    if(codes.Where(x=>x.Code == code.Code).Count()>1)
                    {
                        throw new Exception(string.Format("Code {0} should not be duplicated.", code.Code));
                    }
                }

                PriceListRate rateObj = null;

                if(!rateByZone.TryGetValue(zone.Key,out rateObj))
                {
                    throw new Exception(string.Format("Zone {0} has no rate defined.", zone.Key));
                }
            }
        }
        private class PriceListRate
        {
            public decimal Rate { get; set; }
            public DateTime? RateEffectiveDate { get; set; }
        }


        #endregion

    }
}
