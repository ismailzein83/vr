using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{

    public abstract class DataRecordStoragePermanentFilterSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract RecordFilterGroup ConvertToRecordFilter(IDataRecordStoragePermanentFilterContext context);
    }

    public class FilterGroupDataRecordStoragePermanentFilter : DataRecordStoragePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("5F841B8D-D330-496A-BC62-0720352B25B7");
        public override RecordFilterGroup ConvertToRecordFilter(IDataRecordStoragePermanentFilterContext context)
        {
            return RecordFilterGroup;
        }
        public RecordFilterGroup RecordFilterGroup { get; set; }

    }
    public class ConditionFilterDataRecordStoragePermanentFilter : DataRecordStoragePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("1AE4EAB5-C93A-48C3-ADA6-FDDCFA7F1027");
        public override RecordFilterGroup ConvertToRecordFilter(IDataRecordStoragePermanentFilterContext context)
        {
            return null;
        }
        public RecordQueryLogicalOperator LogicalOperator { get; set; }
        public List<DataRecordStoragePermanentFilterSettings> Filters { get; set; }

    }
    public class DataRecordStoragePermanentFilterSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_PermanentFilterSettings";
        public string Editor { get; set; }
    }
    public interface IDataRecordStoragePermanentFilterContext
    {

    }
    public class DataRecordStoragePermanentFilterContext : IDataRecordStoragePermanentFilterContext
    {

    }
}
