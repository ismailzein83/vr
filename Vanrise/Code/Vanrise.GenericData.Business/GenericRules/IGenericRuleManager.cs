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

        Vanrise.Entities.InsertOperationOutput<GenericRule> AddGenericRule(GenericRule rule);
    }
}
