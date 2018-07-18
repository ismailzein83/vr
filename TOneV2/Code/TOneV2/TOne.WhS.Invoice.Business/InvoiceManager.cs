using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Business.Extensions;
using TOne.WhS.Invoice.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Common.Business;
using System.Globalization;
namespace TOne.WhS.Invoice.Business
{
    public class InvoiceManager
    {
        CurrencyManager _currencyManager = new CurrencyManager();
        public IDataRetrievalResult<InvoiceComparisonResultDetail> CompareInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonInput> input)
        {
            if(input.ResultKey == null)
            {
                ValidateCompareInvoices(input.Query);
            }
            return BigDataManager.Instance.RetrieveData(input, new InvoiceComparisonRequestHandler());
        }

        public static InvoiceComparisonResultDetail InvoiceComparisonMapper(InvoiceComparisonResult invoiceComparisonResult, CompareInvoiceAction compareInvoiceAction)
        {

            string description = null;
            string resultTooltipDescription = null;

            switch(invoiceComparisonResult.Result)
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

            return new InvoiceComparisonResultDetail
            {
                Entity = invoiceComparisonResult,
                ResultDescription = description,
                ResultTooltipDescription = resultTooltipDescription
            };
        }
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId)
        {

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoiceDetail(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);
            var supplierInvoiceDetails = (SupplierInvoiceDetails)invoice.Entity.Details;         
            supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
            string timezone = "";
            if(supplierInvoiceDetails.TimeZoneId != null)
            {
                Vanrise.Entities.VRTimeZone timeZone = new Vanrise.Common.Business.VRTimeZoneManager().GetVRTimeZone((int)supplierInvoiceDetails.TimeZoneId);
                timezone = timeZone.Name;
            }
          
            return new ComparisonInvoiceDetail()
            {
                To = invoice.PartnerName,
                ToDate = invoice.Entity.ToDate,
                DueDate = invoice.Entity.DueDate,
                Calls = invoice.Entity.Details.TotalNumberOfCalls,
                FromDate = invoice.Entity.FromDate,
                IssuedDate = invoice.Entity.IssueDate,
                SerialNumber = invoice.Entity.SerialNumber,
                Duration = invoice.Entity.Details.Duration,
                TotalAmount = invoice.Entity.Details.TotalAmount,
                IsLocked = invoice.Lock,
                IsPaid = invoice.Paid,
                IssuedBy = invoice.UserName,
                PartnerId = invoice.Entity.PartnerId,
                Currency = invoice.Entity.Details.SupplierCurrency,
                TimeZone = timezone
            };
        }

        public bool UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(input.InvoiceId);
            invoice.ThrowIfNull("invoice", input.InvoiceId);

            if (input.AttachementFiles != null)
            {
                VRFileManager vrFileManager = new VRFileManager();
                foreach (var attachment in input.AttachementFiles)
                {
                    if (!vrFileManager.SetFileUsedAndUpdateSettings(attachment.FileId, new VRFileSettings { ExtendedSettings = new OriginalInvoiceDataFileSetting { InvoiceId = input.InvoiceId, InvoiceTypeId = invoice.InvoiceTypeId} }))
                    {
                        return false;
                    }
                }
            }
            InvoiceCarrierType invoiceCarrierType = input.invoiceCarrierType;
           switch (invoiceCarrierType)
           {
               case InvoiceCarrierType.Customer:
                      var customerInvoiceDetails = invoice.Details as CustomerInvoiceDetails;
                      customerInvoiceDetails.ThrowIfNull("customerInvoiceDetails");
                      customerInvoiceDetails.AttachementFiles = input.AttachementFiles;
                      customerInvoiceDetails.Reference = input.Reference;
                      customerInvoiceDetails.OriginalAmountByCurrency = input.OriginalDataCurrency;
              
                      invoice.Details = customerInvoiceDetails;
                   break;
               case InvoiceCarrierType.Supplier:
                     var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
                     supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                     supplierInvoiceDetails.AttachementFiles = input.AttachementFiles;
                     supplierInvoiceDetails.Reference = input.Reference;
                     supplierInvoiceDetails.OriginalAmountByCurrency = input.OriginalDataCurrency;

                     invoice.Details = supplierInvoiceDetails;
                   break;

           }
          

            
            return invoiceManager.TryUpdateInvoice(invoice);
        }


        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);


            string reference = null;
            Dictionary<int, OriginalDataCurrrency> originalAmountByCurrency = null;
            List<AttachementFile> attachementFiles = null;
            switch (invoiceCarrierType)
            {
                 case InvoiceCarrierType.Customer :
                    var customerInvoiceDetails = invoice.Details as CustomerInvoiceDetails;
                    customerInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                    reference = customerInvoiceDetails.Reference;
                    originalAmountByCurrency = customerInvoiceDetails.OriginalAmountByCurrency;
                    attachementFiles = customerInvoiceDetails.AttachementFiles;

                   break;

                case InvoiceCarrierType.Supplier:
                    var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
                    supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
                    reference = supplierInvoiceDetails.Reference;
                    originalAmountByCurrency = supplierInvoiceDetails.OriginalAmountByCurrency;
                    attachementFiles = supplierInvoiceDetails.AttachementFiles;
                    break;
            }


            var originalInvoiceDataRuntime = new OriginalInvoiceDataRuntime
              {
                  Reference = reference,
              };
            CurrencyManager currencyManager = new CurrencyManager();

            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var currencyItemSetName = invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceId, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
            if (currencyItemSetName != null)
            {
                originalInvoiceDataRuntime.OriginalDataCurrency = new Dictionary<int, OriginalDataCurrrency>();
                foreach (var item in currencyItemSetName)
                {
                    var itemDetails = item.Details as InvoiceBySaleCurrencyItemDetails;
                    if (itemDetails != null)
                    {
                        var currencySymbol = currencyManager.GetCurrencySymbol(itemDetails.CurrencyId);
                        currencySymbol.ThrowIfNull("currencySymbol", itemDetails.CurrencyId);
                        var originalDataCurrrency = originalInvoiceDataRuntime.OriginalDataCurrency.GetOrCreateItem(itemDetails.CurrencyId);
                        originalDataCurrrency.CurrencySymbol = currencySymbol;
                    }
                }
            }


            if (originalAmountByCurrency != null)
            {
                foreach (var originalAmount in originalAmountByCurrency)
                {
                    var record = originalInvoiceDataRuntime.OriginalDataCurrency.GetRecord(originalAmount.Key);
                    if (record != null)
                    {
                        record.OriginalAmount = originalAmount.Value.OriginalAmount;
                        record.IncludeOriginalAmountInSettlement = originalAmount.Value.IncludeOriginalAmountInSettlement;
                    }
                }
            }

            if (attachementFiles != null && attachementFiles.Count > 0)
            {
                originalInvoiceDataRuntime.AttachementFilesRuntime = new List<AttachementFileRuntime>();
                var fileIds = attachementFiles.Select(x => x.FileId);
                VRFileManager vrFileManager = new VRFileManager();
                var files = vrFileManager.GetFilesInfo(fileIds);
                foreach (var attachementFile in attachementFiles)
                {
                    if (files != null)
                    {
                        var file = files.FindRecord(x => x.FileId == attachementFile.FileId);
                        if (file != null)
                        {
                            originalInvoiceDataRuntime.AttachementFilesRuntime.Add(new AttachementFileRuntime
                            {
                                FileId = file.FileId,
                                FileName = file.Name
                            });
                        }
                    }
                }
            }
            return originalInvoiceDataRuntime;
        }


        public bool DoesUserUserHaveCompareInvoiceAccess(Guid invoiceTypeId)
        {
            return new InvoiceTypeManager().DoesUserHaveViewAccess(invoiceTypeId);
        }

        public bool DoesUserHaveUpdateOriginalInvoiceDataAccess(long invoiceId)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().DoesUserHaveGenerateAccess(invoiceId);

        }

        private void ValidateCompareInvoices(InvoiceComparisonInput input)
        {
            CompareInvoices(input);
        }
        private List<InvoiceComparisonResult> CompareInvoices(InvoiceComparisonInput input)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(input.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", input.InvoiceTypeId);
            var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.InvoiceTypeId, input.InvoiceActionId);
            invoiceAction.ThrowIfNull("invoiceAction");
            var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
            invoiceActionSettings.ThrowIfNull("invoiceActionSettings");

            var itemGrouping = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == invoiceActionSettings.ItemGroupingId);
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
            dimensions.Add(invoiceActionSettings.ZoneDimensionId);
            dimensions.Add(invoiceActionSettings.FromDateDimensionId);
            dimensions.Add(invoiceActionSettings.ToDateDimensionId);
            dimensions.Add(invoiceActionSettings.CurrencyDimensionId);

           // dimensions.Add(invoiceActionSettings.RateDimensionId);

            List<Guid> measures = new List<Guid>();
            measures.Add(invoiceActionSettings.AmountMeasureId);
            measures.Add(invoiceActionSettings.DurationMeasureId);
            measures.Add(invoiceActionSettings.NumberOfCallsMeasureId);
            measures.Add(invoiceActionSettings.RateMeasureId);

            GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
            {
                DimensionIds = dimensions,
                MeasureIds = measures,
                InvoiceTypeId = input.InvoiceTypeId,
                ItemGroupingId = invoiceActionSettings.ItemGroupingId
            };
            var groupedItems = invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);

            var systemInvoiceItems = ConverGroupingItemsToComparisonResult(groupedItems, invoiceActionSettings, itemGrouping, input.DecimalDigits, longPrecision);

            ExcelConvertor excelConvertor = new ExcelConvertor();

            ExcelConversionSettings excelConversionSettings = new ExcelConversionSettings();
            excelConversionSettings.DateTimeFormat = input.DateTimeFormat;
            excelConversionSettings.ListMappings = new List<ListMapping>();
            excelConversionSettings.ListMappings.Add(input.ListMapping);

            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(input.InputFileId, excelConversionSettings, true, false);
            return ProccessInvoiceComparisonResult(convertedExcel, systemInvoiceItems, input.Threshold, input.DateTimeFormat, input.ComparisonCriterias, input.DecimalDigits, longPrecision);


        }
        private List<InvoiceComparisonResult> ProccessInvoiceComparisonResult(ConvertedExcel convertedExcel, Dictionary<ComparisonKey, InvoiceItemToCompare> systemInvoiceItems, decimal threshold, string dateTimeFormat, List<ComparisonCriteria> comparisonCriterias, int? decimalDigits, int longPrecision)
        {
            List<InvoiceComparisonResult> invoiceComparisonResults = new List<InvoiceComparisonResult>();
            ConvertedExcelList convertedExcelList;
            if (convertedExcel.Lists.TryGetValue("MainList", out convertedExcelList))
            {
                foreach (var obj in convertedExcelList.Records)
                {

                    InvoiceComparisonResult invoiceComparisonResult = new InvoiceComparisonResult();
                    FillInvoiceComparisonProviderFields(invoiceComparisonResult, obj.Fields, dateTimeFormat, decimalDigits, longPrecision);

                    ComparisonKey comparisonKey = new ComparisonKey
                    {
                        Destination = invoiceComparisonResult.Destination,
                        From = invoiceComparisonResult.From.Date,
                        To = invoiceComparisonResult.To.Date,
                    };
                   
                    InvoiceItemToCompare invoiceItemToCompare;
                    if (systemInvoiceItems.TryGetValue(comparisonKey, out invoiceItemToCompare))
                    {
                        invoiceComparisonResult.SystemAmount = invoiceItemToCompare.Amount;
                        invoiceComparisonResult.SystemCalls = invoiceItemToCompare.Calls;
                        invoiceComparisonResult.SystemDuration = invoiceItemToCompare.Duration;
                        invoiceComparisonResult.SystemRate = invoiceItemToCompare.Rate;
                        invoiceComparisonResult.Currency = _currencyManager.GetCurrencySymbol(invoiceItemToCompare.CurrencyId);
                        LabelColor resultLabelColor;
                        invoiceComparisonResult.Result = GetInvoiceComparisonResult(threshold, invoiceItemToCompare.Amount, invoiceItemToCompare.Calls, invoiceItemToCompare.Duration, invoiceComparisonResult.ProviderAmount, invoiceComparisonResult.ProviderCalls, invoiceComparisonResult.ProviderDuration, comparisonCriterias, out resultLabelColor);
                        invoiceComparisonResult.ResultColor = resultLabelColor;
                        systemInvoiceItems.Remove(comparisonKey);
                    }
                   else
                    {
                       // invoiceComparisonResult.Currency = currencySymbol;
                        invoiceComparisonResult.Result = ComparisonResult.MissingSystem;
                        invoiceComparisonResult.ResultColor = LabelColor.Error;
                    }

                    invoiceComparisonResult.DiffCalls = CalculateDiffValue(invoiceComparisonResult.SystemCalls, invoiceComparisonResult.ProviderCalls);
                    invoiceComparisonResult.DiffCallsColor = GetDiffLabelColor(invoiceComparisonResult.DiffCalls, threshold);

                    invoiceComparisonResult.DiffAmount = CalculateDiffValue(invoiceComparisonResult.SystemAmount, invoiceComparisonResult.ProviderAmount);
                    invoiceComparisonResult.DiffAmountColor = GetDiffLabelColor(invoiceComparisonResult.DiffAmount, threshold);

                    invoiceComparisonResult.DiffDuration = CalculateDiffValue(invoiceComparisonResult.SystemDuration, invoiceComparisonResult.ProviderDuration);
                    invoiceComparisonResult.DiffDurationColor = GetDiffLabelColor(invoiceComparisonResult.DiffDuration, threshold);

                    invoiceComparisonResults.Add(invoiceComparisonResult);
                }
                AddMissingSystemInvoices(invoiceComparisonResults, systemInvoiceItems);
            }
            return invoiceComparisonResults;
        }
        private ComparisonResult GetInvoiceComparisonResult(decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration, List<ComparisonCriteria> comparisonCriterias, out LabelColor resultLabelColor)
        {
            if (CheckIfMajorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, sysDuration, providerAmount, providerNCalls, providerDuration))
            {
                resultLabelColor = LabelColor.Error;
                return ComparisonResult.MajorDiff;
            }
            if (CheckIfMinorErrorDiff(comparisonCriterias, threshold, sysAmount, sysNCalls, sysDuration, providerAmount, providerNCalls, providerDuration))
            {
                resultLabelColor = LabelColor.Warning;
                return ComparisonResult.MinorDiff;
            }
            resultLabelColor = LabelColor.Success;
            return ComparisonResult.Identical;

        }
        private bool CheckIfMajorErrorDiff(List<ComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case ComparisonCriteria.Calls:
                            if (CheckThresholdPercentage(sysNCalls, providerNCalls, threshold))
                            {

                                return true;
                            }
                            break;
                        case ComparisonCriteria.Amount:
                            if (CheckThresholdPercentage(sysAmount, providerAmount, threshold))
                            {

                                return true;
                            }
                            break;
                        case ComparisonCriteria.Duration:
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
        private bool CheckIfMinorErrorDiff(List<ComparisonCriteria> comparisonCriterias, decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls, decimal? providerDuration)
        {
            if (comparisonCriterias != null && comparisonCriterias.Count > 0)
            {
                foreach (var item in comparisonCriterias)
                {
                    switch (item)
                    {
                        case ComparisonCriteria.Calls:
                            if (providerNCalls.HasValue && sysNCalls != providerNCalls.Value)
                                return true;
                            break;
                        case ComparisonCriteria.Amount:
                            if (providerAmount.HasValue && sysAmount != providerAmount.Value)
                                return true;
                            break;
                        case ComparisonCriteria.Duration:
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

        private Dictionary<ComparisonKey, InvoiceItemToCompare> ConverGroupingItemsToComparisonResult(IEnumerable<GroupingInvoiceItemDetail> groupedItems, CompareInvoiceAction compareInvoiceAction, ItemGrouping itemGrouping, int? decimalDigits, int longPrecision)
        {
            Dictionary<ComparisonKey, InvoiceItemToCompare> invoiceItemsToCompare = new Dictionary<ComparisonKey, InvoiceItemToCompare>();
            if (groupedItems != null)
            {

                AggregateItemField amountMeasure = null;
                AggregateItemField durationMeasure = null;
                AggregateItemField callsMeasure = null;
                AggregateItemField rateMeasure = null;

                foreach (var measure in itemGrouping.AggregateItemFields)
                {
                    if (measure.AggregateItemFieldId == compareInvoiceAction.AmountMeasureId)
                    {
                        amountMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.DurationMeasureId)
                    {
                        durationMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.NumberOfCallsMeasureId)
                    {
                        callsMeasure = measure;
                    }
                    else if (measure.AggregateItemFieldId == compareInvoiceAction.RateMeasureId)
                    {
                        rateMeasure = measure;
                    }
                }


                foreach (var item in groupedItems)
                {
                    var zoneDimension = item.DimensionValues[0];
                    var fromDateDimension = item.DimensionValues[1];
                    var toDateDimension = item.DimensionValues[2];
                    var currecnyDimension = item.DimensionValues[3];
                    InvoiceItemToCompare invoiceItemToCompare = new InvoiceItemToCompare
                    {
                        Destination = zoneDimension != null ? zoneDimension.Name : null,
                        From = fromDateDimension != null ? Convert.ToDateTime(fromDateDimension.Value) : default(DateTime),
                        To = toDateDimension != null ? Convert.ToDateTime(toDateDimension.Value) : default(DateTime),
                        CurrencyId = currecnyDimension != null ? Convert.ToInt32(currecnyDimension.Value) : default(int),
                    };

                    InvoiceGroupingMeasureValue amountMeasureItem;
                    InvoiceGroupingMeasureValue callsMeasureItem;
                    InvoiceGroupingMeasureValue durationMeasureItem;
                    InvoiceGroupingMeasureValue rateMeasureItem;

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
                    invoiceItemToCompare.Rate = Math.Round(invoiceItemToCompare.Rate, longPrecision,MidpointRounding.AwayFromZero);
                    if (decimalDigits.HasValue)
                    {
                        invoiceItemToCompare.Amount = Math.Round(invoiceItemToCompare.Amount, decimalDigits.Value, MidpointRounding.AwayFromZero);
                        invoiceItemToCompare.Duration = Math.Round(invoiceItemToCompare.Duration, decimalDigits.Value, MidpointRounding.AwayFromZero);
                    }

                    var comparisonKey = new ComparisonKey
                    {
                        Destination = invoiceItemToCompare.Destination,
                        From = invoiceItemToCompare.From.Date,
                        To = invoiceItemToCompare.To.Date,
                    };
                    if (!invoiceItemsToCompare.ContainsKey(comparisonKey))
                    {
                        invoiceItemsToCompare.Add(comparisonKey, invoiceItemToCompare);
                    }else
                    {
                        throw new Exception(string.Format("Same Period Exist for {0} From: '{1}' To: '{2}'. ", invoiceItemToCompare.Destination, invoiceItemToCompare.From.Date, invoiceItemToCompare.To.Date));
                    }
                }

            }
            return invoiceItemsToCompare;
        }
        private void AddMissingSystemInvoices(List<InvoiceComparisonResult> invoiceComparisonResults, Dictionary<ComparisonKey, InvoiceItemToCompare> systemInvoiceItems)
        {
            if (systemInvoiceItems.Count > 0)
            {
                foreach (var item in systemInvoiceItems)
                {
                    var invoiceComparisonResult = new InvoiceComparisonResult
                    {
                        SystemDuration = item.Value.Duration,
                        SystemCalls = item.Value.Calls,
                        SystemRate = item.Value.Rate,
                        SystemAmount = item.Value.Amount,
                        Currency =_currencyManager.GetCurrencySymbol(item.Value.CurrencyId),
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
        private void FillInvoiceComparisonProviderFields(InvoiceComparisonResult invoiceComparisonResult, ConvertedExcelFieldsByName fields, string dateTimeFormat, int? decimalDigits, int longPrecision)
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
        private decimal? CalculateDiffValue(decimal? sysVal, decimal? provideVal)
        {
            if (sysVal.HasValue && sysVal.Value != 0 && provideVal.HasValue && provideVal.Value != 0)
            {
                return (sysVal.Value - provideVal.Value) * 100 / sysVal.Value;
            }
            return null;
        }

        private struct ComparisonKey
        {
            public string Destination { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        private class InvoiceItemToCompare
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

        private class InvoiceComparisonRequestHandler : BigDataRequestHandler<InvoiceComparisonInput, InvoiceComparisonResult, InvoiceComparisonResultDetail>
        {
            public InvoiceComparisonRequestHandler()
            {

            }
            public override InvoiceComparisonResultDetail EntityDetailMapper(InvoiceComparisonResult entity)
            {
                return null;
            }
            protected override Vanrise.Entities.BigResult<InvoiceComparisonResultDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonInput> input, IEnumerable<InvoiceComparisonResult> allRecords)
            {
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceAction = invoiceTypeManager.GetInvoiceAction(input.Query.InvoiceTypeId, input.Query.InvoiceActionId);
                invoiceAction.ThrowIfNull("invoiceAction");
                var invoiceActionSettings = invoiceAction.Settings as CompareInvoiceAction;
                invoiceActionSettings.ThrowIfNull("invoiceActionSettings");
                Func<InvoiceComparisonResult, bool> filterExpression = (invoiceComparisonResult) =>
                    {
                        if (input.Query.ComparisonResults != null)
                        {
                            if (!input.Query.ComparisonResults.Contains(invoiceComparisonResult.Result))
                                return false;
                        }
                        return true;
                    };

                return allRecords.ToBigResult(input, filterExpression, (entity) => InvoiceManager.InvoiceComparisonMapper(entity, invoiceActionSettings));
            }
            public override IEnumerable<InvoiceComparisonResult> RetrieveAllData(DataRetrievalInput<InvoiceComparisonInput> input)
            {
                return new InvoiceManager().CompareInvoices(input.Query);
            }
            protected override ResultProcessingHandler<InvoiceComparisonResultDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceComparisonInput> input, BigResult<InvoiceComparisonResultDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceComparisonResultDetail>
                {
                    ExportExcelHandler = new InvoiceComparisonExcelExportHandler(input.Query)
                };
            }

        }

        private class InvoiceComparisonExcelExportHandler : ExcelExportHandler<InvoiceComparisonResultDetail>
        {
            InvoiceComparisonInput _query;
            public InvoiceComparisonExcelExportHandler(InvoiceComparisonInput query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvoiceComparisonResultDetail> context)
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
                sheet.Header.Cells.Add(new ExportExcelHeaderCell{
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
                     Title = "Diff. Amount %",
                  });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                     Title = "Result",
                });

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<InvoiceComparisonResultDetail>;

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
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemDuration });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderDuration });
                            row.Cells.Add(new ExportExcelCell { 
                                Value = item.Entity.DiffDuration,
                                Style = new ExcelCellStyle
                                {
                                    Color = item.Entity.DiffDurationColor
                                }
                            });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.SystemAmount });
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.ProviderAmount });
                            row.Cells.Add(new ExportExcelCell { 
                                Value = item.Entity.DiffAmount ,
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
    }
}
