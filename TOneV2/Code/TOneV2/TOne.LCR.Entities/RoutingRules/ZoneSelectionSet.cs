using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class ZoneSelectionSet : BaseCodeSet
    {
        public MultipleSelectionOption ZoneSelectionOption { get; set; }

        public List<int> SelectedZoneIds { get; set; }

        public override List<string> GetCodes()
        {
            throw new NotImplementedException();
        }
    }
}
