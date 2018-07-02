using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.ActionFilterCondition
{
    public class GenericFilterGroupCondition : GenericBEActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("E6D82564-419C-4A46-A346-774C3EA5E9AA"); }
        }
        public RecordFilterGroup FilterGroup { get; set; }
        public override bool IsFilterMatch(IGenericBEActionFilterConditionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
