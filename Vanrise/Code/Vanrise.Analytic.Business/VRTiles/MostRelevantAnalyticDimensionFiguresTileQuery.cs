using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class MostRelevantAnalyticDimensionFiguresTileQuery : FiguresTileQuerySettings
    {
        public override Guid ConfigId { get { return new Guid("A0833F27-6718-40E1-BB69-344B0BD3CFAF"); } }

        public override List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context)
        {
            List<string> dimensionFields = new List<string>();
            AnalyticItemConfigManager analyticItemConfigmanager = new AnalyticItemConfigManager();
            var dimensionName = analyticItemConfigmanager.GetDimensionName(AnalyticTableId, DimensionId);
            dimensionName.ThrowIfNull("dimensionName");
            dimensionFields.Add(dimensionName);
            List<FigureItemValue> figureItemValues = new List<FigureItemValue>();
            VRTimePeriodContext timePeriodContext = new VRTimePeriodContext() { EffectiveDate = DateTime.Today };
            TimePeriod.GetTimePeriod(timePeriodContext);
            AnalyticManager analyticManager = new AnalyticManager();

            var Query = new AnalyticQuery()
            {
                MeasureFields = context.ItemsToDisplayNames,
                DimensionFields = dimensionFields,
                TableId = AnalyticTableId,
                FromTime = timePeriodContext.FromTime,
                ToTime = timePeriodContext.ToTime,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                // OrderType =AnalyticQueryOrderType.ByAllDimensions,
            };

            var analyticRecords = analyticManager.GetAllFilteredRecords(Query) as List<AnalyticRecord>;
            analyticRecords.ThrowIfNull("analyticRecords");
            var record = analyticRecords.FirstOrDefault();
            record.ThrowIfNull("record");
            var dimensions = record.DimensionValues;
            var selectedDimension = dimensions.First();
                figureItemValues.Add(new FigureItemValue()
                {
                    Name = selectedDimension.Name,
                    Value = selectedDimension.Value
                });

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
        public Guid DimensionId { get; set; }
        public List<Guid> Measures { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
        public RecordFilter RecordFilter { get; set; }
        public AnalyticQueryAdvancedMeasureOrderOptions AdvancedOrderOptions { get; set; }

    }

}
