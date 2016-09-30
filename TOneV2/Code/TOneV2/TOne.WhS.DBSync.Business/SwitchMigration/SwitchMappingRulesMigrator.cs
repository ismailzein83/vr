using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business.SwitchMigration
{
    public class SwitchMappingRulesMigrator
    {
        public static SwitchLogger Logger { get; set; }
        private string ConnectionString { get; set; }
        private List<SwitchMappingRules> SwitchMappingRules { get; set; }
        private Dictionary<string, CarrierAccount> CarrierAccounts { get; set; }

        public SwitchMappingRulesMigrator(string connectionString)
        {
            ConnectionString = connectionString;
            Logger = new SwitchLogger
            {
                InfoMessage = new StringBuilder(),
                WarningMessage = new StringBuilder(),
                InParsedMappingFailedCount = 0,
                InParsedMappingSuccededCount = 0,
                OutParsedMappingFailedCount = 0,
                OutParsedMappingSuccededCount = 0
            };
        }
        public List<SwitchMappingRules> LoadSwitches()
        {
            SwitchMappingRulesManager manager = new SwitchMappingRulesManager(ConnectionString);
            SwitchMappingRules = manager.LoadSwitches();
            return SwitchMappingRules;
        }
        public void Migrate(string switchId, string parser, DateTime date)
        {
            List<InParsedMapping> inParsedMappings = new List<InParsedMapping>();
            List<OutParsedMapping> outParsedMappings = new List<OutParsedMapping>();
            var switchDictionnary = SwitchMappingRules.ToDictionary(it => it.Id.ToString(), it => it);
            SwitchContext context = new SwitchContext
            {
                ConnectionString = ConnectionString,
                SwitchId = switchId,
                Parser = parser,
                BED = date
            };
            SwitchMappingRules currentSwitch = switchDictionnary.ContainsKey(context.SwitchId)
                ? switchDictionnary[context.SwitchId]
                : null;

            if (currentSwitch == null)
            {
                Logger.Success = false;
                Logger.InfoMessage.AppendLine("No matching Switch exists");
                return;
            }

            switch (context.Parser)
            {
                case "TOne.WhS.DBSync.Business.TelesSwitchParser":
                    TelesSwitchParser telesSwitchParser = new TelesSwitchParser(currentSwitch.Configuration);
                    telesSwitchParser.GetParsedMappings(out inParsedMappings, out outParsedMappings);
                    break;
            }
            MigrateToToneV2(inParsedMappings, outParsedMappings, context.BED, context.SwitchId);
        }

        #region private functions
        private Dictionary<string, Switch> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetAllSwitches().Where(it => it.SourceId != null).ToDictionary(it => it.SourceId, it => it);
        }
        private Dictionary<string, CarrierAccount> GetAllCarriers()
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetAllCarriers().Where(it => it.SourceId != null).ToDictionary(it => it.SourceId, it => it);
        }
        private InsertOperationOutput<GenericRuleDetail> AddGenericRule(GenericRule rule)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(new Guid("E1ADF1F2-6BC3-4541-8DE4-E5F578A79372"));

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            var manager = Activator.CreateInstance(managerType) as IGenericRuleManager;

            return manager.AddGenericRule(rule);
        }
        private void MigrateToToneV2(List<InParsedMapping> inParsedMappings, List<OutParsedMapping> outParsedMappings, DateTime date, string switchId)
        {
            try
            {

                CarrierAccounts = GetAllCarriers();
                Dictionary<string, Switch> switches = GetSwitches();
                int currentSwitchId;
                if (switches.ContainsKey(switchId)) currentSwitchId = switches[switchId].SwitchId;
                else
                {
                    Logger.Success = false;
                    Logger.InfoMessage.AppendLine("No matching Switch exists");
                    return;
                }
                #region customer
                foreach (var elt in inParsedMappings)
                {
                    if (!elt.InTrunk.Values.Any())
                        continue;
                    MappingRule rule = GetRule(elt.CustomerId, elt.InTrunk, currentSwitchId, date, 1);
                    if (rule == null) continue;
                    var output = AddGenericRule(rule);
                    if (output.Result == InsertOperationResult.Succeeded) Logger.InParsedMappingSuccededCount++;
                    if (output.Result == InsertOperationResult.Failed) Logger.InParsedMappingFailedCount++;
                }
                #endregion

                #region supplier
                foreach (var elt in outParsedMappings)
                {
                    if (!elt.OutTrunk.Values.Any())
                        continue;
                    MappingRule rule = GetRule(elt.SupplierId, elt.OutTrunk, currentSwitchId, date, 2);
                    if (rule == null) continue;
                    var output = AddGenericRule(rule);
                    if (output.Result == InsertOperationResult.Succeeded) Logger.OutParsedMappingSuccededCount++;
                    if (output.Result == InsertOperationResult.Failed) Logger.OutParsedMappingFailedCount++;
                }
                #endregion

            }
            catch (Exception exc)
            {
                Logger.InfoMessage.AppendLine(exc.ToString());
            }
        }
        private MappingRule GetRule(string carrierId, StaticValues inOutCarrier, int currentSwitchId, DateTime date, int carrierType)
        {
            int carrierAccountId;
            if (CarrierAccounts.ContainsKey(carrierId)) carrierAccountId = CarrierAccounts[carrierId].CarrierAccountId;
            else
            {
                Logger.WarningMessage.AppendLine(string.Format("Carrier ID {0} does not have any matching in ToneV2", carrierId));
                return null;
            }
            MappingRule rule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = carrierAccountId
                },
                DefinitionId = new Guid("E1ADF1F2-6BC3-4541-8DE4-E5F578A79372"),
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = "Switch Migration",
                BeginEffectiveTime = date

            };
            rule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = ((new List<long> { carrierType }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = ((new List<int> { currentSwitchId }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Carrier"] = inOutCarrier;
            return rule;

        }
        #endregion
    }


}
