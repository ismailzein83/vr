using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class BusinessEntityDefinitionSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string SelectorUIControl { get; set; }
        public virtual string GroupSelectorUIControl { get; set; }
        public virtual string ManagerFQTN { get; set; }
        public virtual string DefinitionEditor { get; set; }
        public virtual string IdType { get; set; }
        public virtual string SelectorFilterEditor { get; set; }
        public virtual string NullDisplayText { get; set; }
    }
}
