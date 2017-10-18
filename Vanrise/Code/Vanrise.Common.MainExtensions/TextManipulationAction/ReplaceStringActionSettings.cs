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
        public override Guid ConfigId { get { return  new Guid("6E193BD6-4B98-4EA6-AA9B-934C65B59810"); } }
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
                return;
                //throw new NullReferenceException("this.StringToReplace");

            if (this.NewString == null)
                this.NewString = String.Empty;

            if (string.IsNullOrEmpty(target.TextValue))
                return;

            target.TextValue = (this.IgnoreCase) ?
                Utilities.ReplaceString(target.TextValue, this.StringToReplace, this.NewString, StringComparison.OrdinalIgnoreCase):
                target.TextValue.Replace(this.StringToReplace, this.NewString);
        }
    }
}
