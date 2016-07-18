using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.Color
{
    public class ColorDefinition
    {
        public Guid ColorDefinitionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
