using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class FilterGroupGenericBEGridCondition : GenericBEGridCondition
    {
        public override Guid ConfigId { get { return new Guid("34FB0901-6270-4EE2-901E-78F6A2C40E4F"); } }
        public RecordFilterGroup FilterGroup { get; set; }
        public override bool IsMatch(IGenericBEGridConditionContext context)
        {
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            var dataRecordDictFilterGenericFieldMatchContext = new DataRecordDictFilterGenericFieldMatchContext(context.Entity.FieldValues, context.DefinitionSettings.DataRecordTypeId);
            return recordFilterManager.IsFilterGroupMatch(FilterGroup, dataRecordDictFilterGenericFieldMatchContext);
        }
    }
}
