using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class DemoCountryInfoFilter
    {
        public List<IDemoCountryInfoFilter> Filters { get; set; }
    }
    public interface IDemoCountryInfoFilter
    {
        bool IsMatch(IDemoCountryInfoFilterContext context);
    }

    public interface IDemoCountryInfoFilterContext
    {
        long DemoCountryId { get; set; }
    }
    public class DemoCountryInfoFilterContext : IDemoCountryInfoFilterContext
    {
        public long DemoCountryId { get; set; }
    }
}
