using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class NumberPrefixesManager : GenericConfigurationManager<NumberPrefixes>
    {
        private NumberPrefixes GetCachedNumberPrefixes()
        {
            var numberPrefixes = base.GetConfiguration(null);
            if (numberPrefixes.Prefixes == null)
            {
                numberPrefixes.Prefixes = new List<NumberPrefix>();
            }
            return numberPrefixes;
        }


        public IEnumerable<NumberPrefix> GetPrefixes()
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            return cachedPrefixes.Prefixes;
        }

        public List<string> GetLeafPrefixes()
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            List<NumberPrefix> prefixes = cachedPrefixes.Prefixes.ToList();
            foreach (var i in cachedPrefixes.Prefixes)
            {
                foreach (var j in cachedPrefixes.Prefixes)
                {
                    if (j.Prefix.StartsWith(i.Prefix) && i.Prefix != j.Prefix)
                    {
                        prefixes.Remove(i);
                    }
                }
            }

            return prefixes.Select(x => x.Prefix).ToList();
        }

        public bool UpdatePrefixes(List<NumberPrefix> prefixes)
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            cachedPrefixes.Prefixes = prefixes;
            bool updateActionSucc = false;
            updateActionSucc = base.UpdateConfiguration(cachedPrefixes);
            return updateActionSucc;
        }


    }
}
