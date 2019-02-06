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
            var measureNames = analyticItemConfigmanager.GetMeasuresNames(AnalyticTableId, Measures);
            measureNames.ThrowIfNull("measureNames");
            var dimensionName = analyticItemConfigmanager.GetDimensionName(AnalyticTableId, DimensionId);
            dimensionName.ThrowIfNull("dimensionName");
            dimensionFields.Add(dimensionName);
            List<FigureItemValue> figureItemValues = new List<FigureItemValue>();
            VRTimePeriodContext timePeriodContext = new VRTimePeriodContext() { EffectiveDate = DateTime.Today };
            TimePeriod.GetTimePeriod(timePeriodContext);
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> selectedItemsToDisplayNames = new List<string>();
            var itemsToDisplay = context.ItemsToDisplay;
            foreach (var item in itemsToDisplay)
            {
                selectedItemsToDisplayNames.Add(item.Name);
            }
            var Query = new AnalyticQuery()
            {
                MeasureFields = measureNames,
                DimensionFields = selectedItemsToDisplayNames,
                TableId = AnalyticTableId,
                FromTime = timePeriodContext.FromTime,
                ToTime = timePeriodContext.ToTime,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                OrderType =AnalyticQueryOrderType.AdvancedMeasureOrder,
                AdvancedOrderOptions = AdvancedOrderOptions
            };

            var analyticRecords = analyticManager.GetAllFilteredRecords(Query) as List<AnalyticRecord>;
            analyticRecords.ThrowIfNull("analyticRecords");
            figureItemValues.Add(new FigureItemValue()
            {
                Name = itemsToDisplay.First().Title,
            });
            if (analyticRecords.Count() > 0)
            {
                var record = analyticRecords.FirstOrDefault();
                record.ThrowIfNull("record");
                var dimensions = record.DimensionValues;
                var selectedDimension = dimensions.First();
                // var selectedItem = context.ItemsToDisplayNames
                figureItemValues.First().Value = selectedDimension.Value;
            }
            return figureItemValues;
        }

        public override List<FigureItemSchema> GetSchema(IFiguresTileQueryGetSchemaContext context)
        {
            List<FigureItemSchema> figureItemsSchema = new List<FigureItemSchema>();
            AnalyticItemConfigManager analyticItemConfigmanager = new AnalyticItemConfigManager();
            var allDimensionsByDimensionName = analyticItemConfigmanager.GetDimensions(AnalyticTableId);
            allDimensionsByDimensionName.ThrowIfNull("allDimensionsByDimensionName");
           
                var selectedDimension = allDimensionsByDimensionName.First(x => x.Value.AnalyticDimensionConfigId == DimensionId);

                if (selectedDimension.Value == null)
                    throw new NullReferenceException("selectedMeasure");

                figureItemsSchema.Add(new FigureItemSchema
                {
                    Name = selectedDimension.Key,
                    Title = selectedDimension.Value.Title,
                });
           
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
