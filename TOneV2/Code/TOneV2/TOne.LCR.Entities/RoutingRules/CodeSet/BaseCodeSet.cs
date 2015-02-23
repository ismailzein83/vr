using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public enum CodeMatchType {  DirectMatch = 0, SubCodeMatch = 10, ZoneMatch = 20 }

    public abstract class BaseCodeSet
    {
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

        public virtual List<int> GetExcludedCodes()
        {
            return null;
        }

        public abstract List<MatchCode> GetMatchedCodes();

        public virtual List<Char> GetMatchCodeFirstDigits()
        {
            return null;
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

    public class MatchCode
    {
        public string Code { get; set; }

        public CodeMatchType MatchType { get; set; }
    }
}
