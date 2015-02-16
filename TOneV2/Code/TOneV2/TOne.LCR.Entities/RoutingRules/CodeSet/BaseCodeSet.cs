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
        public abstract List<MatchCode> GetMatchedCodes();
    }

    public class MatchCode
    {
        public string Code { get; set; }

        public CodeMatchType MatchType { get; set; }
    }
}
