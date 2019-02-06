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
    public class OverallAnalyticFiguresTileQuery : FiguresTileQuerySettings
    {
        public override Guid ConfigId { get { return new Guid("1A67CEF6-7472-4151-8A74-8AF12D245E27"); } }

        public override List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context)
        {
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
                MeasureFields = selectedItemsToDisplayNames,
                TableId = AnalyticTableId,
                FromTime = timePeriodContext.FromTime,
                ToTime = timePeriodContext.ToTime,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                // OrderType =AnalyticQueryOrderType.ByAllDimensions,
            };

            var analyticRecords = analyticManager.GetAllFilteredRecords(Query) as List<AnalyticRecord>;
            analyticRecords.ThrowIfNull("analyticRecords");
            if (analyticRecords.Count() > 0)
            {
                var record = analyticRecords.FirstOrDefault();
                record.ThrowIfNull("record");
                var measures = record.MeasureValues;
                foreach (var measure in measures)
                {
                    var item = itemsToDisplay.FindRecord(x => x.Name == measure.Key);
                    figureItemValues.Add(new FigureItemValue()
                    {
                        Name = item.Title,
                        Value = measure.Value.Value
                    });
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
        public RecordFilter RecordFilter { get; set; }

    }

}
