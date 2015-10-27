using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class AddPrefixActionSettings : NormalizeNumberActionSettings
    {
        public string Prefix { get; set; }

        public override string GetDescription()
        {
            return string.Format("Add Prefix: Prefix = {0}", Prefix);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            target.PhoneNumber = String.Format("{0}{1}", this.Prefix, target.PhoneNumber);
        }
    }
}
