using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class ReplaceStringActionSettings : NormalizeNumberActionSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("12a627f4-5e64-4957-b3f9-e0b890955037"); } }
        public string StringToReplace { get; set; }

        public string NewString { get; set; }

        public bool IgnoreCase { get; set; }

        public override string GetDescription()
        {
            return string.Format("Replace String: String To Replace = {0}, NewString = {1}", this.StringToReplace, this.NewString);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (String.IsNullOrEmpty(this.StringToReplace))
                throw new NullReferenceException("this.StringToReplace");
            if (this.NewString == null)
                this.NewString = String.Empty;

            target.PhoneNumber = (this.IgnoreCase) ?
                Regex.Replace(target.PhoneNumber, this.StringToReplace, this.NewString, RegexOptions.IgnoreCase) :
                target.PhoneNumber.Replace(this.StringToReplace, this.NewString);
        }
    }
}
