using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{   
    public class AccountBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public override string DefinitionEditor { get { throw new NotImplementedException(); } }

        public override string SelectorUIControl
        {
            get { throw new NotImplementedException(); }
        }
        public override string ManagerFQTN
        {
            get { return "Retail.BusinessEntity.Business.AccountManager, Retail.BusinessEntity.Business"; }
        }
        public override string IdType
        {
            get { return "System.Int64"; }
        }

        public Guid StatusGroupId { get; set; }

        public AccountGridDefinition GridDefinition { get; set; }

        public List<AccountViewDefinition> AccountViewDefinitions { get; set; }

        public FixedChargingDefinition FixedChargingDefinition { get; set; }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
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
