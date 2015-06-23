using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class VisualElement
    {
        public VisualElementSettings settings { get; set; }
        public string directive { get; set; }
        public int numberOfColumns { get; set; }
    }
}
