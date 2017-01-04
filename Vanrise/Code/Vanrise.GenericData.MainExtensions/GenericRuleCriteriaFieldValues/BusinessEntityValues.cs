using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues
{
    public class BusinessEntityValues : Vanrise.GenericData.Entities.GenericRuleCriteriaFieldValues
    {
        public IBusinessEntityGroup BusinessEntityGroup { get; set; }

        public override IEnumerable<object> GetValues()
        {
            var context = new BusinessEntityGroupContext();
            return this.BusinessEntityGroup.GetIds(context);
        }

        public string GetDescription()
        {
            var context = new BusinessEntityGroupContext();
            return this.BusinessEntityGroup.GetDescription(context);
        }
    }
}