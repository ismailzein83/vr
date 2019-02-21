using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.FiguresTileQueries
{
    public class OverallAnalyticFiguresTileQuery : FiguresTileQuerySettings
    {
        public override Guid ConfigId { get { return new Guid("1A67CEF6-7472-4151-8A74-8AF12D245E27"); } }

        public override List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context)
        {
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            StyleDefinitionManager styleDefinitionManager = new StyleDefinitionManager();
            List<FigureItemValue> figureItemValues = new List<FigureItemValue>();
            VRTimePeriodContext timePeriodContext = new VRTimePeriodContext() { EffectiveDate = DateTime.Now };
            TimePeriod.GetTimePeriod(timePeriodContext);
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> selectedItemsToDisplayNames = new List<string>();
            var itemsToDisplay = context.ItemsToDisplay;
            var allMeasures = analyticItemConfigManager.GetMeasures(AnalyticTableId);
            foreach (var item in itemsToDisplay)
            {
                selectedItemsToDisplayNames.Add(item.Name);
                figureItemValues.Add(new FigureItemValue()
                {
                    Name = item.Title,
                });
            }
            var Query = new AnalyticQuery()
            {
                MeasureFields = selectedItemsToDisplayNames,
                TableId = AnalyticTableId,
                FromTime = timePeriodContext.FromTime,
                ToTime = timePeriodContext.ToTime,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                // OrderType =AnalyticQueryOrderType.ByAllDimensions,
                FilterGroup = this.RecordFilter
                // (this.RecordFilter == null) ? null : new RecordFilterGroup() { Filters = new List<RecordFilter>() { this.RecordFilter }, LogicalOperator = RecordQueryLogicalOperator.And, FieldName = this.RecordFilter.FieldName }
            };

            var analyticRecords = analyticManager.GetAllFilteredRecords(Query) as List<AnalyticRecord>;
            analyticRecords.ThrowIfNull("analyticRecords");
            if (analyticRecords.Count() > 0)
            {
                var record = analyticRecords.FirstOrDefault();
                record.ThrowIfNull("record");
                var measures = record.MeasureValues;
                foreach (var measureKvp in measures)
                {
                    var analyticMeasure = allMeasures.GetRecord(measureKvp.Key);
                    analyticMeasure.ThrowIfNull("analyticMeasure");
                    StyleDefinition styleDefinition = null;
                    if (measureKvp.Value.StyleDefinitionId.HasValue)
                        styleDefinition = styleDefinitionManager.GetStyleDefinition(measureKvp.Value.StyleDefinitionId.Value);

                    var item = itemsToDisplay.FindRecord(x => x.Name == measureKvp.Key);
                    if (item != null)
                    {
                        var figureItemValue = figureItemValues.FindRecord(x => x.Name == item.Title);
                        if (figureItemValue != null)
                        {
                            // figureItemValue.Value = measure.Value.ModifiedValue;
                            figureItemValue.StyleFormatingSettings = styleDefinition != null && styleDefinition.StyleDefinitionSettings != null ? styleDefinition.StyleDefinitionSettings.StyleFormatingSettings : null;
                            if (analyticMeasure.Config != null)
                            {
                                var measureType = analyticMeasure.Config.FieldType;
                                if (measureType != null)
                                    figureItemValue.Value = measureType.GetDescription(measureKvp.Value.ModifiedValue);
                            }
                        }
                    }
                }
            }
            return figureItemValues;
        }

        public override List<FigureItemSchema> GetSchema(IFiguresTileQueryGetSchemaContext context)
        {
            List<FigureItemSchema> figureItemsSchema = new List<FigureItemSchema>();
            AnalyticItemConfigManager analyticItemConfigmanager = new AnalyticItemConfigManager();
            var allMeasuresByMeasureName = analyticItemConfigmanager.GetMeasures(AnalyticTableId);
            allMeasuresByMeasureName.ThrowIfNull("allMeasuresByMeasureName");
            foreach (var measureId in Measures)
            {
                var selectedMeasure = allMeasuresByMeasureName.First(x => x.Value.AnalyticMeasureConfigId == measureId);

                if (selectedMeasure.Value == null)
                    throw new NullReferenceException("selectedMeasure");

                figureItemsSchema.Add(new FigureItemSchema
                {
                    Name = selectedMeasure.Key,
                    Title = selectedMeasure.Value.Title,
                });
            }
            return figureItemsSchema;
        }

        public Guid AnalyticTableId { get; set; }
        public List<Guid> Measures { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }

    }

}
