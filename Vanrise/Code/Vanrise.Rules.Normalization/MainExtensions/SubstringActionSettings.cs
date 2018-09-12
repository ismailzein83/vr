using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public enum SubstringStartDirection { FromLeft = 0, FromRight = 1 }
    public class SubstringActionSettings : NormalizeNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("b285d8dd-b628-4df0-b28c-114ebb9bed5a"); } }

        public SubstringStartDirection StartDirection { get; set; }

        public int StartIndex { get; set; }

        public int? Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}{1} {2}", StartIndex, Length.HasValue ? ", Length =" : string.Empty, Length);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (string.IsNullOrEmpty(target.PhoneNumber))
                return;
            if (this.StartIndex >= 1 && this.StartIndex <= target.PhoneNumber.Length)
            {
                var startIndex = this.StartIndex - 1;
                int maxValidSubStringLength = target.PhoneNumber.Length - startIndex;

                target.PhoneNumber = (this.Length.HasValue && this.Length.Value <= maxValidSubStringLength) ?
                    target.PhoneNumber.Substring(startIndex, this.Length.Value) :
                    target.PhoneNumber = target.PhoneNumber.Substring(startIndex, maxValidSubStringLength);
            }

            //if (target.PhoneNumber.Length > this.StartIndex - 1)
            //    target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex - 1, Math.Min(target.PhoneNumber.Length - this.StartIndex - 1, (this.Length != null && (int)this.Length <= target.PhoneNumber.Length) ? (int)this.Length : target.PhoneNumber.Length));
        }
    }
}
