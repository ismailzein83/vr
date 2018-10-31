using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class DemoCurrencyInfoFilter
    {
        public List<IDemoCurrencyInfoFilter> Filters { get; set; }
    }
    public interface IDemoCurrencyInfoFilter
    {
        bool IsMatch(IDemoCurrencyInfoFilterContext context);
    }

    public interface IDemoCurrencyInfoFilterContext
    {
        long DemoCurrencyId { get; set; }
    }
    public class DemoCurrencyInfoFilterContext : IDemoCurrencyInfoFilterContext
    {
        public long DemoCurrencyId { get; set; }
    }
}
