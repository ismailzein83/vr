using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEConditions
{
    public class GenericFilterGroupCondition : GenericBECondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("D89CB0E3-BD36-409A-965B-3E6AFBCAD4C9"); }
        }
        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public override bool IsMatch(IGenericBEConditionContext context)
        {
            RecordFilterManager recordFilterManager = new RecordFilterManager();

            var genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            var fieldTyes = genericBusinessEntityDefinitionManager.GetDataRecordTypeFields(context.DefinitionSettings.DataRecordTypeId);
            var dataRecordDictFilterGenericFieldMatchContext = new DataRecordDictFilterGenericFieldMatchContext(context.Entity.FieldValues, fieldTyes);
            return recordFilterManager.IsFilterGroupMatch(this.FilterGroup, dataRecordDictFilterGenericFieldMatchContext);
        }
    }
}
