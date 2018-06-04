using System;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IBuildingDataManager : IDataManager
    {
        bool AreCompaniesUpdated(ref object updateHandle);
        List<Building> GetBuildings();
        bool Insert(Building building, out long insertedId);
        bool Update(Building building);
    }
}
