using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class SubstringActionSettings : NormalizeNumberActionSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("b285d8dd-b628-4df0-b28c-114ebb9bed5a"); } }
        public int StartIndex { get; set; }

        public int? Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex-1, Length);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (this.StartIndex >= 1 && this.StartIndex <= target.PhoneNumber.Length)
            {
                int maxValidSubStringLength = target.PhoneNumber.Length - this.StartIndex + 1; // +1 to include the first number in the max valid length
                
                target.PhoneNumber = (this.Length.HasValue && this.Length.Value <= maxValidSubStringLength) ?
                    target.PhoneNumber.Substring(this.StartIndex - 1, this.Length.Value) :
                    target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex - 1, maxValidSubStringLength);
            }

            //if (target.PhoneNumber.Length > this.StartIndex - 1)
            //    target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex - 1, Math.Min(target.PhoneNumber.Length - this.StartIndex - 1, (this.Length != null && (int)this.Length <= target.PhoneNumber.Length) ? (int)this.Length : target.PhoneNumber.Length));
        }
    }
}
