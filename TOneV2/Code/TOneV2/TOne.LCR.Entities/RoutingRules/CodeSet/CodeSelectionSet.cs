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


        public override List<MatchCode> GetMatchedCodes()
        {
            throw new NotImplementedException();
        }
    }
}
