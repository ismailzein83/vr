using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.MainExtensions.StyleFormatingSettings
{
    public class CSSClass : Entities.StyleFormatingSettings
    {
        public string ClassName { get; set; }
        public override string UniqueName { get { return "VR_AccountBalance_StyleFormating_CSSClass"; } }
    }
}
