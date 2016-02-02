using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberPrefixes : GenericConfiguration
    {
        public List<NumberPrefix> Prefixes { get; set; }
        public override string OwnerKey
        {
            get { return null; }
        }
    }
    public class NumberPrefix
    {
        public string Prefix { get; set; }
    }


}
