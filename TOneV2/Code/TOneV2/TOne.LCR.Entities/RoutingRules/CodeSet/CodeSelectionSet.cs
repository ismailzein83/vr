using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class CodeSelectionSet : BaseCodeSet
    {
        public string Code { get; set; }

        public bool WithSubCodes { get; set; }

        public List<string> ExcludedCodes { get; set; }

        public override CodeSetMatch GetMatch()
        {
            CodeSetMatch match = new CodeSetMatch { MatchCodes = new Dictionary<string, bool>() };
            match.MatchCodes.Add(this.Code, this.WithSubCodes);
            return match;
        }

        public override bool IsCodeExcluded(string code)
        {
            return this.ExcludedCodes != null && this.ExcludedCodes.Contains(code);
        }
    }
}
