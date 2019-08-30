using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ZooInfoFilter
    {
        public List<IZooInfoFilter> Filters { get; set; }
    }

    public interface IZooInfoFilter
    {
        bool IsMatch(IZooInfoFilterContext context);
    }

    public interface IZooInfoFilterContext 
    {
        long ZooId { get; set; }
    }

    public class ZooInfoFilterContext : IZooInfoFilterContext
    {
        public long ZooId { get; set; }
    }
}
