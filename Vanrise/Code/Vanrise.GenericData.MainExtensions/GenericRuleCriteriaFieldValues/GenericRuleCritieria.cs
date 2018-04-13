using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Business.GenericRules;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericRuleCritieria : IGenericRuleCriteriaManager
    {
        public Entities.GenericRuleCriteriaFieldValues GetCriteriaFieldValues(Object criteria)
        {
            var values = new List<object>();
            values.Add(criteria);
            Entities.GenericRuleCriteriaFieldValues staticValues = new StaticValues
            {
                Values = values
            };
            return staticValues;
        }
    }
}

