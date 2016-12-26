using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("70D4A6AD-10CC-4F0B-8364-7D8EF3C044C4"); } }

        public override string DefinitionEditor
        { 
            get { return "retail-be-accountbedefinitions-editor"; } 
        }

        public override string IdType
        {
            get { return "System.Int64"; }
        }

        public override string ManagerFQTN
        {
            get { return "Retail.BusinessEntity.Business.AccountManager, Retail.BusinessEntity.Business"; }
        }

        public override string SelectorUIControl
        {
            get { return ""; }
        }

        public Guid StatusGroupId { get; set; }

        public AccountGridDefinition GridDefinition { get; set; }

        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }

        public FixedChargingDefinition FixedChargingDefinition { get; set; }
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
