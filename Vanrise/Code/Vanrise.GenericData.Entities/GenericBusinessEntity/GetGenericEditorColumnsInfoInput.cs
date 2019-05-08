using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GetGenericEditorColumnsInfoInput
    {
        public VRGenericEditorDefinitionSetting GenericEditorDefinitionSetting { get; set; }
        public Guid DataRecordTypeId { get; set; }
    }
}
