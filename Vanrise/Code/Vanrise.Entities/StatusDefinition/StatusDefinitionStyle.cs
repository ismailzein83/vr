using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
   public class StatusDefinitionStyle
    {
        public Guid StatusDefinitionId { get; set; }
        public string Name { get; set; }
        public StyleFormatingSettings StyleFormatingSettings { get; set; }
    }
}
