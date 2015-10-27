using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class SubstringActionSettings : NormalizeNumberActionSettings
    {
        public int StartIndex { get; set; }

        public int Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex, Length);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (target.PhoneNumber.Length > this.StartIndex)
                target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex, Math.Min(target.PhoneNumber.Length - this.StartIndex, this.Length));
        }
    }
}
