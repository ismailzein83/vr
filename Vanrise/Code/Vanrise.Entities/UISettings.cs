using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UISettings
    {
        public Dictionary<string, object> Parameters { get; set; }
    }
    public class UIParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
    public abstract class UIExtendedSettings
    {
        public abstract Dictionary<string, object> GetUIParameters();
    }
}


