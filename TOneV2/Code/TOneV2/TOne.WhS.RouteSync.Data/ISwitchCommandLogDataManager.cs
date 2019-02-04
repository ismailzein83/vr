using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Data
{
    public interface ISwitchCommandLogDataManager : IDataManager
    {
        bool Insert(SwitchCommandLog switchCommandLog, out long insertedId);
    }
}