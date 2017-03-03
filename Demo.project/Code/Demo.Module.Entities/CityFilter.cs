using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{

    public class CityFilter
    {
        public IEnumerable<int> ExcludedCityIds { get; set; }

        public IEnumerable<ICityFilter> Filters { get; set; }
    }

    public interface ICityFilter
    {
        bool IsExcluded(ICityFilterContext context);
    }

    public interface ICityFilterContext
    {
       City city { get; }

        object CustomObject { get; set; }
    }

    public class CityFilterContext : ICityFilterContext
    {
        public City city { get; set; }

        public object CustomObject { get; set; }
    }
}
