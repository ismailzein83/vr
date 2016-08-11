using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.TextManipulationAction
{
    public class SubstringActionSettings : TextManipulationActionSettings
    {
        public int StartIndex { get; set; }

        public int? Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex - 1, Length);
        }

        public override void Execute(ITextManipulationActionContext context, TextManipulationTarget target)
        {
            if (this.StartIndex >= 1 && this.StartIndex <= target.TextValue.Length)
            {
                int maxValidSubStringLength = target.TextValue.Length - this.StartIndex + 1; // +1 to include the first number in the max valid length

                target.TextValue = (this.Length.HasValue && this.Length.Value <= maxValidSubStringLength) ?
                    target.TextValue.Substring(this.StartIndex - 1, this.Length.Value) :
                    target.TextValue = target.TextValue.Substring(this.StartIndex - 1, maxValidSubStringLength);
            }
        }
    }
}
