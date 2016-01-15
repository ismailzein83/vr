using System;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CDRDetail
    {
        public CDR Entity { get; set; }
        public string CallClassName { get; set; }
        public string CallTypeName { get; set; }
        public string SubscriberTypeName { get; set; }
    }
}




