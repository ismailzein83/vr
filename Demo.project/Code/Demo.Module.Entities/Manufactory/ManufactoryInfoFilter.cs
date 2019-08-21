using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ManufactoryInfoFilter
    {
        public List<IManufactoryInfoFilter> Filters { get; set; }
    }

    public interface IManufactoryInfoFilter
    {
        bool IsMatch(IManufactoryInfoFilterContext context);
    }

    public interface IManufactoryInfoFilterContext
    {
        int Id { get;  }
    }

    public class ManufactoryInfoFilterContext : IManufactoryInfoFilterContext
    {
        public int Id { get; set; }
    }

}
