using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class SubstringActionSettings : NormalizeNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("b285d8dd-b628-4df0-b28c-114ebb9bed5a"); } }

        public SubstringStartDirection StartDirection { get; set; }

        public int StartIndex { get; set; }

        public int? Length { get; set; }

        public override string GetDescription()
        {
            string startDirection = Utilities.GetEnumDescription(this.StartDirection);
            return string.Format("Substring {0}: Start Index = {1}{2} {3}", startDirection, StartIndex, Length.HasValue ? ", Length =" : string.Empty, Length);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (string.IsNullOrEmpty(target.PhoneNumber))
                return;

            if (this.StartIndex < 1)
                throw new NotSupportedException(string.Format("Invalid value for Start Index: {0}", this.StartIndex));

            if (this.StartIndex > target.PhoneNumber.Length)
            {
                target.PhoneNumber = null;
                return;
            }

            int startIndex;
            int maxValidSubStringLength;

            switch (this.StartDirection)
            {
                case SubstringStartDirection.FromLeft:
                    startIndex = this.StartIndex - 1;
                    maxValidSubStringLength = target.PhoneNumber.Length - startIndex;

                    if (this.Length.HasValue && this.Length.Value <= maxValidSubStringLength)
                        target.PhoneNumber = target.PhoneNumber.Substring(startIndex, this.Length.Value);
                    else
                        target.PhoneNumber = target.PhoneNumber.Substring(startIndex, maxValidSubStringLength);
                    break;

                case SubstringStartDirection.FromRight:
                    int fromRightStartIndex = target.PhoneNumber.Length - this.StartIndex;
                    maxValidSubStringLength = fromRightStartIndex + 1;

                    if (this.Length.HasValue && this.Length.Value <= maxValidSubStringLength)
                    {
                        startIndex = fromRightStartIndex - this.Length.Value + 1;
                        target.PhoneNumber = target.PhoneNumber.Substring(startIndex, this.Length.Value);
                    }
                    else
                    {
                        startIndex = 0;
                        target.PhoneNumber = target.PhoneNumber.Substring(startIndex, maxValidSubStringLength);
                    }
                    break;

                default: throw new NotSupportedException(string.Format("Start Direction '{0}'", this.StartDirection));
            }
        }
    }

    public enum SubstringStartDirection
    {
        [Description("From Left")]
        FromLeft = 0,
        [Description("From Right")]
        FromRight = 1
    }
}