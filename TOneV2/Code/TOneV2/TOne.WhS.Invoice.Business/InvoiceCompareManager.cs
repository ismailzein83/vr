using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Business.Extensions;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceCompareManager
    {

        CurrencyManager _currencyManager = new CurrencyManager();

        #region Voice Comparison

        public IDataRetrievalResult<InvoiceComparisonVoiceResultDetail> CompareVoiceInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonVoiceInput> input)
        {
            if (input.ResultKey == null)
            {
                ValidateCompareVoiceInvoices(input.Query);
            }
            return BigDataManager.Instance.RetrieveData(input, new InvoiceComparisonVoiceRequestHandler());
        }

        public static InvoiceComparisonVoiceResultDetail InvoiceComparisonVoiceMapper(InvoiceComparisonVoiceResult invoiceComparisonResult, CompareInvoiceAction compareInvoiceAction)
        {

            string description = null;
            string resultTooltipDescription = null;

            switch (invoiceComparisonResult.Result)
            {
                case ComparisonResult.Identical:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    break;
                case ComparisonResult.MajorDiff:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Difference greater than threshold.";
                    break;
                case ComparisonResult.MinorDiff:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Difference less than threshold.";
                    break;
                case ComparisonResult.MissingProvider:
                    description = string.Format("{0} {1}", Utilities.GetEnumDescription(invoiceComparisonResult.Result), compareInvoiceAction.PartnerLabel);
                    resultTooltipDescription = "Record doesn't exist for provider.";
                    break;
                case ComparisonResult.MissingSystem:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Record doesn't exist for system.";
                    break;
            }

            return new InvoiceComparisonVoiceResultDetail
            {
                Entity = invoiceComparisonResult,
                ResultDescription = description,
                ResultTooltipDescription = resultTooltipDescription
            };
        }

        private void ValidateCompareVoiceInvoices(InvoiceComparisonVoiceInput input)
        {
            CompareVoiceInvoices(input);
        }

        public List<InvoiceComparisonVoiceResult> CompareVoiceInvoices(InvoiceComparisonVoiceInput input)
        {
            if (input.ListMapping == null || input.ListMapping.FieldMappings == null || input.ListMapping.FieldMappings.Count == 0)
                return null;

            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(input.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", input.InvoiceTypeId);
            var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.InvoiceTypeId, input.InvoiceActionId);
            invoiceAction.ThrowIfNull("invoiceAction");
            var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
            invoiceActionSettings.ThrowIfNull("invoiceActionSettings");

            var invoiceActionVoiceSettings = invoiceActionSettings.VoiceSettings;
            invoiceActionVoiceSettings.ThrowIfNull("invoiceActionVoiceSettings");

            var itemGrouping = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == invoiceActionVoiceSettings.ItemGroupingId);
            var longPrecision = new GeneralSettingsManager().GetLongPrecisionValue();
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var invoiceItems = invoiceItemManager.GetInvoiceItemsByItemSetNames(input.InvoiceId, new List<String> { itemGrouping.ItemSetName }, CompareOperator.Equal);
            InvoiceItemGroupingManager invoiceItemGroupingManager = new InvoiceItemGroupingManager();
            var invoice = new Vanrise.Invoice.Business.InvoiceManager().GetInvoice(input.InvoiceId);
            invoice.ThrowIfNull("invoice", input.InvoiceId);
            //   var currencyId = invoice.Details.GetType().GetProperty(invoiceType.Settings.CurrencyFieldName).GetValue(invoice.Details, null);
            //  string currencySymbol = null;
            //  if (currencyId != null)
            //     currencySymbol = new CurrencyManager().GetCurrencySymbol(currencyId);

            List<Guid> dimensions = new List<Guid>();
            dimensions.Add(invoiceActionVoiceSettings.ZoneDimensionId);
            //  dimensions.Add(invoiceActionSettings.FromDateDimensionId);
            //  dimensions.Add(invoiceActionSettings.ToDateDimensionId);
            dimensions.Add(invoiceActionVoiceSettings.CurrencyDimensionId);

            dimensions.Add(invoiceActionVoiceSettings.RateDimensionId);

            List<Guid> measures = new List<Guid>();
            measures.Add(invoiceActionVoiceSettings.AmountMeasureId);
            measures.Add(invoiceActionVoiceSettings.DurationMeasureId);
            measures.Add(invoiceActionVoiceSettings.NumberOfCallsMeasureId);
            measures.Add(invoiceActionVoiceSettings.RateMeasureId);

            measures.Add(invoiceActionVoiceSettings.FromDateMeasureId);
            measures.Add(invoiceActionVoiceSettings.ToDateMeasureId);


            GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
            {
                DimensionIds = dimensions,
                MeasureIds = measures,
                InvoiceTypeId = input.InvoiceTypeId,
                ItemGroupingId = invoiceActionVoiceSettings.ItemGroupingId
            };
            var groupedItems = invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);

            var systemInvoiceItems = ConverGroupingItemsToVoiceComparisonResult(groupedItems, invoiceActionSettings, itemGrouping, input.DecimalDigits, longPrecision);

            ExcelConvertor excelConvertor = new ExcelConvertor();

            ExcelConversionSettings excelConversionSettings = new ExcelConversionSettings();
            excelConversionSettings.DateTimeFormat = input.DateTimeFormat;
            excelConversionSettings.ListMappings = new List<ListMapping>();
            excelConversionSettings.ListMappings.Add(input.ListMapping);

            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(input.InputFileId, excelConversionSettings, true, false);
            return ProccessInvoiceComparisonResult(convertedExcel, systemInvoiceItems, input.Threshold, input.DateTimeFormat, input.ComparisonCriterias, input.DecimalDigits, longPrecision);

        }

        private List<InvoiceComparisonVoiceResult> ProccessInvoiceComparisonResult(ConvertedExcel convertedExcel, Dictionary<VoiceComparisonKey, VoiceInvoiceItemToCompare> systemInvoiceItems, decimal threshold, string dateTimeFormat, List<VoiceComparisonCriteria> comparisonCriterias, int? decimalDigits, int longPrecision)
        {
            List<InvoiceComparisonVoiceResult> invoiceComparisonResults = new List<InvoiceComparisonVoiceResult>();
            ConvertedExcelList convertedExcelList;
            if (convertedExcel.Lists.TryGetValue("VoiceMainList", out convertedExcelList))
            {
                foreach (var obj in convertedExcelList.Records)
                {

                    InvoiceComparisonVoiceResult invoiceComparisonResult = new InvoiceComparisonVoiceResult();
                    FillInvoiceComparisonVoiceProviderFields(invoiceComparisonResult, obj.Fields, dateTimeFormat, decimalDigits, longPrecision);

                    VoiceComparisonKey comparisonKey = new VoiceComparisonKey
                    {
                        Destination = invoiceComparisonResult.Destination,
                        From = invoiceComparisonResult.From.Date,
                        To = invoiceComparisonResult.To.Date,
                        Currency = invoiceComparisonResult.Currency
                        //,
                        //Rate = invoiceComparisonResult.ProviderRate
                    };

                    VoiceInvoiceItemToCompare invoiceItemToCompare;
                    if (systemInvoiceItems.TryGetValue(comparisonKey, out invoiceItemToCompare))
                    {
                        invoiceComparisonResult.SystemAmount = invoiceItemToCompare.Amount;
                        invoiceComparisonResult.SystemCalls = invoiceItemToCompare.Calls;
                        invoiceComparisonResult.SystemDuration = invoiceItemToCompare.Duration;
                        invoiceComparisonResult.SystemRate = invoiceItemToCompare.Rate;
                        invoiceComparisonResult.Currency = _currencyManager.GetCurrencySymbol(invoiceItemToCompare.CurrencyId);
                        LabelColor resultLabelColor;
                        invoiceComparisonResult.Result = GetInvoiceComparisonVoiceResult(threshold, invoiceItemToCompare.Amount, invoiceItemToCompare.Calls, invoiceItemToCompare.Duration, invoiceComparisonResult.ProviderAmount, invoiceComparisonResult.ProviderCalls, invoiceComparisonResult.ProviderDuration, comparisonCriterias, out resultLabelColor);
                        invoiceComparisonResult.ResultColor = resultLabelColor;
                        systemInvoiceItems.Remove(comparisonKey);
                    }
                    else
                    {
                        // invoiceComparisonResult.Currency = currencySymbol;
                        invoiceComparisonResult.Result = ComparisonResult.MissingSystem;
                        invoiceComparisonResult.ResultColor = LabelColor.Error;
                    }

                    invoiceComparisonResult.DiffCallsPercentage = CalculateDiffPercentageValue(invoiceComparisonResult.SystemCalls, invoiceComparisonResult.ProviderCalls);
                    invoiceComparisonResult.DiffCalls = CalculateDiffValue(invoiceComparisonResult.SystemCalls, invoiceComparisonResult.ProviderCalls);
                    invoiceComparisonResult.DiffCallsColor = GetDiffLabelColor(invoiceComparisonResult.DiffCallsPercentage, threshold);

                    invoiceComparisonResult.DiffAmountPercentage = CalculateDiffPercentageValue(invoiceComparisonResult.SystemAmount, invoiceComparisonResult.ProviderAmount);
                    invoiceComparisonResult.DiffAmountColor = GetDiffLabelColor(invoiceComparisonResult.DiffAmountPercentage, threshold);
                    invoiceComparisonResult.DiffAmount = CalculateDiffValue(invoiceComparisonResult.SystemAmount, invoiceComparisonResult.ProviderAmount);

                    invoiceComparisonResult.DiffDurationPercentage = CalculateDiffPercentageValue(invoiceComparisonResult.SystemDuration, invoiceComparisonResult.ProviderDuration);
                    invoiceComparisonResult.DiffDurationColor = GetDiffLabelColor(invoiceComparisonResult.DiffDurationPercentage, threshold);
                    invoiceComparisonResult.DiffDuration = CalculateDiffValue(invoiceComparisonResult.SystemDuration, invoiceComparisonResult.ProviderDuration);


                    invoiceComparisonResults.Add(invoiceComparisonResult);
                }
                AddMissingSystemInvoices(invoiceComparisonResults, systemInvoiceItems);
            }
            return invoiceComparisonResults;
        }
        private ComparisonResult GetInvoiceComparisonVoiceResult(decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration, List<VoiceComparisonCriteria> comparisonCriterias, out LabelColor resultLabelColor)
        {
            if (VoiceCheckIfMajorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, sysDuration, providerAmount, providerNCalls, providerDuration))
            {
                resultLabelColor = LabelColor.Error;
                return ComparisonResult.MajorDiff;
            }
            if (VoiceCheckIfMinorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, sysDuration, providerAmount, providerNCalls, providerDuration))
            {
                resultLabelColor = LabelColor.Warning;
                return ComparisonResult.MinorDiff;
            }
            resultLabelColor = LabelColor.Success;
            return ComparisonResult.Identical;

        }
        private bool VoiceCheckIfMajorErrorDiff(List<VoiceComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case VoiceComparisonCriteria.Calls:
                            if (CheckThresholdPercentage(sysNCalls, providerNCalls, threshold))
                            {

                                return true;
                            }
                            break;
                        case VoiceComparisonCriteria.Amount:
                            if (CheckThresholdPercentage(sysAmount, providerAmount, threshold))
                            {

                                return true;
                            }
                            break;
                        case VoiceComparisonCriteria.Duration:
                            if (CheckThresholdPercentage(sysDuration, providerDuration, threshold))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            else
            {
                if (CheckThresholdPercentage(sysAmount, providerAmount, threshold)
                   || CheckThresholdPercentage(sysNCalls, providerNCalls, threshold)
                   || CheckThresholdPercentage(sysDuration, providerDuration, threshold))
                {
                    return true;
                }
            }

            return false;
        }
        private bool VoiceCheckIfMinorErrorDiff(List<VoiceComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case VoiceComparisonCriteria.Calls:
                            if (providerNCalls.HasValue && sysNCalls != providerNCalls.Value)
                                return true;
                            break;
                        case VoiceComparisonCriteria.Amount:
                            if (providerAmount.HasValue && sysAmount != providerAmount.Value)
                                return true;
                            break;
                        case VoiceComparisonCriteria.Duration:
                            if (providerDuration.HasValue && sysDuration != providerDuration.Value)
                                return true;
                            break;
                    }
                }
            }
            else
            {
                if ((providerAmount.HasValue && sysAmount != providerAmount.Value)
                                       || (providerNCalls.HasValue && sysNCalls != providerNCalls.Value)
                                       || (providerDuration.HasValue && sysDuration != providerDuration.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<VoiceComparisonKey, VoiceInvoiceItemToCompare> ConverGroupingItemsToVoiceComparisonResult(IEnumerable<GroupingInvoiceItemDetail> groupedItems, CompareInvoiceAction compareInvoiceAction, ItemGrouping itemGrouping, int? decimalDigits, int longPrecision)
        {
            Dictionary<VoiceComparisonKey, VoiceInvoiceItemToCompare> invoiceItemsToCompare = new Dictionary<VoiceComparisonKey, VoiceInvoiceItemToCompare>();
            if (groupedItems != null)
            {

                AggregateItemField amountMeasure = null;
                AggregateItemField durationMeasure = null;
                AggregateItemField callsMeasure = null;
                AggregateItemField rateMeasure = null;
                AggregateItemField fromDateMeasure = null;
                AggregateItemField toDateMeasure = null;
                foreach (var measure in itemGrouping.AggregateItemFields)
                {
                    if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.AmountMeasureId)
                    {
                        amountMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.DurationMeasureId)
                    {
                        durationMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.NumberOfCallsMeasureId)
                    {
                        callsMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.RateMeasureId)
                    {
                        rateMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.FromDateMeasureId)
                    {
                        fromDateMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.VoiceSettings.ToDateMeasureId)
                    {
                        toDateMeasure = measure;
                    }
                }


                foreach (var item in groupedItems)
                {
                    var zoneDimension = item.DimensionValues[0];
                    //var fromDateDimension = item.DimensionValues[1];
                    //var toDateDimension = item.DimensionValues[2];
                    var currecnyDimension = item.DimensionValues[1];
                    VoiceInvoiceItemToCompare invoiceItemToCompare = new VoiceInvoiceItemToCompare
                    {
                        Destination = zoneDimension != null ? zoneDimension.Name : null,
                        CurrencyId = currecnyDimension != null ? Convert.ToInt32(currecnyDimension.Value) : default(int),
                    };

                    InvoiceGroupingMeasureValue amountMeasureItem;
                    InvoiceGroupingMeasureValue callsMeasureItem;
                    InvoiceGroupingMeasureValue durationMeasureItem;
                    InvoiceGroupingMeasureValue rateMeasureItem;


                    InvoiceGroupingMeasureValue fromDateMeasureItem;
                    InvoiceGroupingMeasureValue toDateMeasureItem;


                    if (amountMeasure != null && item.MeasureValues.TryGetValue(amountMeasure.FieldName, out amountMeasureItem))
                    {
                        invoiceItemToCompare.Amount = amountMeasureItem.Value != null ? Convert.ToDecimal(amountMeasureItem.Value) : default(decimal);
                    }

                    if (callsMeasure != null && item.MeasureValues.TryGetValue(callsMeasure.FieldName, out callsMeasureItem))
                    {
                        invoiceItemToCompare.Calls = callsMeasureItem.Value != null ? Convert.ToInt32(callsMeasureItem.Value) : default(Int32);
                    }

                    if (durationMeasure != null && item.MeasureValues.TryGetValue(durationMeasure.FieldName, out durationMeasureItem))
                    {
                        invoiceItemToCompare.Duration = durationMeasureItem.Value != null ? Convert.ToDecimal(durationMeasureItem.Value) : default(decimal);
                    }
                    if (rateMeasure != null && item.MeasureValues.TryGetValue(rateMeasure.FieldName, out rateMeasureItem))
                    {
                        invoiceItemToCompare.Rate = rateMeasureItem.Value != null ? Convert.ToDecimal(rateMeasureItem.Value) : default(decimal);
                    }

                    if (fromDateMeasure != null && item.MeasureValues.TryGetValue(fromDateMeasure.FieldName, out fromDateMeasureItem))
                    {
                        invoiceItemToCompare.From = fromDateMeasureItem.Value != null ? Convert.ToDateTime(fromDateMeasureItem.Value) : default(DateTime);
                    }

                    if (toDateMeasure != null && item.MeasureValues.TryGetValue(toDateMeasure.FieldName, out toDateMeasureItem))
                    {
                        invoiceItemToCompare.To = toDateMeasureItem.Value != null ? Convert.ToDateTime(toDateMeasureItem.Value) : default(DateTime);
                    }


                    invoiceItemToCompare.Rate = Math.Round(invoiceItemToCompare.Rate, longPrecision, MidpointRounding.AwayFromZero);
                    if (decimalDigits.HasValue)
                    {
                        invoiceItemToCompare.Amount = Math.Round(invoiceItemToCompare.Amount, decimalDigits.Value, MidpointRounding.AwayFromZero);
                        invoiceItemToCompare.Duration = Math.Round(invoiceItemToCompare.Duration, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }

                    var comparisonKey = new VoiceComparisonKey
                    {
                        Destination = invoiceItemToCompare.Destination,
                        From = invoiceItemToCompare.From.Date,
                        To = invoiceItemToCompare.To.Date,
                        Currency = _currencyManager.GetCurrencySymbol(invoiceItemToCompare.CurrencyId)
                    };
                    if (!invoiceItemsToCompare.ContainsKey(comparisonKey))
                    {
                        invoiceItemsToCompare.Add(comparisonKey, invoiceItemToCompare);
                    }
                    else
                    {
                        throw new Exception(string.Format("Same Period Exist for {0} From: '{1}' To: '{2}'. ", invoiceItemToCompare.Destination, invoiceItemToCompare.From.Date, invoiceItemToCompare.To.Date));
                    }
                }

            }
            return invoiceItemsToCompare;
        }
        private void AddMissingSystemInvoices(List<InvoiceComparisonVoiceResult> invoiceComparisonResults, Dictionary<VoiceComparisonKey, VoiceInvoiceItemToCompare> systemInvoiceItems)
        {
            if (systemInvoiceItems.Count > 0)
            {
                foreach (var item in systemInvoiceItems)
                {
                    var invoiceComparisonResult = new InvoiceComparisonVoiceResult
                    {
                        SystemDuration = item.Value.Duration,
                        SystemCalls = item.Value.Calls,
                        SystemRate = item.Value.Rate,
                        SystemAmount = item.Value.Amount,
                        Currency = _currencyManager.GetCurrencySymbol(item.Value.CurrencyId),
                        Destination = item.Value.Destination,
                        From = item.Value.From,
                        To = item.Value.To,
                        Result = ComparisonResult.MissingProvider,
                        ResultColor = LabelColor.Error
                    };
                    invoiceComparisonResults.Add(invoiceComparisonResult);
                }
            }
        }
        private void FillInvoiceComparisonVoiceProviderFields(InvoiceComparisonVoiceResult invoiceComparisonResult, ConvertedExcelFieldsByName fields, string dateTimeFormat, int? decimalDigits, int longPrecision)
        {

            #region Fill Destination
            ConvertedExcelField zoneField;
            if (fields.TryGetValue("Zone", out zoneField))
            {
                if (zoneField.FieldValue == null || String.IsNullOrWhiteSpace(zoneField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped Zone cannot have a null value.");
                invoiceComparisonResult.Destination = zoneField.FieldValue.ToString();
            };


            #endregion

            ConvertedExcelField currency;
            if (fields.TryGetValue("Currency", out currency))
            {
                if (currency.FieldValue == null || String.IsNullOrWhiteSpace(currency.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped Currency cannot have a null value.");
                invoiceComparisonResult.Currency = currency.FieldValue.ToString();
            };
            #region Fill FromDate
            ConvertedExcelField fromDateField;
            if (fields.TryGetValue("FromDate", out fromDateField))
            {
                if (fromDateField.FieldValue == null || String.IsNullOrWhiteSpace(fromDateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped from date cannot have a null value.");

                if (fromDateField.FieldValue is DateTime)
                {
                    invoiceComparisonResult.From = Convert.ToDateTime(fromDateField.FieldValue).Date;
                }
                else
                {
                    DateTime fromDate;
                    if (DateTime.TryParseExact(fromDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out fromDate))
                        invoiceComparisonResult.From = fromDate.Date;
                    else
                        throw new NullReferenceException("From date has an invalid format.");
                }

            };
            #endregion

            #region Fill ToDate

            ConvertedExcelField toDateField;
            if (fields.TryGetValue("ToDate", out toDateField))
            {
                if (toDateField.FieldValue == null || String.IsNullOrWhiteSpace(toDateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped to date cannot have a null value.");

                if (toDateField.FieldValue is DateTime)
                {
                    invoiceComparisonResult.To = Convert.ToDateTime(toDateField.FieldValue).Date;
                }
                else
                {
                    DateTime toDate;
                    if (DateTime.TryParseExact(toDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out toDate))
                        invoiceComparisonResult.To = toDate.Date;
                    else
                        throw new NullReferenceException("To date has an invalid format.");
                }

            };
            #endregion

            #region Fill Rate

            ConvertedExcelField rateField;
            if (fields.TryGetValue("Rate", out rateField))
            {
                if (rateField.FieldValue == null || String.IsNullOrWhiteSpace(rateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped rate cannot have a null value.");

                Decimal rate;
                if (Decimal.TryParse(rateField.FieldValue.ToString(), out rate))
                {
                    invoiceComparisonResult.ProviderRate = Math.Round(rate, longPrecision, MidpointRounding.AwayFromZero);
                }
                else
                    throw new NullReferenceException("Rate has an invalid format.");

            };

            #endregion

            #region Fill NumberOfCalls

            ConvertedExcelField callsField;
            if (fields.TryGetValue("NumberOfCalls", out callsField))
            {
                if (callsField.FieldValue == null || String.IsNullOrWhiteSpace(callsField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped number of calls cannot have a null value.");
                Decimal providerCalls;
                if (Decimal.TryParse(callsField.FieldValue.ToString(), out providerCalls))
                {
                    if (decimalDigits.HasValue)
                    {
                        invoiceComparisonResult.ProviderCalls = Math.Round(providerCalls, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }
                    else
                        invoiceComparisonResult.ProviderCalls = providerCalls;
                }
                else
                    throw new NullReferenceException("Number of calls has an invalid format.");
            };
            #endregion

            #region Fill Amount

            ConvertedExcelField amountField;
            if (fields.TryGetValue("Amount", out amountField))
            {
                if (amountField.FieldValue == null || String.IsNullOrWhiteSpace(amountField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped amount cannot have a null value.");
                Decimal providerAmount;
                if (Decimal.TryParse(amountField.FieldValue.ToString(), out providerAmount))
                {
                    if (decimalDigits.HasValue)
                    {
                        invoiceComparisonResult.ProviderAmount = Math.Round(providerAmount, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }
                    else
                        invoiceComparisonResult.ProviderAmount = providerAmount;
                }
                else
                    throw new NullReferenceException("Amount has an invalid format.");

            };
            #endregion

            #region Fill Duration

            ConvertedExcelField durationField;
            if (fields.TryGetValue("Duration", out durationField))
            {
                if (durationField.FieldValue == null || String.IsNullOrWhiteSpace(durationField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped duration cannot have a null value.");

                Decimal providerDuration;
                if (Decimal.TryParse(durationField.FieldValue.ToString(), out providerDuration))
                {
                    if (decimalDigits.HasValue)
                    {
                        invoiceComparisonResult.ProviderDuration = Math.Round(providerDuration, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }
                    else
                        invoiceComparisonResult.ProviderDuration = providerDuration;
                }
                else
                    throw new NullReferenceException("Duration has an invalid format.");
            };
            #endregion

        }




        private struct VoiceComparisonKey
        {
            public string Destination { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string Currency { get; set; }
        }
        private class VoiceInvoiceItemToCompare
        {
            public int CurrencyId { get; set; }
            public string Destination { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            // public string Currency { get; set; }
            public Decimal Rate { get; set; }
            public Decimal Duration { get; set; }
            public Decimal Amount { get; set; }
            public int Calls { get; set; }
        }

        #region Private Classes

        private class InvoiceComparisonVoiceRequestHandler : BigDataRequestHandler<InvoiceComparisonVoiceInput, InvoiceComparisonVoiceResult, InvoiceComparisonVoiceResultDetail>
        {
            public InvoiceComparisonVoiceRequestHandler()
            {

            }
            public override InvoiceComparisonVoiceResultDetail EntityDetailMapper(InvoiceComparisonVoiceResult entity)
            {
                return null;
            }
            protected override Vanrise.Entities.BigResult<InvoiceComparisonVoiceResultDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonVoiceInput> input, IEnumerable<InvoiceComparisonVoiceResult> allRecords)
            {

                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.Query.InvoiceTypeId, input.Query.InvoiceActionId);
                invoiceAction.ThrowIfNull("invoiceAction");
                var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
                invoiceActionSettings.ThrowIfNull("invoiceActionSettings");
                Func<InvoiceComparisonVoiceResult, bool> filterExpression = (invoiceComparisonResult) =>
                {
                    if (input.Query.ComparisonResults != null)
                    {
                        if (!input.Query.ComparisonResults.Contains(invoiceComparisonResult.Result))
                            return false;
                    }
                    return true;
                };

                return allRecords.ToBigResult(input, filterExpression, (entity) => InvoiceCompareManager.InvoiceComparisonVoiceMapper(entity, invoiceActionSettings));
            }
            public override IEnumerable<InvoiceComparisonVoiceResult> RetrieveAllData(DataRetrievalInput<InvoiceComparisonVoiceInput> input)
            {
                return new InvoiceCompareManager().CompareVoiceInvoices(input.Query);
            }
            protected override ResultProcessingHandler<InvoiceComparisonVoiceResultDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceComparisonVoiceInput> input, BigResult<InvoiceComparisonVoiceResultDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceComparisonVoiceResultDetail>
                {
                    ExportExcelHandler = new InvoiceComparisonVoiceExcelExportHandler(input.Query)
                };
            }

        }

        private class InvoiceComparisonVoiceExcelExportHandler : ExcelExportHandler<InvoiceComparisonVoiceResultDetail>
        {
            InvoiceComparisonVoiceInput _query;
            public InvoiceComparisonVoiceExcelExportHandler(InvoiceComparisonVoiceInput query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvoiceComparisonVoiceResultDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Invoice Comparison",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceAction = invoiceTypeManager.GetInvoiceAction(_query.InvoiceTypeId, _query.InvoiceActionId);
                invoiceAction.ThrowIfNull("invoiceAction");
                var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
                invoiceActionSettings.ThrowIfNull("invoiceActionSettings");

                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Destination",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "From",
                    CellType = ExcelCellType.DateTime,
                    DateTimeType = DateTimeType.Date
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Till",
                    CellType = ExcelCellType.DateTime,
                    DateTimeType = DateTimeType.Date
                });
                //sheet.Header.Cells.Add(new ExportExcelHeaderCell
                //{
                //    Title = "Curr.",
                //               });

                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Rate",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Rate", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Calls",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Calls", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Calls",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Calls %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Duration",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Duration", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Duration",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Duration %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Amount",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Amount", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Amount",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Amount %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Result",
                });

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<InvoiceComparisonVoiceResultDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach (var item in results.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.Destination });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.From });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.To });
                            //  row.Cells.Add(new ExportExcelCell { Value =  item.Entity.Currency });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemRate });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderRate });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemCalls });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderCalls });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffCalls,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffCallsColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffCallsPercentage,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffCallsColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemDuration });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderDuration });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffDuration,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffDurationColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffDurationPercentage,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffDurationColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemAmount });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderAmount });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffAmount,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffAmountColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffAmountPercentage,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffAmountColor
                                }
                            });

                            ExcelCellStyle resultStyle = new ExcelCellStyle();

                            switch (item.Entity.Result)
                            {
                                case ComparisonResult.Identical:
                                    resultStyle.Color = LabelColor.Success;
                                    break;
                                case ComparisonResult.MissingProvider:
                                case ComparisonResult.MissingSystem:
                                case ComparisonResult.MajorDiff:
                                    resultStyle.Color = LabelColor.Failed;
                                    break;
                                case ComparisonResult.MinorDiff:
                                    resultStyle.Color = LabelColor.Warning;
                                    break;
                            }

                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.ResultDescription,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.ResultColor
                                }
                            });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #endregion

        #region SMS Comparison

        public IDataRetrievalResult<InvoiceComparisonSMSResultDetail> CompareSMSInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonSMSInput> input)
        {
            if (input.ResultKey == null)
            {
                ValidateCompareSMSInvoices(input.Query);
            }
            return BigDataManager.Instance.RetrieveData(input, new InvoiceComparisonSMSRequestHandler());
        }

        public static InvoiceComparisonSMSResultDetail InvoiceComparisonSMSMapper(InvoiceComparisonSMSResult invoiceComparisonResult, CompareInvoiceAction compareInvoiceAction)
        {

            string description = null;
            string resultTooltipDescription = null;

            switch (invoiceComparisonResult.Result)
            {
                case ComparisonResult.Identical:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    break;
                case ComparisonResult.MajorDiff:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Difference greater than threshold.";
                    break;
                case ComparisonResult.MinorDiff:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Difference less than threshold.";
                    break;
                case ComparisonResult.MissingProvider:
                    description = string.Format("{0} {1}", Utilities.GetEnumDescription(invoiceComparisonResult.Result), compareInvoiceAction.PartnerLabel);
                    resultTooltipDescription = "Record doesn't exist for provider.";
                    break;
                case ComparisonResult.MissingSystem:
                    description = Utilities.GetEnumDescription(invoiceComparisonResult.Result);
                    resultTooltipDescription = "Record doesn't exist for system.";
                    break;
            }

            return new InvoiceComparisonSMSResultDetail
            {
                Entity = invoiceComparisonResult,
                ResultDescription = description,
                ResultTooltipDescription = resultTooltipDescription
            };
        }

        private void ValidateCompareSMSInvoices(InvoiceComparisonSMSInput input)
        {
            CompareSMSInvoices(input);
        }

        public List<InvoiceComparisonSMSResult> CompareSMSInvoices(InvoiceComparisonSMSInput input)
        {
            if (input.ListMapping == null || input.ListMapping.FieldMappings == null || input.ListMapping.FieldMappings.Count == 0)
                return null;

            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(input.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", input.InvoiceTypeId);
            var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.InvoiceTypeId, input.InvoiceActionId);
            invoiceAction.ThrowIfNull("invoiceAction");
            var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
            invoiceActionSettings.ThrowIfNull("invoiceActionSettings");

            var invoiceActionSMSSettings = invoiceActionSettings.SMSSettings;
            invoiceActionSMSSettings.ThrowIfNull("invoiceActionSMSSettings");

            var itemGrouping = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == invoiceActionSMSSettings.ItemGroupingId);
            var longPrecision = new GeneralSettingsManager().GetLongPrecisionValue();
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var invoiceItems = invoiceItemManager.GetInvoiceItemsByItemSetNames(input.InvoiceId, new List<String> { itemGrouping.ItemSetName }, CompareOperator.Equal);
            InvoiceItemGroupingManager invoiceItemGroupingManager = new InvoiceItemGroupingManager();
            var invoice = new Vanrise.Invoice.Business.InvoiceManager().GetInvoice(input.InvoiceId);
            invoice.ThrowIfNull("invoice", input.InvoiceId);
            //   var currencyId = invoice.Details.GetType().GetProperty(invoiceType.Settings.CurrencyFieldName).GetValue(invoice.Details, null);
            //  string currencySymbol = null;
            //  if (currencyId != null)
            //     currencySymbol = new CurrencyManager().GetCurrencySymbol(currencyId);

            List<Guid> dimensions = new List<Guid>();
            dimensions.Add(invoiceActionSMSSettings.MobileNetworkDimensionId);
            //  dimensions.Add(invoiceActionSettings.FromDateDimensionId);
            //  dimensions.Add(invoiceActionSettings.ToDateDimensionId);
            dimensions.Add(invoiceActionSMSSettings.CurrencyDimensionId);

            dimensions.Add(invoiceActionSMSSettings.RateDimensionId);

            List<Guid> measures = new List<Guid>();
            measures.Add(invoiceActionSMSSettings.AmountMeasureId);
            measures.Add(invoiceActionSMSSettings.NumberOfSMSsMeasureId);
            measures.Add(invoiceActionSMSSettings.RateMeasureId);

            measures.Add(invoiceActionSMSSettings.FromDateMeasureId);
            measures.Add(invoiceActionSMSSettings.ToDateMeasureId);


            GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
            {
                DimensionIds = dimensions,
                MeasureIds = measures,
                InvoiceTypeId = input.InvoiceTypeId,
                ItemGroupingId = invoiceActionSMSSettings.ItemGroupingId
            };
            var groupedItems = invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);

            var systemInvoiceItems = ConverGroupingItemsToSMSComparisonResult(groupedItems, invoiceActionSettings, itemGrouping, input.DecimalDigits, longPrecision);

            ExcelConvertor excelConvertor = new ExcelConvertor();

            ExcelConversionSettings excelConversionSettings = new ExcelConversionSettings();
            excelConversionSettings.DateTimeFormat = input.DateTimeFormat;
            excelConversionSettings.ListMappings = new List<ListMapping>();
            excelConversionSettings.ListMappings.Add(input.ListMapping);

            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(input.InputFileId, excelConversionSettings, true, false);
            return ProccessInvoiceComparisonVoiceResult(convertedExcel, systemInvoiceItems, input.Threshold, input.DateTimeFormat, input.ComparisonCriterias, input.DecimalDigits, longPrecision);

        }

        private List<InvoiceComparisonSMSResult> ProccessInvoiceComparisonVoiceResult(ConvertedExcel convertedExcel, Dictionary<SMSComparisonKey, SMSInvoiceItemToCompare> systemInvoiceItems, decimal threshold, string dateTimeFormat, List<SMSComparisonCriteria> comparisonCriterias, int? decimalDigits, int longPrecision)
        {
            List<InvoiceComparisonSMSResult> invoiceComparisonResults = new List<InvoiceComparisonSMSResult>();
            ConvertedExcelList convertedExcelList;
            if (convertedExcel.Lists.TryGetValue("SMSMainList", out convertedExcelList))
            {
                foreach (var obj in convertedExcelList.Records)
                {

                    InvoiceComparisonSMSResult invoiceComparisonResult = new InvoiceComparisonSMSResult();
                    FillInvoiceComparisonSMSProviderFields(invoiceComparisonResult, obj.Fields, dateTimeFormat, decimalDigits, longPrecision);

                    SMSComparisonKey comparisonKey = new SMSComparisonKey
                    {
                        MobileNetwork = invoiceComparisonResult.MobileNetwork,
                        From = invoiceComparisonResult.From.Date,
                        To = invoiceComparisonResult.To.Date,
                        Currency = invoiceComparisonResult.Currency
                        //,
                        //Rate = invoiceComparisonResult.ProviderRate
                    };

                    SMSInvoiceItemToCompare invoiceItemToCompare;
                    if (systemInvoiceItems.TryGetValue(comparisonKey, out invoiceItemToCompare))
                    {
                        invoiceComparisonResult.SystemAmount = invoiceItemToCompare.Amount;
                        invoiceComparisonResult.SystemSMSs = invoiceItemToCompare.SMSs;
                        invoiceComparisonResult.SystemRate = invoiceItemToCompare.Rate;
                        invoiceComparisonResult.Currency = _currencyManager.GetCurrencySymbol(invoiceItemToCompare.CurrencyId);
                        LabelColor resultLabelColor;
                        invoiceComparisonResult.Result = GetInvoiceComparisonSMSResult(threshold, invoiceItemToCompare.Amount, invoiceItemToCompare.SMSs, invoiceComparisonResult.ProviderAmount, invoiceComparisonResult.ProviderSMSs, comparisonCriterias, out resultLabelColor);
                        invoiceComparisonResult.ResultColor = resultLabelColor;
                        systemInvoiceItems.Remove(comparisonKey);
                    }
                    else
                    {
                        // invoiceComparisonResult.Currency = currencySymbol;
                        invoiceComparisonResult.Result = ComparisonResult.MissingSystem;
                        invoiceComparisonResult.ResultColor = LabelColor.Error;
                    }

                    invoiceComparisonResult.DiffSMSsPercentage = CalculateDiffPercentageValue(invoiceComparisonResult.SystemSMSs, invoiceComparisonResult.ProviderSMSs);
                    invoiceComparisonResult.DiffSMSs = CalculateDiffValue(invoiceComparisonResult.SystemSMSs, invoiceComparisonResult.ProviderSMSs);
                    invoiceComparisonResult.DiffSMSsColor = GetDiffLabelColor(invoiceComparisonResult.DiffSMSsPercentage, threshold);

                    invoiceComparisonResult.DiffAmountPercentage = CalculateDiffPercentageValue(invoiceComparisonResult.SystemAmount, invoiceComparisonResult.ProviderAmount);
                    invoiceComparisonResult.DiffAmountColor = GetDiffLabelColor(invoiceComparisonResult.DiffAmountPercentage, threshold);
                    invoiceComparisonResult.DiffAmount = CalculateDiffValue(invoiceComparisonResult.SystemAmount, invoiceComparisonResult.ProviderAmount);

                    invoiceComparisonResults.Add(invoiceComparisonResult);
                }
                AddMissingSystemInvoices(invoiceComparisonResults, systemInvoiceItems);
            }
            return invoiceComparisonResults;
        }
        private ComparisonResult GetInvoiceComparisonSMSResult(decimal threshold, decimal sysAmount, decimal sysNCalls, decimal? providerAmount, decimal? providerNCalls, List<SMSComparisonCriteria> comparisonCriterias, out LabelColor resultLabelColor)
        {
            if (SMSCheckIfMajorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, providerAmount, providerNCalls))
            {
                resultLabelColor = LabelColor.Error;
                return ComparisonResult.MajorDiff;
            }
            if (SMSCheckIfMinorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, providerAmount, providerNCalls))
            {
                resultLabelColor = LabelColor.Warning;
                return ComparisonResult.MinorDiff;
            }
            resultLabelColor = LabelColor.Success;
            return ComparisonResult.Identical;

        }
        private bool SMSCheckIfMajorErrorDiff(List<SMSComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNSMSs, decimal? providerAmount, decimal? providerNSMSs)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case SMSComparisonCriteria.NoOfSMS:
                            if (CheckThresholdPercentage(sysNSMSs, providerNSMSs, threshold))
                            {

                                return true;
                            }
                            break;
                        case SMSComparisonCriteria.Amount:
                            if (CheckThresholdPercentage(sysAmount, providerAmount, threshold))
                            {

                                return true;
                            }
                            break;
                    }
                }
            }
            else
            {
                if (CheckThresholdPercentage(sysAmount, providerAmount, threshold)
                   || CheckThresholdPercentage(sysNSMSs, providerNSMSs, threshold))
                {
                    return true;
                }
            }

            return false;
        }
        private bool SMSCheckIfMinorErrorDiff(List<SMSComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNSMSs, decimal? providerAmount, decimal? providerNSMSs)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case SMSComparisonCriteria.NoOfSMS:
                            if (providerNSMSs.HasValue && sysNSMSs != providerNSMSs.Value)
                                return true;
                            break;
                        case SMSComparisonCriteria.Amount:
                            if (providerAmount.HasValue && sysAmount != providerAmount.Value)
                                return true;
                            break;
                    }
                }
            }
            else
            {
                if ((providerAmount.HasValue && sysAmount != providerAmount.Value)
                                       || (providerNSMSs.HasValue && sysNSMSs != providerNSMSs.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<SMSComparisonKey, SMSInvoiceItemToCompare> ConverGroupingItemsToSMSComparisonResult(IEnumerable<GroupingInvoiceItemDetail> groupedItems, CompareInvoiceAction compareInvoiceAction, ItemGrouping itemGrouping, int? decimalDigits, int longPrecision)
        {
            Dictionary<SMSComparisonKey, SMSInvoiceItemToCompare> invoiceItemsToCompare = new Dictionary<SMSComparisonKey, SMSInvoiceItemToCompare>();
            if (groupedItems != null)
            {

                AggregateItemField amountMeasure = null;
                AggregateItemField smssMeasure = null;
                AggregateItemField rateMeasure = null;
                AggregateItemField fromDateMeasure = null;
                AggregateItemField toDateMeasure = null;
                foreach (var measure in itemGrouping.AggregateItemFields)
                {
                    if (measure.AggregateItemFieldId == compareInvoiceAction.SMSSettings.AmountMeasureId)
                    {
                        amountMeasure = measure;
                    }

                    else if (measure.AggregateItemFieldId == compareInvoiceAction.SMSSettings.NumberOfSMSsMeasureId)
                    {
                        smssMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.SMSSettings.RateMeasureId)
                    {
                        rateMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.SMSSettings.FromDateMeasureId)
                    {
                        fromDateMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.SMSSettings.ToDateMeasureId)
                    {
                        toDateMeasure = measure;
                    }
                }


                foreach (var item in groupedItems)
                {
                    var mobileNetworkDimension = item.DimensionValues[0];
                    //var fromDateDimension = item.DimensionValues[1];
                    //var toDateDimension = item.DimensionValues[2];
                    var currecnyDimension = item.DimensionValues[1];
                    SMSInvoiceItemToCompare invoiceItemToCompare = new SMSInvoiceItemToCompare
                    {
                        MobileNetwork = mobileNetworkDimension != null ? mobileNetworkDimension.Name : null,
                        CurrencyId = currecnyDimension != null ? Convert.ToInt32(currecnyDimension.Value) : default(int),
                    };

                    InvoiceGroupingMeasureValue amountMeasureItem;
                    InvoiceGroupingMeasureValue smssMeasureItem;
                    InvoiceGroupingMeasureValue rateMeasureItem;


                    InvoiceGroupingMeasureValue fromDateMeasureItem;
                    InvoiceGroupingMeasureValue toDateMeasureItem;


                    if (amountMeasure != null && item.MeasureValues.TryGetValue(amountMeasure.FieldName, out amountMeasureItem))
                    {
                        invoiceItemToCompare.Amount = amountMeasureItem.Value != null ? Convert.ToDecimal(amountMeasureItem.Value) : default(decimal);
                    }

                    if (smssMeasure != null && item.MeasureValues.TryGetValue(smssMeasure.FieldName, out smssMeasureItem))
                    {
                        invoiceItemToCompare.SMSs = smssMeasureItem.Value != null ? Convert.ToInt32(smssMeasureItem.Value) : default(Int32);
                    }

                    if (rateMeasure != null && item.MeasureValues.TryGetValue(rateMeasure.FieldName, out rateMeasureItem))
                    {
                        invoiceItemToCompare.Rate = rateMeasureItem.Value != null ? Convert.ToDecimal(rateMeasureItem.Value) : default(decimal);
                    }

                    if (fromDateMeasure != null && item.MeasureValues.TryGetValue(fromDateMeasure.FieldName, out fromDateMeasureItem))
                    {
                        invoiceItemToCompare.From = fromDateMeasureItem.Value != null ? Convert.ToDateTime(fromDateMeasureItem.Value) : default(DateTime);
                    }

                    if (toDateMeasure != null && item.MeasureValues.TryGetValue(toDateMeasure.FieldName, out toDateMeasureItem))
                    {
                        invoiceItemToCompare.To = toDateMeasureItem.Value != null ? Convert.ToDateTime(toDateMeasureItem.Value) : default(DateTime);
                    }


                    invoiceItemToCompare.Rate = Math.Round(invoiceItemToCompare.Rate, longPrecision, MidpointRounding.AwayFromZero);
                    if (decimalDigits.HasValue)
                    {
                        invoiceItemToCompare.Amount = Math.Round(invoiceItemToCompare.Amount, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }

                    var comparisonKey = new SMSComparisonKey
                    {
                        MobileNetwork = invoiceItemToCompare.MobileNetwork,
                        From = invoiceItemToCompare.From.Date,
                        To = invoiceItemToCompare.To.Date,
                        Currency = _currencyManager.GetCurrencySymbol(invoiceItemToCompare.CurrencyId)
                    };
                    if (!invoiceItemsToCompare.ContainsKey(comparisonKey))
                    {
                        invoiceItemsToCompare.Add(comparisonKey, invoiceItemToCompare);
                    }
                    else
                    {
                        throw new Exception(string.Format("Same Period Exist for {0} From: '{1}' To: '{2}'. ", invoiceItemToCompare.MobileNetwork, invoiceItemToCompare.From.Date, invoiceItemToCompare.To.Date));
                    }
                }

            }
            return invoiceItemsToCompare;
        }
        private void AddMissingSystemInvoices(List<InvoiceComparisonSMSResult> invoiceComparisonResults, Dictionary<SMSComparisonKey, SMSInvoiceItemToCompare> systemInvoiceItems)
        {
            if (systemInvoiceItems.Count > 0)
            {
                foreach (var item in systemInvoiceItems)
                {
                    var invoiceComparisonResult = new InvoiceComparisonSMSResult
                    {
                        SystemSMSs = item.Value.SMSs,
                        SystemRate = item.Value.Rate,
                        SystemAmount = item.Value.Amount,
                        Currency = _currencyManager.GetCurrencySymbol(item.Value.CurrencyId),
                        MobileNetwork = item.Value.MobileNetwork,
                        From = item.Value.From,
                        To = item.Value.To,
                        Result = ComparisonResult.MissingProvider,
                        ResultColor = LabelColor.Error
                    };
                    invoiceComparisonResults.Add(invoiceComparisonResult);
                }
            }
        }
        private void FillInvoiceComparisonSMSProviderFields(InvoiceComparisonSMSResult invoiceComparisonResult, ConvertedExcelFieldsByName fields, string dateTimeFormat, int? decimalDigits, int longPrecision)
        {

            #region Fill Destination
            ConvertedExcelField mobileNetworkField;
            if (fields.TryGetValue("MobileNetwork", out mobileNetworkField))
            {
                if (mobileNetworkField.FieldValue == null || String.IsNullOrWhiteSpace(mobileNetworkField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped Mobile Network mobileNetworkField have a null value.");
                invoiceComparisonResult.MobileNetwork = mobileNetworkField.FieldValue.ToString();
            };


            #endregion

            ConvertedExcelField currency;
            if (fields.TryGetValue("Currency", out currency))
            {
                if (currency.FieldValue == null || String.IsNullOrWhiteSpace(currency.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped Currency cannot have a null value.");
                invoiceComparisonResult.Currency = currency.FieldValue.ToString();
            };
            #region Fill FromDate
            ConvertedExcelField fromDateField;
            if (fields.TryGetValue("FromDate", out fromDateField))
            {
                if (fromDateField.FieldValue == null || String.IsNullOrWhiteSpace(fromDateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped from date cannot have a null value.");

                if (fromDateField.FieldValue is DateTime)
                {
                    invoiceComparisonResult.From = Convert.ToDateTime(fromDateField.FieldValue).Date;
                }
                else
                {
                    DateTime fromDate;
                    if (DateTime.TryParseExact(fromDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out fromDate))
                        invoiceComparisonResult.From = fromDate.Date;
                    else
                        throw new NullReferenceException("From date has an invalid format.");
                }

            };
            #endregion

            #region Fill ToDate

            ConvertedExcelField toDateField;
            if (fields.TryGetValue("ToDate", out toDateField))
            {
                if (toDateField.FieldValue == null || String.IsNullOrWhiteSpace(toDateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped to date cannot have a null value.");

                if (toDateField.FieldValue is DateTime)
                {
                    invoiceComparisonResult.To = Convert.ToDateTime(toDateField.FieldValue).Date;
                }
                else
                {
                    DateTime toDate;
                    if (DateTime.TryParseExact(toDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out toDate))
                        invoiceComparisonResult.To = toDate.Date;
                    else
                        throw new NullReferenceException("To date has an invalid format.");
                }

            };
            #endregion

            #region Fill Rate

            ConvertedExcelField rateField;
            if (fields.TryGetValue("Rate", out rateField))
            {
                if (rateField.FieldValue == null || String.IsNullOrWhiteSpace(rateField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped rate cannot have a null value.");

                Decimal rate;
                if (Decimal.TryParse(rateField.FieldValue.ToString(), out rate))
                {
                    invoiceComparisonResult.ProviderRate = Math.Round(rate, longPrecision, MidpointRounding.AwayFromZero);
                }
                else
                    throw new NullReferenceException("Rate has an invalid format.");

            };

            #endregion

            #region Fill NumberOfCalls

            ConvertedExcelField smssField;
            if (fields.TryGetValue("NumberOfSMSs", out smssField))
            {
                if (smssField.FieldValue == null || String.IsNullOrWhiteSpace(smssField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped number of smss cannot have a null value.");
                Decimal providerCalls;
                if (Decimal.TryParse(smssField.FieldValue.ToString(), out providerCalls))
                {
                    if (decimalDigits.HasValue)
                    {
                        invoiceComparisonResult.ProviderSMSs = Math.Round(providerCalls, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }
                    else
                        invoiceComparisonResult.ProviderSMSs = providerCalls;
                }
                else
                    throw new NullReferenceException("Number of smss has an invalid format.");
            };
            #endregion

            #region Fill Amount

            ConvertedExcelField amountField;
            if (fields.TryGetValue("Amount", out amountField))
            {
                if (amountField.FieldValue == null || String.IsNullOrWhiteSpace(amountField.FieldValue.ToString()))
                    throw new NullReferenceException("Mapped amount cannot have a null value.");
                Decimal providerAmount;
                if (Decimal.TryParse(amountField.FieldValue.ToString(), out providerAmount))
                {
                    if (decimalDigits.HasValue)
                    {
                        invoiceComparisonResult.ProviderAmount = Math.Round(providerAmount, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }
                    else
                        invoiceComparisonResult.ProviderAmount = providerAmount;
                }
                else
                    throw new NullReferenceException("Amount has an invalid format.");

            };
            #endregion
        }

        private struct SMSComparisonKey
        {
            public string MobileNetwork { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string Currency { get; set; }
        }
        private class SMSInvoiceItemToCompare
        {
            public int CurrencyId { get; set; }
            public string MobileNetwork { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public Decimal Rate { get; set; }
            public Decimal Amount { get; set; }
            public int SMSs { get; set; }
        }

        #region Private Classes

        private class InvoiceComparisonSMSRequestHandler : BigDataRequestHandler<InvoiceComparisonSMSInput, InvoiceComparisonSMSResult, InvoiceComparisonSMSResultDetail>
        {
            public InvoiceComparisonSMSRequestHandler()
            {

            }
            public override InvoiceComparisonSMSResultDetail EntityDetailMapper(InvoiceComparisonSMSResult entity)
            {
                return null;
            }
            protected override Vanrise.Entities.BigResult<InvoiceComparisonSMSResultDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonSMSInput> input, IEnumerable<InvoiceComparisonSMSResult> allRecords)
            {

                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.Query.InvoiceTypeId, input.Query.InvoiceActionId);
                invoiceAction.ThrowIfNull("invoiceAction");
                var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
                invoiceActionSettings.ThrowIfNull("invoiceActionSettings");
                Func<InvoiceComparisonSMSResult, bool> filterExpression = (invoiceComparisonResult) =>
                {
                    if (input.Query.ComparisonResults != null)
                    {
                        if (!input.Query.ComparisonResults.Contains(invoiceComparisonResult.Result))
                            return false;
                    }
                    return true;
                };

                return allRecords.ToBigResult(input, filterExpression, (entity) => InvoiceComparisonSMSMapper(entity, invoiceActionSettings));
            }
            public override IEnumerable<InvoiceComparisonSMSResult> RetrieveAllData(DataRetrievalInput<InvoiceComparisonSMSInput> input)
            {
                return new InvoiceCompareManager().CompareSMSInvoices(input.Query);
            }
            protected override ResultProcessingHandler<InvoiceComparisonSMSResultDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceComparisonSMSInput> input, BigResult<InvoiceComparisonSMSResultDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceComparisonSMSResultDetail>
                {
                    ExportExcelHandler = new InvoiceComparisonSMSExcelExportHandler(input.Query)
                };
            }

        }

        private class InvoiceComparisonSMSExcelExportHandler : ExcelExportHandler<InvoiceComparisonSMSResultDetail>
        {
            InvoiceComparisonSMSInput _query;
            public InvoiceComparisonSMSExcelExportHandler(InvoiceComparisonSMSInput query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvoiceComparisonSMSResultDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Invoice Comparison",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceAction = invoiceTypeManager.GetInvoiceAction(_query.InvoiceTypeId, _query.InvoiceActionId);
                invoiceAction.ThrowIfNull("invoiceAction");
                var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
                invoiceActionSettings.ThrowIfNull("invoiceActionSettings");

                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Destination",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "From",
                    CellType = ExcelCellType.DateTime,
                    DateTimeType = DateTimeType.Date
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Till",
                    CellType = ExcelCellType.DateTime,
                    DateTimeType = DateTimeType.Date
                });
                //sheet.Header.Cells.Add(new ExportExcelHeaderCell
                //{
                //    Title = "Curr.",
                //               });

                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Rate",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Rate", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Calls",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Calls", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Calls",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Calls %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Duration",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Duration", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Duration",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Duration %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Sys. Amount",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = string.Format("{0} Amount", invoiceActionSettings.PartnerLabel),
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Amount",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Diff. Amount %",
                });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Result",
                });

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<InvoiceComparisonSMSResultDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach (var item in results.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.MobileNetwork });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.From });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.To });
                            //  row.Cells.Add(new ExportExcelCell { Value =  item.Entity.Currency });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemRate });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderRate });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemSMSs });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffSMSs,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffSMSsColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffSMSsPercentage,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffSMSsColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemAmount });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderAmount });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffAmount,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffAmountColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.Entity.DiffAmountPercentage,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffAmountColor
                                }
                            });

                            ExcelCellStyle resultStyle = new ExcelCellStyle();

                            switch (item.Entity.Result)
                            {
                                case ComparisonResult.Identical:
                                    resultStyle.Color = LabelColor.Success;
                                    break;
                                case ComparisonResult.MissingProvider:
                                case ComparisonResult.MissingSystem:
                                case ComparisonResult.MajorDiff:
                                    resultStyle.Color = LabelColor.Failed;
                                    break;
                                case ComparisonResult.MinorDiff:
                                    resultStyle.Color = LabelColor.Warning;
                                    break;
                            }

                            row.Cells.Add(new ExportExcelCell
                            {
                                Value = item.ResultDescription,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.ResultColor
                                }
                            });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #endregion

        #region Common Methods
        public bool DoesUserUserHaveCompareInvoiceAccess(Guid invoiceTypeId)
        {
            return new InvoiceTypeManager().DoesUserHaveViewAccess(invoiceTypeId);
        }

        private bool CheckThresholdPercentage(decimal sysValue, decimal? provValue, decimal threshold)
        {
            if (provValue.HasValue)
            {
                if (sysValue == provValue)
                    return false;
                if (sysValue == 0)
                {
                    return Math.Abs(provValue.Value - sysValue) * 100 / provValue.Value > threshold;
                }
                return Math.Abs(sysValue - provValue.Value) * 100 / sysValue > threshold;
            }
            return false;
        }
        private LabelColor? GetDiffLabelColor(decimal? diffValue, decimal threshold)
        {
            if (diffValue.HasValue && diffValue.Value != 0)
            {
                if (Math.Abs(diffValue.Value) > threshold)
                {
                    return LabelColor.Error;
                }
                else
                {
                    return LabelColor.Warning;
                }
            }
            return null;
        }
        private decimal? CalculateDiffPercentageValue(decimal? sysVal, decimal? provideVal)
        {
            if (sysVal.HasValue && sysVal.Value != 0 && provideVal.HasValue && provideVal.Value != 0)
            {
                return (sysVal.Value - provideVal.Value) * 100 / sysVal.Value;
            }
            return null;
        }
        private decimal? CalculateDiffValue(decimal? sysVal, decimal? provideVal)
        {
            if (sysVal.HasValue && sysVal.Value != 0 && provideVal.HasValue && provideVal.Value != 0)
            {
                return (sysVal.Value - provideVal.Value);
            }
            return null;
        }

        #endregion
    }
}
