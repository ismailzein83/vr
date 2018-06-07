using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class RoomDataManager : BaseSQLDataManager, IRoomDataManager
    {

        #region Constructors
        public RoomDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Room]", ref updateHandle);
        }
        public List<Room> GetRooms()
        {
            return GetItemsSP("[dbo].[sp_Room_GetAll]", RoomMapper);
        }
        public bool Insert(Room room, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Room_Insert]", out id, room.Name, room.BuildingId);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Room room)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Room_Update]", room.RoomId, room.Name, room.BuildingId);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Room RoomMapper(IDataReader reader)
        {
            return new Room
            {
                RoomId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                BuildingId = GetReaderValue<long>(reader, "BuildingId"),
            };
        }
        #endregion
    }
}