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
        public int ID { get; set; }
        public string Prefix { get; set; }
    }
    public class NumberPrefixDetail
    {
        public NumberPrefix Entity { get; set; }
    }

    public class NumberPrefixInfo
    {
        public int ID { get; set; }
        public string Prefix { get; set; }

    }

}
