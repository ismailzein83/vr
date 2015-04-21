using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
    public abstract class BaseCodeSet
    {
        public abstract string GetDescription(IBusinessEntityInfoManager businessEntityManager);
        public virtual CodeSetMatch GetMatch()
        {
            return null;
        }

        public virtual bool IsCodeExcluded(string code)
        {
            return false;
        }

        public virtual bool IsZoneExcluded(int zoneId)
        {
            return false;
        }
    }

    public class CodeSetMatch
    {
        public bool IsMatchingAllZones { get; set; }

        public List<int> MatchZoneIds { get; set; }

        /// <summary>
        /// key is code, value is withsubcodes flag
        /// </summary>
        public Dictionary<string, bool> MatchCodes { get; set; }
    }
}
