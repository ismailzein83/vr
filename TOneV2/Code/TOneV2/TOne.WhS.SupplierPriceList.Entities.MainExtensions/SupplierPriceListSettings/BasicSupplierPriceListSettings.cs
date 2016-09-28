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
        public override Guid ConfigId { get { return new Guid("6B36007E-3333-40D3-B574-510C8338E6C0"); }}
     
        #region Properties
        public ListMapping CodeListMapping { get; set; }
        public ListMapping NormalRateListMapping { get; set; }
        public string  DateTimeFormat { get; set; }
        public List<OtherRateListMapping> OtherRateListMapping { get; set; }
        public List<FlaggedServiceListMapping> FlaggedServiceListMapping { get; set; }
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

            ExcelConversionSettings excelConversionSettings = new Vanrise.ExcelConversion.Entities.ExcelConversionSettings();
            excelConversionSettings.DateTimeFormat = this.DateTimeFormat;
            excelConversionSettings.ListMappings = new List<ListMapping>();
            excelConversionSettings.ListMappings.Add(this.CodeListMapping);
            excelConversionSettings.ListMappings.Add(this.NormalRateListMapping);
            if (this.OtherRateListMapping != null)
            {
                foreach (var list in this.OtherRateListMapping)
                {
                    excelConversionSettings.ListMappings.Add(list.RateListMapping);
                }
            }
            if (this.FlaggedServiceListMapping != null)
            {
                foreach (var list in this.FlaggedServiceListMapping)
                {
                    excelConversionSettings.ListMappings.Add(list.ServiceListMapping);
                }
            }
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(context.InputFileId, excelConversionSettings, true, this.IsCommaDecimalSeparator);
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
                PriceListOtherRates = BuildPriceListOtherRates(convertedExcel),
                PriceListServices =  BuildPriceListFlaggedServices(convertedExcel)
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
                        if (codeGroupField.FieldValue != null && !String.IsNullOrWhiteSpace(codeGroupField.FieldValue.ToString()))
                            codeGroup = codeGroupField.FieldValue.ToString();
                    };
                    if (obj.Fields.TryGetValue("EffectiveDate", out codeEffectiveDateField))
                    {
                        if (codeEffectiveDateField.FieldValue != null && !String.IsNullOrWhiteSpace(codeEffectiveDateField.FieldValue.ToString()))
                            result = (DateTime)codeEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        string zone = null;
                        if (zoneField.FieldValue != null && !String.IsNullOrWhiteSpace(zoneField.FieldValue.ToString()))
                            zone = zoneField.FieldValue.ToString();

                        if (obj.Fields.TryGetValue("Code", out codeField))
                        {
                            if (codeField.FieldValue == null || String.IsNullOrWhiteSpace(codeField.FieldValue.ToString()))
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
                        if (rateEffectiveDateField.FieldValue != null && !String.IsNullOrWhiteSpace(rateEffectiveDateField.FieldValue.ToString()))
                            result = (DateTime)rateEffectiveDateField.FieldValue;
                    };
                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        if (obj.Fields.TryGetValue("Rate", out rateField))
                        {
                            decimal? rate = null;
                            if (rateField.FieldValue != null && !String.IsNullOrWhiteSpace(rateField.FieldValue.ToString()))
                                rate = (decimal)rateField.FieldValue;
                            priceListRates.Add(new PriceListRate
                            {
                                ZoneName = zoneField.FieldValue !=null? zoneField.FieldValue.ToString() : null,
                                Rate = rate,
                                EffectiveDate = result
                            });
                        }
                    }
                }
            }
            return priceListRates;
        }

        private Dictionary<int, List<PriceListRate>> BuildPriceListOtherRates(ConvertedExcel convertedExcel)
        {
            Dictionary<int, List<PriceListRate>> otherRatesByRateType = null;

            if(this.OtherRateListMapping != null)
            {
                otherRatesByRateType = new Dictionary<int, List<PriceListRate>>();
                foreach(var list in this.OtherRateListMapping)
                {
                    ConvertedExcelList RateConvertedExcelList;
                    if (convertedExcel.Lists.TryGetValue(list.RateListMapping.ListName, out RateConvertedExcelList))
                    {
                        foreach (var obj in RateConvertedExcelList.Records)
                        {
                            ConvertedExcelField zoneField;
                            ConvertedExcelField rateField;
                            ConvertedExcelField rateEffectiveDateField;
                            DateTime? result = null;
                            if (obj.Fields.TryGetValue("EffectiveDate", out rateEffectiveDateField))
                            {
                                if (rateEffectiveDateField.FieldValue != null && !String.IsNullOrWhiteSpace(rateEffectiveDateField.FieldValue.ToString()))
                                    result = (DateTime)rateEffectiveDateField.FieldValue;
                            };
                            if (obj.Fields.TryGetValue("Zone", out zoneField))
                            {
                                if (obj.Fields.TryGetValue("Rate", out rateField))
                                {
                                    decimal? rate = null;
                                    if (rateField.FieldValue != null && !String.IsNullOrWhiteSpace(rateField.FieldValue.ToString()))
                                        rate = (decimal)rateField.FieldValue;

                                   PriceListRate priceListRate = new PriceListRate
                                    {
                                        ZoneName = zoneField.FieldValue != null ? zoneField.FieldValue.ToString() : null,
                                        Rate = rate,
                                        EffectiveDate = result
                                    };

                                    List<PriceListRate> priceListRates = null;


                                    if (!otherRatesByRateType.TryGetValue(list.RateTypeId, out priceListRates))
                                    {
                                        priceListRates = new List<PriceListRate> { priceListRate };
                                        otherRatesByRateType.Add(list.RateTypeId, priceListRates);
                                    }else
                                    {
                                        priceListRates.Add(priceListRate);
                                        otherRatesByRateType[list.RateTypeId] = priceListRates;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return otherRatesByRateType;
        }

        private List<PriceListZoneService> BuildPriceListFlaggedServices(ConvertedExcel convertedExcel)
        {
            List<PriceListZoneService> priceListZoneServices = null;

            if (this.FlaggedServiceListMapping != null)
            {
                priceListZoneServices = new List<PriceListZoneService>();
                foreach (var list in this.FlaggedServiceListMapping)
                {
                    ConvertedExcelList flaggedServiceConvertedExcelList;
                    if (convertedExcel.Lists.TryGetValue(list.ServiceListMapping.ListName, out flaggedServiceConvertedExcelList))
                    {
                        foreach (var obj in flaggedServiceConvertedExcelList.Records)
                        {
                            ConvertedExcelField zoneField;
                            ConvertedExcelField serviceEffectiveDateField;
                            DateTime? result = null;
                            if (obj.Fields.TryGetValue("EffectiveDate", out serviceEffectiveDateField))
                            {
                                if (serviceEffectiveDateField.FieldValue != null && !String.IsNullOrWhiteSpace(serviceEffectiveDateField.FieldValue.ToString()))
                                    result = (DateTime)serviceEffectiveDateField.FieldValue;
                            };
                            if (obj.Fields.TryGetValue("Zone", out zoneField))
                            {
                                string zoneName = zoneField.FieldValue != null && !String.IsNullOrWhiteSpace(zoneField.FieldValue.ToString()) ? zoneField.FieldValue.ToString() : null;
                                //if (zoneName == null || !priceListZoneServices.Any(x => zoneName.ToLower().Equals(x.ZoneName.ToLower()) && x.ZoneServiceConfigId == list.ZoneServiceConfigId))
                                // {
                                     PriceListZoneService priceListZoneService = new PriceListZoneService
                                     {
                                         ZoneName =zoneName,
                                         ZoneServiceConfigId = list.ZoneServiceConfigId,
                                         EffectiveDate = result
                                     };
                                     priceListZoneServices.Add(priceListZoneService);
                               //  }
                               
                            }
                        }
                    }
                }
            }
            return priceListZoneServices;
        }

        #endregion

    }
}
