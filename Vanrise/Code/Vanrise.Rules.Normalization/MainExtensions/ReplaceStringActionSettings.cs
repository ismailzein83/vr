﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public class ReplaceStringActionSettings : NormalizeNumberActionSettings
    {
        public string StringToReplace { get; set; }

        public string NewString { get; set; }

        public bool IgnoreCase { get; set; }

        public override string GetDescription()
        {
            return string.Format("Replace String: String To Replace = {0}, NewString = {1}", this.StringToReplace, this.NewString);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            target.PhoneNumber = (this.IgnoreCase) ?
                Regex.Replace(target.PhoneNumber, this.StringToReplace, this.NewString, RegexOptions.IgnoreCase) :
                target.PhoneNumber.Replace(this.StringToReplace, this.NewString);
        }
    }
}
