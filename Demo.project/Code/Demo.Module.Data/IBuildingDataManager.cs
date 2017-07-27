using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IBuildingDataManager : IDataManager
    {
        bool AreBuildingsUpdated(ref object updateHandle);

        List<Building> GetBuildings();

        bool Insert(Building building, out int insertedId);
        bool Update(Building building);
        bool Delete(int Id);
    }
}
