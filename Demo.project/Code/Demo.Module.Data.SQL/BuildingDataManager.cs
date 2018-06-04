using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class BuildingDataManager : BaseSQLDataManager, IBuildingDataManager
    {

        #region Constructors
        public BuildingDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Building]", ref updateHandle);
        }
        public List<Building> GetBuildings()
        {
            return GetItemsSP("[dbo].[sp_Building_GetAll]", BuildingMapper);
        }
        public bool Insert(Building building, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Building_Insert]", out id, building.Name);
            insertedId = Convert.ToInt64(id);
            return (nbOfRecordsAffected > 0);
        }
        public bool Update(Building building)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Building_Update]", building.BuildingId, building.Name);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Building BuildingMapper(IDataReader reader)
        {
            return new Building
            {
                BuildingId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
        #endregion
    }
}