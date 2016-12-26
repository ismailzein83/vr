using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountServiceBEActionType : BEActionType
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public AccountServiceCondition AvailabilityCondition { get; set; }

        public AccountServiceBEActionSettings ActionSettings { get; set; }
    }

    public abstract class AccountServiceBEActionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string ClientActionName { get; }
    }
}
