using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Range
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public string Name { get; set; }
        public Guid StyleDefinitionId { get; set; }
    }
}
