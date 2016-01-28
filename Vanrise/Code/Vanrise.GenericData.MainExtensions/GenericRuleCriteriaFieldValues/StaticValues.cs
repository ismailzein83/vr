using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues
{
    public class StaticValues : Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues
    {
        public IEnumerable<object> Values { get; set; }
        public override IEnumerable<object> GetValues()
        {
            return this.Values;
        }

        public override string GetDescription()
        {
            return this.Values != null ? String.Join(",", this.Values) : null;
        }
    }
}
