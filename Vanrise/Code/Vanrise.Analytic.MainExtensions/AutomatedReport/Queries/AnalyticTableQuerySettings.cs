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
            VRAutomatedReportDataList automatedreportDataList = new VRAutomatedReportDataList()
            {
                Items = new List<VRAutomatedReportDataItem>()
            };
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

            VRAutomatedReportQueryDefinitionManager reportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            var automatedReportQueryDefinitionSettings = reportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(context.QueryDefinitionId);
            automatedReportQueryDefinitionSettings.ThrowIfNull("automatedReportQueryDefinitionSettings", context.QueryDefinitionId);
            var analyticTableQueryDefinitionSettings = automatedReportQueryDefinitionSettings.ExtendedSettings.CastWithValidate<AnalyticTableQueryDefinitionSettings>("AnalyticTableQuerySettings");
            VRTimePeriodManager timePeriodManager = new VRTimePeriodManager();
            var dateTimeRange = timePeriodManager.GetTimePeriod(this.TimePeriod);
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();

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
                FromTime = dateTimeRange.From,
                ToTime = dateTimeRange.To
            };
            
            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord summaryRecord;
            var dataRecords = analyticManager.GetAllFilteredRecords(query, out summaryRecord);
            if (dataRecords != null &&  dataRecords.Count() > 0)
            {
                foreach (var dataItem in dataRecords)
                {
                    var item = new VRAutomatedReportDataItem
                    {
                        Fields = new Dictionary<string, VRAutomatedReportDataFieldValue>()
                    };
                    if (dataItem.DimensionValues != null && dataItem.DimensionValues.Count() > 0)
                    {
                        for (var i = 0; i < this.Dimensions.Count; i++)
                        {
                            var dimension = Dimensions[i];
                            var dimensionValue = dataItem.DimensionValues.ElementAtOrDefault(i);

                            var dataFieldValue = new VRAutomatedReportDataFieldValue()
                            {
                                Value = dimensionValue!=null ? dimensionValue.Value : null
                            };
                            item.Fields.Add(dimension.DimensionName, dataFieldValue);
                        }
                    }

                    if (dataItem.MeasureValues != null && dataItem.MeasureValues.Count > 0)
                    {
                        for (var i = 0; i < this.Measures.Count; i++)
                        {
                            var measure = Measures[i];
                            MeasureValue measureValue;
                            dataItem.MeasureValues.TryGetValue(measure.MeasureName, out measureValue);

                            var dataFieldValue = new VRAutomatedReportDataFieldValue()
                            {
                                Value = measureValue!= null ? measureValue.Value : null
                            };
                            item.Fields.Add(measure.MeasureName, dataFieldValue);
                        }
                    }
                    automatedreportDataList.Items.Add(item);
                }
            }
            automatedReportDataResult.Lists.Add("Main", automatedreportDataList);
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
                    foreach (var dimensionName in subtable.Dimensions)
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
                            subTableSchema.FieldSchemas.Add(dimensionName, dataFieldSchema);
                        }
                    }

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
