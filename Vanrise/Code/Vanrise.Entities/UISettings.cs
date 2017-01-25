using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UISettings
    {
        public List<UIParameter> Parameters { get; set; }
    }
    public class UIParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}


