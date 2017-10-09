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
using Vanrise.Common;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Common.Business;
using System.Globalization;
namespace TOne.WhS.Invoice.Business
{
    public class InvoiceManager
    {
        public IDataRetrievalResult<InvoiceComparisonResultDetail> CompareInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonInput> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new InvoiceComparisonRequestHandler());
        }

        public static InvoiceComparisonResultDetail InvoiceComparisonMapper(InvoiceComparisonResult invoiceComparisonResult, CompareInvoiceAction compareInvoiceAction)
        {

            string description = null;

            switch(invoiceComparisonResult.Result)
            {
                case ComparisonResult.Identical: description = Utilities.GetEnumDescription(invoiceComparisonResult.Result); break;
                case ComparisonResult.MajorDiff: description = Utilities.GetEnumDescription(invoiceComparisonResult.Result); break;
                case ComparisonResult.MinorDiff: description = Utilities.GetEnumDescription(invoiceComparisonResult.Result); break;
                case ComparisonResult.MissingProvider: description = string.Format("{0} {1}", Utilities.GetEnumDescription(invoiceComparisonResult.Result), compareInvoiceAction.PartnerLabel); break;
                case ComparisonResult.MissingSystem: description = Utilities.GetEnumDescription(invoiceComparisonResult.Result); break;
            }

            return new InvoiceComparisonResultDetail
            {
                Entity = invoiceComparisonResult,
                ResultDescription = description
            };
        }

        public bool UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(input.InvoiceId);
            invoice.ThrowIfNull("invoice", input.InvoiceId);
            var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
            supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");
            supplierInvoiceDetails.OriginalAmount = input.OriginalAmount;
            supplierInvoiceDetails.Reference = input.Reference;
            supplierInvoiceDetails.AttachementFiles = input.AttachementFiles;
            invoice.Details = supplierInvoiceDetails;
            return invoiceManager.TryUpdateInvoice(invoice);
        }


        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoice(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);
            var supplierInvoiceDetails = invoice.Details as SupplierInvoiceDetails;
            supplierInvoiceDetails.ThrowIfNull("supplierInvoiceDetails");

            OriginalInvoiceDataRuntime originalInvoiceDataRuntime = new OriginalInvoiceDataRuntime
            {
                OriginalAmount = supplierInvoiceDetails.OriginalAmount,
                Reference = supplierInvoiceDetails.Reference,
            };

            if (supplierInvoiceDetails.AttachementFiles != null && supplierInvoiceDetails.AttachementFiles.Count > 0)
            {
                originalInvoiceDataRuntime.AttachementFilesRuntime = new List<AttachementFileRuntime>();
                var fileIds = supplierInvoiceDetails.AttachementFiles.Select(x => x.FileId);
                VRFileManager vrFileManager = new VRFileManager();
                var files = vrFileManager.GetFilesInfo(fileIds);
                foreach (var attachementFile in supplierInvoiceDetails.AttachementFiles)
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
                return CompareInvoices(input.Query);
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

                InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
                var invoiceItems = invoiceItemManager.GetInvoiceItemsByItemSetNames(input.InvoiceId, new List<String> { itemGrouping.ItemSetName }, CompareOperator.Equal);
                InvoiceItemGroupingManager invoiceItemGroupingManager = new InvoiceItemGroupingManager();


                List<Guid> dimensions = new List<Guid>();
                dimensions.Add(invoiceActionSettings.ZoneDimensionId);
                dimensions.Add(invoiceActionSettings.FromDateDimensionId);
                dimensions.Add(invoiceActionSettings.ToDateDimensionId);
                dimensions.Add(invoiceActionSettings.CurrencyDimensionId);
                dimensions.Add(invoiceActionSettings.RateDimensionId);

                List<Guid> measures = new List<Guid>();
                measures.Add(invoiceActionSettings.AmountMeasureId);
                measures.Add(invoiceActionSettings.DurationMeasureId);
                measures.Add(invoiceActionSettings.NumberOfCallsMeasureId);

                GroupingInvoiceItemQuery query = new GroupingInvoiceItemQuery
                {
                    DimensionIds = dimensions,
                    MeasureIds = measures,
                    InvoiceTypeId = input.InvoiceTypeId,
                    ItemGroupingId = invoiceActionSettings.ItemGroupingId
                };
                var groupedItems = invoiceItemGroupingManager.ApplyFinalGroupingAndFiltering(new GroupingInvoiceItemQueryContext(query), invoiceItems, query.DimensionIds, query.MeasureIds, null, itemGrouping);

                var systemInvoiceItems = ConverGroupingItemsToComparisonResult(groupedItems, invoiceActionSettings, itemGrouping);

                ExcelConvertor excelConvertor = new ExcelConvertor();

                ExcelConversionSettings excelConversionSettings = new ExcelConversionSettings();
                excelConversionSettings.DateTimeFormat = input.DateTimeFormat;
                excelConversionSettings.ListMappings = new List<ListMapping>();
                excelConversionSettings.ListMappings.Add(input.ListMapping);

                ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(input.InputFileId, excelConversionSettings, true, false);
                return ProccessInvoiceComparisonResult(convertedExcel, systemInvoiceItems, input.Threshold,input.DateTimeFormat);


            }
            private List<InvoiceComparisonResult> ProccessInvoiceComparisonResult(ConvertedExcel convertedExcel, Dictionary<ComparisonKey, InvoiceItemToCompare> systemInvoiceItems, decimal threshold, string dateTimeFormat)
            {
                List<InvoiceComparisonResult> invoiceComparisonResults = new List<InvoiceComparisonResult>();
                ConvertedExcelList convertedExcelList;
                if (convertedExcel.Lists.TryGetValue("MainList", out convertedExcelList))
                {
                    foreach (var obj in convertedExcelList.Records)
                    {

                        InvoiceComparisonResult invoiceComparisonResult = new InvoiceComparisonResult();
                        FillInvoiceComparisonProviderFields(invoiceComparisonResult, obj.Fields, dateTimeFormat);

                        ComparisonKey comparisonKey = new ComparisonKey
                        {
                            Currency = invoiceComparisonResult.Currency,
                            Destination = invoiceComparisonResult.Destination,
                            From = invoiceComparisonResult.From.Date,
                            To = invoiceComparisonResult.To.Date
                        };

                        InvoiceItemToCompare invoiceItemToCompare;
                        if (systemInvoiceItems.TryGetValue(comparisonKey, out invoiceItemToCompare))
                        {
                            invoiceComparisonResult.SystemAmount = invoiceItemToCompare.Amount;
                            invoiceComparisonResult.SystemCalls = invoiceItemToCompare.Calls;
                            invoiceComparisonResult.SystemDuration = invoiceItemToCompare.Duration;
                            invoiceComparisonResult.SystemRate = invoiceItemToCompare.Rate;
                            LabelColor resultLabelColor;
                            invoiceComparisonResult.Result = GetInvoiceComparisonResult(threshold, invoiceItemToCompare.Amount, invoiceItemToCompare.Calls, invoiceItemToCompare.Duration, invoiceComparisonResult.ProviderAmount, invoiceComparisonResult.ProviderCalls, invoiceComparisonResult.ProviderDuration, out resultLabelColor);
                            invoiceComparisonResult.ResultColor = resultLabelColor;
                            systemInvoiceItems.Remove(comparisonKey);
                        }
                        else
                        {
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
            private ComparisonResult GetInvoiceComparisonResult(decimal threshold, decimal sysAmount, decimal sysNCalls, decimal sysDuration, decimal? providerAmount, decimal? providerNCalls,decimal? providerDuration, out LabelColor resultLabelColor)
            {

                if (CheckThresholdPercentage(sysAmount, providerAmount, threshold) 
                    || CheckThresholdPercentage(sysNCalls, providerNCalls, threshold) 
                    || CheckThresholdPercentage(sysDuration, providerDuration, threshold))
                {
                    resultLabelColor = LabelColor.Error;
                    return ComparisonResult.MajorDiff;
                }
               
                if ((providerAmount.HasValue && sysAmount != providerAmount.Value) 
                    || (providerNCalls.HasValue && sysNCalls != providerNCalls.Value) 
                    || (providerDuration.HasValue && sysDuration != providerDuration.Value))
                {
                    resultLabelColor = LabelColor.Warning;
                    return ComparisonResult.MinorDiff;
                }
                resultLabelColor = LabelColor.Success;
                return ComparisonResult.Identical;
            }

       
            private Dictionary<ComparisonKey, InvoiceItemToCompare> ConverGroupingItemsToComparisonResult(List<GroupingInvoiceItemDetail> groupedItems, CompareInvoiceAction compareInvoiceAction, ItemGrouping itemGrouping)
            {
                Dictionary<ComparisonKey, InvoiceItemToCompare> invoiceItemsToCompare = new Dictionary<ComparisonKey, InvoiceItemToCompare>();
                if (groupedItems != null)
                {

                    AggregateItemField amountMeasure = null;
                    AggregateItemField durationMeasure = null;
                    AggregateItemField callsMeasure = null;

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
                    }

                    foreach (var item in groupedItems)
                    {
                        var zoneDimension = item.DimensionValues[0];
                        var fromDateDimension = item.DimensionValues[1];
                        var toDateDimension = item.DimensionValues[2];
                        var currencyDimension = item.DimensionValues[3];
                        var rateDimension = item.DimensionValues[4];

                        InvoiceItemToCompare invoiceItemToCompare = new InvoiceItemToCompare
                        {
                            Destination = zoneDimension != null ? zoneDimension.Name : null,
                            From = fromDateDimension != null ? Convert.ToDateTime(fromDateDimension.Value) : default(DateTime),
                            To = toDateDimension != null ? Convert.ToDateTime(toDateDimension.Value) : default(DateTime),
                            Currency = currencyDimension != null ? currencyDimension.Name : null,
                            Rate = rateDimension != null ? Convert.ToDecimal(zoneDimension.Value) : default(decimal),
                        };

                        InvoiceGroupingMeasureValue amountMeasureItem;
                        InvoiceGroupingMeasureValue callsMeasureItem;
                        InvoiceGroupingMeasureValue durationMeasureItem;

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
                        invoiceItemsToCompare.Add(new ComparisonKey
                        {
                            Currency = invoiceItemToCompare.Currency,
                            Destination = invoiceItemToCompare.Destination,
                            From = invoiceItemToCompare.From.Date,
                            To = invoiceItemToCompare.To.Date,
                        }, invoiceItemToCompare);
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
                        invoiceComparisonResults.Add(new InvoiceComparisonResult
                        {
                            SystemDuration = item.Value.Duration,
                            SystemCalls = item.Value.Calls,
                            SystemRate = item.Value.Rate,
                            SystemAmount = item.Value.Amount,
                            Currency = item.Value.Currency,
                            Destination = item.Value.Destination,
                            From = item.Value.From,
                            To = item.Value.To,
                            Result = ComparisonResult.MissingProvider,
                            ResultColor = LabelColor.Error
                        });
                    }
                }
            }
            private void FillInvoiceComparisonProviderFields(InvoiceComparisonResult invoiceComparisonResult, ConvertedExcelFieldsByName fields, string dateTimeFormat)
            {

                #region Fill Destination
                ConvertedExcelField zoneField;
                if (fields.TryGetValue("Zone", out zoneField))
                {
                    invoiceComparisonResult.Destination = zoneField.FieldValue.ToString();
                };
                #endregion

                #region Fill FromDate
                ConvertedExcelField fromDateField;
                if (fields.TryGetValue("FromDate", out fromDateField))
                {
                   
                    if (fromDateField.FieldValue != null && !String.IsNullOrWhiteSpace(fromDateField.FieldValue.ToString()))
                    {
                        if(fromDateField.FieldValue is DateTime)
                        {
                            invoiceComparisonResult.From = Convert.ToDateTime(fromDateField.FieldValue).Date;

                        }else
                        {
                            DateTime fromDate;
                            if (DateTime.TryParseExact(fromDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out fromDate))
                                invoiceComparisonResult.From = fromDate.Date;
                        }
                       
                    }
                };
                #endregion

                #region Fill ToDate

                ConvertedExcelField toDateField;
                if (fields.TryGetValue("ToDate", out toDateField))
                {
                    if (toDateField.FieldValue != null && !String.IsNullOrWhiteSpace(toDateField.FieldValue.ToString()))
                    {
                        if (toDateField.FieldValue is DateTime)
                        {
                            invoiceComparisonResult.To = Convert.ToDateTime(toDateField.FieldValue).Date;
                        }
                        else
                        {
                            DateTime toDate;
                            if (DateTime.TryParseExact(toDateField.FieldValue.ToString(), dateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out toDate))
                                invoiceComparisonResult.To = toDate.Date;
                        }
                    }
                };
                #endregion

                #region Fill Currency

                ConvertedExcelField currencyField;
                if (fields.TryGetValue("Currency", out currencyField))
                {
                    invoiceComparisonResult.Currency = currencyField.FieldValue.ToString();
                };
               
                #endregion

                #region Fill Rate

                ConvertedExcelField rateField;
                if (fields.TryGetValue("Rate", out rateField))
                {
                    if (rateField.FieldValue != null && !String.IsNullOrWhiteSpace(rateField.FieldValue.ToString()))
                        invoiceComparisonResult.ProviderRate = Convert.ToDecimal(rateField.FieldValue);
                };
              
                #endregion

                #region Fill NumberOfCalls

                ConvertedExcelField callsField;
                if (fields.TryGetValue("NumberOfCalls", out callsField))
                {
                    if (callsField.FieldValue != null && !String.IsNullOrWhiteSpace(callsField.FieldValue.ToString()))
                        invoiceComparisonResult.ProviderCalls = Convert.ToDecimal(callsField.FieldValue);
                };
                #endregion

                #region Fill Amount

                ConvertedExcelField amountField;
                if (fields.TryGetValue("Amount", out amountField))
                {
                    if (amountField.FieldValue != null && !String.IsNullOrWhiteSpace(amountField.FieldValue.ToString()))
                        invoiceComparisonResult.ProviderAmount = Convert.ToDecimal(amountField.FieldValue);
                };
                #endregion

                #region Fill Duration

                ConvertedExcelField durationField;
                if (fields.TryGetValue("Duration", out durationField))
                {
                    if (durationField.FieldValue != null && !String.IsNullOrWhiteSpace(durationField.FieldValue.ToString()))
                        invoiceComparisonResult.ProviderDuration = Convert.ToDecimal(durationField.FieldValue);
                };
                #endregion

            }
      
            protected override ResultProcessingHandler<InvoiceComparisonResultDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceComparisonInput> input, BigResult<InvoiceComparisonResultDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceComparisonResultDetail>
                {
                    ExportExcelHandler = new InvoiceComparisonExcelExportHandler(input.Query)
                };
            }


            private bool CheckThresholdPercentage(decimal sysValue, decimal? provValue, decimal threshold)
            {
                if (provValue.HasValue)
                {
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
            private decimal? CalculateDiffValue(decimal? sysVal,decimal? provideVal)
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
                public string Currency { get; set; }
            }
            private class InvoiceItemToCompare
            {
                public string Destination { get; set; }
                public DateTime From { get; set; }
                public DateTime To { get; set; }
                public string Currency { get; set; }
                public Decimal Rate { get; set; }
                public Decimal Duration { get; set; }
                public Decimal Amount { get; set; }
                public int Calls { get; set; }
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
                sheet.Header.Cells.Add(new ExportExcelHeaderCell
                {
                    Title = "Curr.",
                               });

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
                            row.Cells.Add(new ExportExcelCell { Value = item.Entity.Currency });
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
