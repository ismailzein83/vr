using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class ZoneSelectionSet : BaseCodeSet
    {
        public MultipleSelection<int> ZoneIds { get; set; }

        public override List<MatchCode> GetMatchedCodes()
        {
            throw new NotImplementedException();
        }
    }
}
