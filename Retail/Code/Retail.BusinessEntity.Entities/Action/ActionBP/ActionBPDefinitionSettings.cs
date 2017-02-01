using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ActionBPDefinitionSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string RuntimeEditor { get; set; }
    }
}
