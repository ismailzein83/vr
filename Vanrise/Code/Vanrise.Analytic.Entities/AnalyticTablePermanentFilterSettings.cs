using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticTablePermanentFilterSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract RecordFilterGroup ConvertToRecordFilter(IAnalyticTablePermanentFilterContext context);

    }
    public class FilterGroupAnalyticTablePermanentFilter: AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("5E32BB46-462E-44A0-B1F0-1F6DA3BD9053");
        public override RecordFilterGroup ConvertToRecordFilter(IAnalyticTablePermanentFilterContext context)
        {
            return RecordFilterGroup;
        }
        public RecordFilterGroup RecordFilterGroup { get; set; }

    }
    public class ConditionFilterAnalyticTablePermanentFilter : AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("A4FCECC7-375F-4069-B0A5-7CCD1AE974CB");
        public override RecordFilterGroup ConvertToRecordFilter(IAnalyticTablePermanentFilterContext context)
        {
            var recordFilterGroup = new RecordFilterGroup()
            {
                LogicalOperator = LogicalOperator,
                Filters = new List<RecordFilter>()
            };
            if (ConditionFilterItems != null && ConditionFilterItems.Count > 0)
            {
                foreach (var filter in ConditionFilterItems)
                {
                    if (filter.Settings != null)
                    {
                        AnalyticTablePermanentFilterContext filterContext = new AnalyticTablePermanentFilterContext();
                        recordFilterGroup.Filters.Add(filter.Settings.ConvertToRecordFilter(filterContext));
                    }
                }
            }
            return recordFilterGroup;
        }

        public RecordQueryLogicalOperator LogicalOperator { get; set; }

        public List<AnalyticTableConditionFilterItem> ConditionFilterItems { get; set; }
    }

    public class AnalyticTableConditionFilterItem
    {
        public Guid ConditionFilterItemId { get; set; }
        public string Name { get; set; }
        public AnalyticTablePermanentFilterSettings Settings { get; set; }
    }
    public class AnalyticTablePermanentFilterSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_PermanentFilterSettings";
        public string Editor { get; set; }
    }
    public interface IAnalyticTablePermanentFilterContext
    {

    }
    public class AnalyticTablePermanentFilterContext: IAnalyticTablePermanentFilterContext
    {

    }
}
