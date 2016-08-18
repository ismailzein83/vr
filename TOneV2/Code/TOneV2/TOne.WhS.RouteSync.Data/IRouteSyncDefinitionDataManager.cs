using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Data
{
    public interface IRouteSyncDefinitionDataManager : IDataManager
    {
        List<RouteSyncDefinition> GetRouteSyncDefinitions();

        bool AreRouteSyncDefinitionsUpdated(ref object updateHandle);

        bool Insert(RouteSyncDefinition routeSyncDefinitionItem, out int insertedId);

        bool Update(RouteSyncDefinition routeSyncDefinitionItem);
    }
}
