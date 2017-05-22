using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealDefinitionManager : BaseDealManager
    {
        #region Public Methods
        public IEnumerable<DealDefinitionInfo> GetDealDefinitionInfo(DealDefinitionFilter filter)
        {
            var cachedDeals = base.GetCachedDeals();

            Func<DealDefinition, bool> filterExpression = (dealDefinition) =>
            {
                if (filter!=null && filter.Filters != null)
                {
                    DealDefinitionFilterContext context = new DealDefinitionFilterContext() { DealDefinition = dealDefinition };
                    foreach (IDealDefinitionFilter dealDefinitionFilter in filter.Filters)
                    {
                        if (!dealDefinitionFilter.IsMatched(context))
                            return false;
                    }
                }

                return true;
            };

            return cachedDeals.MapRecords(DealDefinitionInfoMapper, filterExpression).OrderBy(item => item.Name);
        }
        #endregion

        #region Mappers

        public DealDefinitionInfo DealDefinitionInfoMapper(DealDefinition dealDefinition)
        {
            return new DealDefinitionInfo()
            {
                DealId = dealDefinition.DealId,
                Name = dealDefinition.Name
            };
        }
        #endregion

        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            throw new NotImplementedException();
        }

        public override BaseDealManager.BaseDealLoggableEntity GetLoggableEntity()
        {
            throw new NotImplementedException();
        }
    }
}