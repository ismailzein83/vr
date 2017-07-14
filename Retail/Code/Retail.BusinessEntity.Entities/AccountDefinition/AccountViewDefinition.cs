using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountViewDefinition
    {
        public Guid AccountViewDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        /// <summary>
        /// should be Null if not available in drilldown
        /// </summary>
        public string DrillDownSectionName { get; set; }

        /// <summary>
        /// should be Null if not available in 360 Degree View
        /// </summary>
        public string Account360DegreeSectionName { get; set; }

        public AccountViewDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountViewDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public virtual bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return true;
        }

        public virtual bool TryConvertToBE360DegreeNode(IAccountViewDefinitionTryConvertToBE360DegreeNodeContext context)
        {
            return false;
        }
    }

    public interface IAccountViewDefinitionCheckAccessContext
    {
        Guid AccountBEDefinitionId { get; }
        int UserId { get; }
    }

    public interface IAccountViewDefinitionTryConvertToBE360DegreeNodeContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        BE360DegreeNode Node { set; }
    }
}
