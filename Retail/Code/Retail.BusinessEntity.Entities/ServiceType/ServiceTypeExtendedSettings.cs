using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ServiceTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return null;
        }
    }
}
