using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class BuildingDataManager : BaseSQLDataManager, IBuildingDataManager
    {
        public BuildingDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        
        public List<Building> GetBuildings()
        {
            return GetItemsSP("[dbo].[sp_Building_GetAll]", BuildingMapper);
        }
        public bool Insert(Building building, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Building_Insert]", out id, building.Name);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }
        public bool Update(Building building)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Building_Update]", building.BuildingId, building.Name);
            return (nbOfRecordsAffected > 0);
        }
        public bool Delete(int buildingId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Building_Delete]", buildingId);
            return (nbOfRecordsAffected > 0);
        }
        public bool AreBuildingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Building]", ref updateHandle);
        }
        
        Building BuildingMapper(IDataReader reader)
        {
            Building building = new Building();
            building.BuildingId = GetReaderValue<int>(reader, "ID");
            building.Name = GetReaderValue<string>(reader, "Name");
            return building;
        }
    }
}
