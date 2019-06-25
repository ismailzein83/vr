using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityInfoFilter
    {
        public IEnumerable<IGenericBusinessEntityFilter> Filters { get; set; }

        public List<GenericBusinessEntityFilter> FieldFilters { get; set; }

        public GenericBESelectorCondition GenericBESelectorCondition { get; set; }

        public List<Object> SelectedIds { get; set; }

        public List<Object> IncludedIds { get; set; }
    }

    public interface IGenericBusinessEntityFilter
    {
        bool IsMatch(IGenericBusinessEntityFilterContext context);
    }

    public interface IGenericBusinessEntityFilterContext
    {
        GenericBusinessEntity GenericBusinessEntity{ get; }
    }

    public class GenericBusinessEntityFilterContext : IGenericBusinessEntityFilterContext
    {
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
    }
}
