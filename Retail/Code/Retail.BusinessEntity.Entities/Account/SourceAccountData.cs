using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourceAccountData : ITargetBE
    {
        public Account Account { get; set; }
        public object SourceBEId
        {
            get { return this.Account.SourceId; }
        }
        public object TargetBEId
        {
            get { return this.Account.AccountId; }
        }
        public List<MappingRule> IdentificationRulesToInsert { get; set; }
        public List<MappingRule> IdentificationRulesToUpdate { get; set; }
    }
}
