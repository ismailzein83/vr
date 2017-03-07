﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public interface IGenericRuleManager
    {
        IDataRetrievalResult<GenericRuleDetail> GetFilteredRules(DataRetrievalInput<GenericRuleQuery> input);
        GenericRule GetGenericRule(int ruleId, bool isViewedFromUI);

        IEnumerable<GenericRule> GetGenericRulesByDefinitionId(Guid ruleDefinitionId);

        bool TryAddGenericRule(GenericRule rule);
        bool TryUpdateGenericRule(GenericRule rule);

        Vanrise.Entities.InsertOperationOutput<GenericRuleDetail> AddGenericRule(GenericRule rule);

        Vanrise.Entities.UpdateOperationOutput<GenericRuleDetail> UpdateGenericRule(GenericRule rule);

        Vanrise.Entities.DeleteOperationOutput<GenericRuleDetail> DeleteGenericRule(int ruleId);
    }
}
