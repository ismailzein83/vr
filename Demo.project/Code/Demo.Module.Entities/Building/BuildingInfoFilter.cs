using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class BuildingInfoFilter
    {
        public List<IBuildingInfoFilter> Filters { get; set; }
    }
    public interface IBuildingInfoFilter
    {
        bool IsMatch(IBuildingInfoFilterContext context);
    }

    public interface IBuildingInfoFilterContext
    {
        long BuildingId { get; set; }
    }
    public class BuildingInfoFilterContext : IBuildingInfoFilterContext
    {
        public long BuildingId { get; set; }
    }
}
