using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization
{
    public class NormalizeNumberSettings
    {
        public List<NormalizeNumberActionSettings> Actions { get; set; }

        public List<string> GetDescriptions()
        {
            if (this.Actions == null) return null;

            List<string> descriptions = new List<string>();

            foreach (NormalizeNumberActionSettings action in this.Actions)
            {
                descriptions.Add(action.GetDescription());
            }

            return descriptions;
        }
    }
}
