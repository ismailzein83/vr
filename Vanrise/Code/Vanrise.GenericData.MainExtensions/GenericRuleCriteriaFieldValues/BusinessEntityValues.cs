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
            dynamic dynVar = new { };
            dynVar.Id = 4;
            dynVar.Name = "teerr";
            dynVar.Id = dynVar.Name;
            int i = dynVar.Tss;
            //DisplayValues(dynVar.Id, dynVar.Name);
            var context = new BusinessEntityGroupContext();
            return this.BusinessEntityGroup.GetIds(context);
        }

        public override string GetDescription()
        {
            var context = new BusinessEntityGroupContext();
            return this.BusinessEntityGroup.GetDescription(context);
        }
    }
}
