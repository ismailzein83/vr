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

        public List<string> ExcludedCodes { get; set; }

        public override CodeSetMatch GetMatch()
        {
            CodeSetMatch match = new CodeSetMatch();
            switch(this.ZoneIds.SelectionOption)
            {
                case MultipleSelectionOption.AllExceptItems: 
                match.IsMatchingAllZones = true; 
                break;
                case MultipleSelectionOption.OnlyItems: 
                    match.MatchZoneIds = this.ZoneIds.SelectedValues;
                    break;
            }
            return match;
        }

        public override bool IsZoneExcluded(int zoneId)
        {
            return this.ZoneIds.SelectionOption == MultipleSelectionOption.AllExceptItems && this.ZoneIds.SelectedValues != null && this.ZoneIds.SelectedValues.Contains(zoneId);
        }

        public override bool IsCodeExcluded(string code)
        {
            return this.ExcludedCodes != null && this.ExcludedCodes.Contains(code);
        }

        public override string Description
        {
            get { return String.Format("Zones: {0}", String.Join(",", ZoneIds.SelectedValues.Select<int, string>(itm => itm.ToString()))); }
        }
    }
}
