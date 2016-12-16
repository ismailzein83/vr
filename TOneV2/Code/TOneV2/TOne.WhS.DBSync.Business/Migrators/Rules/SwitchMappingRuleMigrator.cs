using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.GenericData.Normalization;
using Vanrise.GenericData.Transformation;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchMappingRuleMigrator : RuleBaseMigrator
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        int _mappingRuleTypeId;

        readonly Dictionary<string, Switch> _allSwitches;
        public SwitchMappingRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            _mappingRuleTypeId = mappingRuleManager.GetRuleTypeId();

            var dbTableSwitches = Context.MigrationContext.DBTables[DBTableName.Switch];
            _allSwitches = (Dictionary<string, Switch>)dbTableSwitches.Records;

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
                      
        }

        public override IEnumerable<SourceRule> GetSourceRules()
        {
            //TODO
            //Get Switch Mapping From DataManager
            //Generate Mapping Rules
            return null;
        }
    }
}
