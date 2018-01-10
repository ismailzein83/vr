using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordType
    {
        public Guid DataRecordTypeId { get; set; }

        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public List<DataRecordField> Fields { get; set; }

        public DataRecordTypeSettings Settings { get; set; }

        public DataRecordTypeExtraField ExtraFieldsEvaluator { get; set; }
    }

    public class DataRecordTypeSettings
    {
        public string DateTimeField { get; set; }
        public string IdField { get; set; }
    }

    public abstract class DataRecordTypeExtraField
    {
        public abstract Guid ConfigId { get; }
        public abstract List<DataRecordField> GetFields(IDataRecordExtraFieldContext context);
    }

    public interface IDataRecordExtraFieldContext
    {

    }
}
