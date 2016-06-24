using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ActionDefinition
    {
        public Guid ActionDefinitionId { get; set; }

        public string Name { get; set; }
    }

    public abstract class ActionDefinitionSettings
    {
        public int ConfigId { get; set; }
    }
}
