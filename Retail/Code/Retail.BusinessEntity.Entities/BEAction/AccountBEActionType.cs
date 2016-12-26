using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEActionType : BEActionType
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid AccountBEDefinitionId { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        public AccountBEActionSettings ActionSettings { get; set; }
    }

    public abstract class AccountBEActionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string ClientActionName { get; }
    }
}
