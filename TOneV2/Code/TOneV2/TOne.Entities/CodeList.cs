using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public class CodeList
    {
        public CodeList(IEnumerable<string> distinctCodes)
        {
            this.DistinctCodes = distinctCodes;
            this.CodesWithPossibleMatches = new Dictionary<string, List<string>>();
            foreach (string distinctCode in distinctCodes)
            {
                List<string> possibleMatches = new List<string>();
                string match = distinctCode;
                while (match.Length > 0)
                {
                    possibleMatches.Add(match);
                    match = match.Substring(0, match.Length - 1);
                }
                CodesWithPossibleMatches.Add(distinctCode, possibleMatches);
            }
        }

        public IEnumerable<string> DistinctCodes { get; private set; }

        public Dictionary<string, List<string>> CodesWithPossibleMatches { get;  private set; }
    }
}
