using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4");
        public override Guid ConfigId { get { return s_configId; } }

        public override string ManagerFQTN
        {
            get { return "Retail.BusinessEntity.Business.AccountBEManager, Retail.BusinessEntity.Business"; }
        }

        public override string DefinitionEditor
        {
            get { return "retail-be-accountbedefinitions-editor"; }
        }

        public override string IdType
        {
            get { return "System.Int64"; }
        }

        public override string SelectorFilterEditor
        {
            get { return "retail-be-accountcondition-selective"; }
        }

        public override string SelectorUIControl
        {
            get { return "retail-be-account-selector"; }
        }

        public Guid StatusBEDefinitionId { get; set; }
        public List<AccountExtraFieldDefinition> AccountExtraFieldDefinitions { get; set; }
        public AccountGridDefinition GridDefinition { get; set; }
        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }
        public List<AccountActionDefinition> ActionDefinitions { get; set; }
        public FixedChargingDefinition FixedChargingDefinition { get; set; }
    }
    public class AccountExtraFieldDefinition
    {
        public Guid AccountExtraFieldDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountExtraFieldDefinitionSettings Settings { get; set; }
    }
    public abstract class AccountExtraFieldDefinitionSettings
    {
        public abstract Guid ConfigId  { get; }
        public abstract IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context);
    }
    public interface IAccountExtraFieldSettingsContext
    {
    }
    public abstract class AccountBEDefinitionCondition
    {
        public abstract Guid ConfigId { get; }

        public abstract bool Evaluate(IAccountConditionEvaluationContext context);
    }

    public interface IAccountBEDefinitionConditionContext
    {
        AccountBEDefinitionSettings AccountBEDefinitionSettings { get; }
    }
}
