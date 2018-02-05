using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionSettings
    {
        int SupplierId { get; }

        int NumberOfTries { get; }

        int? Percentage { get; }
    }

    public interface IRouteBackupOptionSettings
    {
        int SupplierId { get; }

        int NumberOfTries { get; }
    }

    public interface IFixedRouteOptionSettings
    {
        List<RouteOptionFilterSettings> Filters { get; set; }
    }

    public interface ISpecialRequestRouteOptionSettings
    {
        bool ForceOption { get; set; }
    }
}