using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class FixedPrefixes : GenericConfiguration
    {
        public List<FixedPrefix> Prefixes { get; set; }
        public override string OwnerKey
        {
            get { return null; }
        }
    }
    public class FixedPrefix
    {
        public int ID { get; set; }
        public string Prefix { get; set; }
    }
    public class FixedPrefixDetail
    {
        public FixedPrefix Entity { get; set; }
    }

    public class FixedPrefixInfo
    {
        public int ID { get; set; }
        public string Prefix { get; set; }

    }

}
