using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.TextManipulationAction
{
    public class AddPrefixActionSettings : TextManipulationActionSettings
    {
        public string Prefix { get; set; }

        public override string GetDescription()
        {
            return string.Format("Add Prefix: Prefix = {0}", Prefix);
        }

        public override void Execute(ITextManipulationActionContext context, TextManipulationTarget target)
        {
            target.TextValue = String.Format("{0}{1}", this.Prefix, target.TextValue);
        }
    }
}
