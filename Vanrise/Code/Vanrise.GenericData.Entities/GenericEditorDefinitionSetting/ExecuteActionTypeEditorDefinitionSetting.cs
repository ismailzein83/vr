using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class ExecuteActionTypeEditorDefinitionSetting
    {
        public abstract Guid ConfigId { get; }
        public virtual string ActionRuntimeEditor { get; set; }
    }
}
