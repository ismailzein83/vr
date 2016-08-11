using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class TextManipulationSettings
    {
        public List<TextManipulationActionSettings> Actions { get; set; }

        public List<string> GetDescriptions()
        {
            if (this.Actions == null) return null;

            List<string> descriptions = new List<string>();

            foreach (TextManipulationActionSettings action in this.Actions)
            {
                descriptions.Add(action.GetDescription());
            }

            return descriptions;
        }

        public void ApplyNormalizationRule(ITextManipulationContext context)
        {
            var target = new TextManipulationTarget
            {
                TextValue = context.InputText
            };
            foreach (var action in this.Actions)
            {
                action.Execute(null, target);
            }
            context.OutputText = target.TextValue;
        }
    }
}
