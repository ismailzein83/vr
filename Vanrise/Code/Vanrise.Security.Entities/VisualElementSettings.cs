using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class VisualElementSettings
    {
        public string operationType { get; set; }
        public string entityType { get; set; }
        public List<string> measureTypes { get; set; }
    }
}
