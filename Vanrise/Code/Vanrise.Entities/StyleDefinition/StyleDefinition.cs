using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class StyleDefinition
    {
        public Guid StyleDefinitionId { get; set; }
        public string Name { get; set; }
        public StyleDefinitionSettings StyleDefinitionSettings { get; set; }
    }

    public class StyleDefinitionSettings
    {
        public StyleFormatingSettings StyleFormatingSettings { get; set; }
    }

    public abstract class StyleFormatingSettings
    {
        public Guid ConfigId { get; set; }
        public virtual string UniqueName { get; set; }
    }
}
