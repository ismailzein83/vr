using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class AddPrefixActionSettings : NormalizeNumberActionSettings
    {
        public override Guid ConfigId { get { return  new Guid("2b333f37-21b2-436c-92f5-cfaa9912b388"); } }
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
