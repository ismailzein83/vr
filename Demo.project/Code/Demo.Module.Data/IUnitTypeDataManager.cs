using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace  Demo.Module.Data
{
    public interface IUnitTypeDataManager : IDataManager
    {
        IEnumerable<UnitType> GetAllUnitTypes();

        
        bool AreUnitTypeUpdated(ref object updateHandle);
    }
}
