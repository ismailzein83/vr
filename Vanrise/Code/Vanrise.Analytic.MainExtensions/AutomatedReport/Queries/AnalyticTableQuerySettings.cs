using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class AnalyticTableQuerySettings : VRAutomatedReportQuerySettings
    {
        public VRTimePeriod TimePeriod { get; set; }

        public List<AnalyticTableQueryDimension> Dimensions { get; set; }

        public List<AnalyticTableQueryMeasure> Measures { get; set; }

        public List<DimensionFilter> Filters { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int? CurrencyId { get; set; }

        public bool WithSummary { get; set; }

        public int? TopRecords { get; set; }

        public AnalyticQueryOrderType? OrderType { get; set; }

        public AnalyticQueryAdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }

        public List<AnalyticQuerySettingsSubTable> SubTables { get; set; }

        public class AnalyticQuerySettingsSubTable
        {
            public string Title { get; set; }

            public Guid SubTableId { get; set; }
         
            public List<string> Dimensions { get; set; }

            public List<string> Measures { get; set; }

            public AnalyticQueryOrderType? OrderType { get; set; }

            public AnalyticQueryAdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }
        }

        public override VRAutomatedReportDataResult Execute(IVRAutomatedReportQueryExecuteContext context)
        {
            VRAutomatedReportDataResult automatedReportDataResult = new VRAutomatedReportDataResult
            {
                Lists = new Dictionary<string, VRAutomatedReportDataList>(),
                Fields = new Dictionary<string, VRAutomatedReportDataFieldValue>()
            };
            VRAutomatedReportDataList automatedReportDataList = new VRAutomatedReportDataList()
            {
                Items = new List<VRAutomatedReportDataItem>()
            };
            if(this.SubTables!=null && this.SubTables.Count>0){
                automatedReportDataList.ItemTables = new Dictionary<Guid,VRAutomatedReportDataSubTable>();
            }
            List<string> dimensionFields = new List<string>();
            List<string> measureFields = new List<string>();
            if (Dimensions != null && Dimensions.Count > 0)
            {
                foreach (var dimension in Dimensions)
                {
                    dimensionFields.Add(dimension.DimensionName);
                }
            }
            if (Measures != null && Measures.Count > 0)
            {
                foreach (var measure in Measures)
                {
                    measureFields.Add(measure.MeasureName);
                }
            }
            List<AnalyticQuerySubTable> analyticSubTables = new List<AnalyticQuerySubTable>();
            if (SubTables != null && SubTables.Count > 0)
            {
                foreach (var subTable in SubTables)
                {
                    analyticSubTables.Add(new AnalyticQuerySubTable()
                    {
                        AdvancedOrderOptions = subTable.AdvancedOrderOptions,
                        Dimensions = subTable.Dimensions,
                        Measures = subTable.Measures,
                        OrderType = AnalyticQueryOrderType.ByAllDimensions
                    });
                }
            }

            VRAutomatedReportQueryDefinitionManager reportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            var automatedReportQueryDefinitionSettings = reportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(context.QueryDefinitionId);
            automatedReportQueryDefinitionSettings.ThrowIfNull("automatedReportQueryDefinitionSettings", context.QueryDefinitionId);
            var analyticTableQueryDefinitionSettings = automatedReportQueryDefinitionSettings.ExtendedSettings.CastWithValidate<AnalyticTableQueryDefinitionSettings>("AnalyticTableQuerySettings");
          
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();

            DateTime fromTime;
            DateTime toTime;

            if (context.FilterDefinition != null && context.FilterRuntime != null)
            {
                var filterContent = context.FilterRuntime.GetFilterContent(new VRReportGenerationRuntimeFilterContext
                {
                    FilterDefinition = context.FilterDefinition
                });
                fromTime = filterContent.FromTime;
                toTime = filterContent.ToTime;
            }
            else
            {
                VRTimePeriodManager timePeriodManager = new VRTimePeriodManager();
                var dateTimeRange = timePeriodManager.GetTimePeriod(this.TimePeriod);
                fromTime = dateTimeRange.From;
                toTime = dateTimeRange.To;
            }

            var query = new AnalyticQuery()
            {
                DimensionFields = dimensionFields,
                MeasureFields = measureFields,
                FilterGroup = FilterGroup,
                CurrencyId = CurrencyId,
                WithSummary = WithSummary,
                TopRecords = TopRecords,
                OrderType = OrderType,
                AdvancedOrderOptions = AdvancedOrderOptions,
                TableId = analyticTableQueryDefinitionSettings.AnalyticTableId,
                FromTime = fromTime,
                ToTime = toTime,
                SubTables = analyticSubTables
            };
            
            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord summaryRecord;
            List<AnalyticResultSubTable> resultSubTables;
            var dataRecords = analyticManager.GetAllFilteredRecords(query, out summaryRecord, out resultSubTables);
            BuildSubTableData(automatedReportDataList, resultSubTables, summaryRecord);
            BuildAutomatedReportDataListItems(dataRecords, automatedReportDataList);
            automatedReportDataResult.Lists.Add("Main", automatedReportDataList);
            return automatedReportDataResult;
        }

        public override VRAutomatedReportDataSchema GetSchema(IVRAutomatedReportQueryGetSchemaContext context)
        {
            VRAutomatedReportDataSchema automatedReportSchema = new VRAutomatedReportDataSchema()
            {
                ListSchemas = new Dictionary<string, VRAutomatedReportDataListSchema>()
            };
            Dictionary<string, VRAutomatedReportDataListSchema> listSchema = new Dictionary<string, VRAutomatedReportDataListSchema>();
            List<string> dimensionNames = new List<string>();
            List<string> measureNames = new List<string>();
            foreach (var dimension in this.Dimensions)
            {
                dimensionNames.Add(dimension.DimensionName);
            }
            foreach (var measure in this.Measures)
            {
                measureNames.Add(measure.MeasureName);
            }

            VRAutomatedReportDataListSchema listSchemaItem = new VRAutomatedReportDataListSchema()
            {
                FieldSchemas = new Dictionary<string, VRAutomatedReportDataFieldSchema>(),
                SubTablesSchemas = new Dictionary<Guid,VRAutomatedReportDataSubTableSchema>()
            };
            Dictionary<string, VRAutomatedReportDataFieldSchema> fieldSchema = new Dictionary<string, VRAutomatedReportDataFieldSchema>();
            VRAutomatedReportQueryDefinitionManager automatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            var automatedReportQueryDefinitionSettings = automatedReportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(context.QueryDefinitionId);
            automatedReportQueryDefinitionSettings.ThrowIfNull("automatedReportQueryDefinitionSettings");
            automatedReportQueryDefinitionSettings.ExtendedSettings.ThrowIfNull("automatedReportQueryDefinitionSettings.ExtendedSettings");
            var analyticTableQueryDefinitionSettings = automatedReportQueryDefinitionSettings.ExtendedSettings.CastWithValidate<AnalyticTableQueryDefinitionSettings>("automatedReportQueryDefinitionSettings.ExtendedSettings");
            analyticTableQueryDefinitionSettings.ThrowIfNull("recordSearchQueryDefinitionSettings");
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            var dimensions = analyticItemConfigManager.GetDimensions(analyticTableQueryDefinitionSettings.AnalyticTableId);
            var measures = analyticItemConfigManager.GetMeasures(analyticTableQueryDefinitionSettings.AnalyticTableId);

            foreach (var dimensionName in dimensionNames)
            {
                var dimension = dimensions.GetRecord(dimensionName);
                if (dimension != null)
                {
                    var dataFieldSchema = new VRAutomatedReportDataFieldSchema()
                    {
                        Field = new DataRecordField()
                        {
                            Name = dimensionName,
                            Title = dimension.Title,
                            Type = dimension.Config != null ? dimension.Config.FieldType : null
                        }
                    };
                    listSchemaItem.FieldSchemas.Add(dimensionName, dataFieldSchema);
                }
            }

            foreach (var measureName in measureNames)
            {
                var measure = measures.GetRecord(measureName);
                if (measure != null)
                {
                    var dataFieldSchema = new VRAutomatedReportDataFieldSchema()
                    {
                        Field = new DataRecordField()
                        {
                            Name = measureName,
                            Title = measure.Title,
                            Type = measure.Config != null ? measure.Config.FieldType : null
                        }
                    };
                    listSchemaItem.FieldSchemas.Add(measureName, dataFieldSchema);
                }
            }

            if (this.SubTables != null && this.SubTables.Count > 0)
            {
                foreach (var subtable in this.SubTables)
                {
                    VRAutomatedReportDataSubTableSchema subTableSchema = new VRAutomatedReportDataSubTableSchema
                    {
                        SubTableTitle = subtable.Title,
                        FieldSchemas = new Dictionary<string, VRAutomatedReportDataFieldSchema>()
                    };
                    foreach (var measureName in subtable.Measures)
                    {
                        var measure = measures.GetRecord(measureName);
                        if (measure != null)
                        {
                            var dataFieldSchema = new VRAutomatedReportDataFieldSchema()
                            {
                                Field = new DataRecordField()
                                {
                                    Name = measureName,
                                    Title = measure.Title,
                                    Type = measure.Config != null ? measure.Config.FieldType : null
                                }
                            };
                            subTableSchema.FieldSchemas.Add(measureName, dataFieldSchema);
                        }
                    }
                    listSchemaItem.SubTablesSchemas.Add(subtable.SubTableId, subTableSchema);
                }
            }
            automatedReportSchema.ListSchemas.Add("Main", listSchemaItem);
            return automatedReportSchema;
        }

        public override Dictionary<string,VRAutomatedReportDataFieldSchema> GetSubTableFields(IVRAutomatedReportQueryGetSubTableFieldsContext context)
        {
            this.SubTables.ThrowIfNull("SubTables");
            context.ThrowIfNull("context");
            var subTable = this.SubTables.FindRecord(x => x.SubTableId == context.SubTableId);
            subTable.ThrowIfNull("subTable", context.SubTableId);
            List<string> dimensionNames = new List<string>();
            foreach (var dimension in subTable.Dimensions)
            {
                dimensionNames.Add(dimension);
            }
            Dictionary<string, VRAutomatedReportDataFieldSchema> subTableFieldSchema = new Dictionary<string, VRAutomatedReportDataFieldSchema>();
            VRAutomatedReportQueryDefinitionManager automatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            var automatedReportQueryDefinitionSettings = automatedReportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(context.QueryDefinitionId);
            automatedReportQueryDefinitionSettings.ThrowIfNull("automatedReportQueryDefinitionSettings");
            automatedReportQueryDefinitionSettings.ExtendedSettings.ThrowIfNull("automatedReportQueryDefinitionSettings.ExtendedSettings");
            var analyticTableQueryDefinitionSettings = automatedReportQueryDefinitionSettings.ExtendedSettings.CastWithValidate<AnalyticTableQueryDefinitionSettings>("automatedReportQueryDefinitionSettings.ExtendedSettings");
            analyticTableQueryDefinitionSettings.ThrowIfNull("recordSearchQueryDefinitionSettings");
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            var dimensions = analyticItemConfigManager.GetDimensions(analyticTableQueryDefinitionSettings.AnalyticTableId);

            foreach (var dimensionName in dimensionNames)
            {
                var dimension = dimensions.GetRecord(dimensionName);
                if (dimension != null)
                {
                    var dataFieldSchema = new VRAutomatedReportDataFieldSchema()
                    {
                        Field = new DataRecordField()
                        {
                            Name = dimensionName,
                            Title = dimension.Title,
                            Type = dimension.Config != null ? dimension.Config.FieldType : null
                        }
                    };
                    subTableFieldSchema.Add(dimensionName, dataFieldSchema);
                }
            }
            return subTableFieldSchema;
        }
        private void BuildAutomatedReportDataListItems(List<AnalyticRecord> dataRecords, VRAutomatedReportDataList automatedReportDataList)
        {
            if (dataRecords != null && dataRecords.Count() > 0)
            {
                foreach (var dataItem in dataRecords)
                {
                    var item = new VRAutomatedReportDataItem
                    {
                        Fields = new Dictionary<string, VRAutomatedReportDataFieldValue>()
                    };
                    if (dataItem.DimensionValues != null && dataItem.DimensionValues.Count() > 0 && this.Dimensions != null && this.Dimensions.Count > 0)
                    {
                        for (var i = 0; i < this.Dimensions.Count; i++)
                        {
                            var dimension = Dimensions[i];
                            var dimensionValue = dataItem.DimensionValues.ElementAtOrDefault(i);
                            if (dimension != null && dimensionValue != null)
                            {
                                var dataFieldValue = new VRAutomatedReportDataFieldValue()
                                {
                                    Value = dimensionValue.Value
                                };
                                item.Fields.Add(dimension.DimensionName, dataFieldValue);
                            }
                        }
                    }

                    if (dataItem.MeasureValues != null && dataItem.MeasureValues.Count > 0)
                    {
                        for (var i = 0; i < this.Measures.Count; i++)
                        {
                            var measure = Measures[i];
                            MeasureValue measureValue;
                            if (measure != null)
                            {
                                dataItem.MeasureValues.TryGetValue(measure.MeasureName, out measureValue);

                                if (measureValue != null)
                                {
                                    var dataFieldValue = new VRAutomatedReportDataFieldValue()
                                    {
                                        Value = measureValue.Value
                                    };
                                    item.Fields.Add(measure.MeasureName, dataFieldValue);
                                }
                            }
                        }
                    }
                    if (dataItem.SubTables != null && dataItem.SubTables.Count > 0 && this.SubTables != null && this.SubTables.Count > 0)
                    {
                        item.SubTables = new Dictionary<Guid, VRAutomatedReportDataSubTable>();
                        for (int i = 0; i < this.SubTables.Count; i++)
                        {
                            var subTable = this.SubTables[i];
                            var dataItemSubTable = dataItem.SubTables[i];
                            if (subTable != null && subTable.Measures != null && subTable.Measures.Count > 0 && dataItemSubTable != null && dataItemSubTable.MeasureValues != null && dataItemSubTable.MeasureValues.Count > 0)
                            {
                                VRAutomatedReportDataSubTable reportDataSubTable = new VRAutomatedReportDataSubTable()
                                {
                                    Fields = new Dictionary<string, VRAutomatedReportDataSubTableFieldInfo>()
                                };
                                for (int k = 0; k < subTable.Measures.Count; k++)
                                {
                                    VRAutomatedReportDataSubTableFieldInfo reportDataSubTableFieldInfo = new VRAutomatedReportDataSubTableFieldInfo()
                                    {
                                        FieldsValues = new List<VRAutomatedReportDataFieldValue>()
                                    };
                                    foreach (var measureValues in dataItemSubTable.MeasureValues)
                                    {
                                        var measureValue = measureValues.ElementAtOrDefault(k);
                                        if (measureValue.Value != null)
                                        {
                                            reportDataSubTableFieldInfo.FieldsValues.Add(new VRAutomatedReportDataFieldValue()
                                            {
                                                Value = measureValue.Value.Value
                                            });
                                        }
                                    }
                                    reportDataSubTable.Fields.Add(subTable.Measures[k], reportDataSubTableFieldInfo);
                                }
                                item.SubTables.Add(subTable.SubTableId, reportDataSubTable);
                            }
                        }
                    }
                    automatedReportDataList.Items.Add(item);
                }
            }
        }
        private void BuildSubTableData(VRAutomatedReportDataList automatedReportDataList, List<AnalyticResultSubTable> resultSubTables, AnalyticRecord summaryRecord)
        {
            if (this.SubTables != null && this.SubTables.Count > 0 && resultSubTables != null && resultSubTables.Count > 0)
            {
                if (this.WithSummary && summaryRecord != null)
                {
                    automatedReportDataList.SummaryDataItem = new VRAutomatedReportDataItem()
                    {
                        SubTables = new Dictionary<Guid, VRAutomatedReportDataSubTable>(),
                        Fields = new Dictionary<string, VRAutomatedReportDataFieldValue>()
                    };
                    if (summaryRecord.MeasureValues != null && summaryRecord.MeasureValues.Count() > 0)
                    {
                        for (var i = 0; i < this.Measures.Count; i++)
                        {
                            var measure = Measures[i];
                            var measureValue = summaryRecord.MeasureValues.ElementAtOrDefault(i);
                            var dataFieldValue = new VRAutomatedReportDataFieldValue()
                            {
                                Value = measureValue.Value != null ? measureValue.Value.Value : null
                            };
                            automatedReportDataList.SummaryDataItem.Fields.Add(measure.MeasureName, dataFieldValue);
                        }
                    }
                }
                for (int l = 0; l < this.SubTables.Count; l++)
                {
                    var subTable = this.SubTables[l];
                    var resultSubTable = resultSubTables[l];
                    VRAutomatedReportDataSubTable reportHeadersSubTable = new VRAutomatedReportDataSubTable()
                    {
                        Fields = new Dictionary<string, VRAutomatedReportDataSubTableFieldInfo>()
                    };
                    if (subTable.Dimensions != null && subTable.Dimensions.Count > 0 && resultSubTable.DimensionValues != null && resultSubTable.DimensionValues.Count > 0)
                    {
                        for (int y = 0; y < subTable.Dimensions.Count; y++)
                        {
                            var reportDataSubTableFieldInfo = reportHeadersSubTable.Fields.GetOrCreateItem(subTable.Dimensions[y], () =>
                            {
                                return new VRAutomatedReportDataSubTableFieldInfo()
                                {
                                    FieldsValues = new List<VRAutomatedReportDataFieldValue>()
                                };
                            });
                            foreach (var dimensionValues in resultSubTable.DimensionValues)
                            {
                                var dimensionValue = dimensionValues.ElementAtOrDefault(y);
                                if (dimensionValue != null)
                                {
                                    reportDataSubTableFieldInfo.FieldsValues.Add(new VRAutomatedReportDataFieldValue()
                                    {
                                        Value = dimensionValue.Value
                                    });
                                }
                            }
                        }
                        automatedReportDataList.ItemTables.Add(subTable.SubTableId, reportHeadersSubTable);
                    }
                    if (this.WithSummary && summaryRecord != null)
                    {
                        if (summaryRecord.SubTables != null && summaryRecord.SubTables.Count > 0)
                        {
                            var summarySubTable = summaryRecord.SubTables[l];
                            if (subTable.Measures != null && subTable.Measures.Count > 0 && summarySubTable != null && summarySubTable.MeasureValues != null && summarySubTable.MeasureValues.Count > 0)
                            {
                                VRAutomatedReportDataSubTable reportDataSubTable = new VRAutomatedReportDataSubTable()
                                {
                                    Fields = new Dictionary<string, VRAutomatedReportDataSubTableFieldInfo>()
                                };
                                for (int k = 0; k < subTable.Measures.Count; k++)
                                {
                                    VRAutomatedReportDataSubTableFieldInfo reportDataSubTableFieldInfo = new VRAutomatedReportDataSubTableFieldInfo()
                                    {
                                        FieldsValues = new List<VRAutomatedReportDataFieldValue>()
                                    };
                                    foreach (var measureValues in summarySubTable.MeasureValues)
                                    {
                                        var measureValue = measureValues.ElementAtOrDefault(k);
                                        if (measureValue.Value != null)
                                        {
                                            reportDataSubTableFieldInfo.FieldsValues.Add(new VRAutomatedReportDataFieldValue()
                                            {
                                                Value = measureValue.Value.Value
                                            });
                                        }
                                    }
                                    reportDataSubTable.Fields.Add(subTable.Measures[k], reportDataSubTableFieldInfo);
                                }
                                automatedReportDataList.SummaryDataItem.SubTables.Add(subTable.SubTableId, reportDataSubTable);
                            }
                        }
                    }
                }
            }
        }
    }

    public class AnalyticTableQueryDimension
    {
        public string DimensionName { get; set; }
    }

    public class AnalyticTableQueryMeasure
    {
        public string MeasureName { get; set; }
    }
}
