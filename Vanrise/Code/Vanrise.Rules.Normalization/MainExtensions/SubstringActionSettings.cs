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

        public int? Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex-1, Length);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (target.PhoneNumber.Length > this.StartIndex - 1)
                target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex - 1, Math.Min(target.PhoneNumber.Length - this.StartIndex - 1, (this.Length != null && (int)this.Length <= target.PhoneNumber.Length) ? (int)this.Length : target.PhoneNumber.Length));
        }
    }
}
