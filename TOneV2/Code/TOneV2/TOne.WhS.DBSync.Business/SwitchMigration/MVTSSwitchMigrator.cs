using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MVTSSwitchMigrator : SwitchRulesMigrator
    {
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            throw new NotImplementedException();
        }
    }
}
