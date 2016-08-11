using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.TextManipulationAction
{
    public class ReplaceStringActionSettings : TextManipulationActionSettings
    {
        public string StringToReplace { get; set; }

        public string NewString { get; set; }

        public bool IgnoreCase { get; set; }

        public override string GetDescription()
        {
            return string.Format("Replace String: String To Replace = {0}, NewString = {1}", this.StringToReplace, this.NewString);
        }

        public override void Execute(ITextManipulationActionContext context, TextManipulationTarget target)
        {
            if (String.IsNullOrEmpty(this.StringToReplace))
                throw new NullReferenceException("this.StringToReplace");
            if (this.NewString == null)
                this.NewString = String.Empty;

            target.TextValue = (this.IgnoreCase) ?
                Regex.Replace(target.TextValue, this.StringToReplace, this.NewString, RegexOptions.IgnoreCase) :
                target.TextValue.Replace(this.StringToReplace, this.NewString);
        }
    }
}
