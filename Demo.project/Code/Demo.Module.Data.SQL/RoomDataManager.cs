using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;
using Demo.Module.Entities.Room;

namespace Demo.Module.Data.SQL
{
    public class RoomDataManager : BaseSQLDataManager,IRoomDataManager
    {
        public RoomDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Room> GetRooms()
        {
            return GetItemsSP("[dbo].[sp_Room_GetAll]", RoomMapper);
        }
        public bool Insert (Room room , out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Room_Insert]", out id, room.Name,room.BuildingId);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }
        public bool Update(Room room)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Room_Update]", room.RoomId, room.Name, room.BuildingId);
            return (nbOfRecordsAffected > 0);
        }
        public bool Delete(int roomId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Room_Delete]", roomId);
            return (nbOfRecordsAffected > 0);
        }
        public bool AreRoomsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Room]", ref updateHandle);
        }
        Room RoomMapper(IDataReader reader)
        {
            Room room = new Room();
            room.RoomId = GetReaderValue<int>(reader, "ID");
            room.Name = GetReaderValue<string>(reader, "Name");
            room.BuildingId = GetReaderValue<int>(reader, "BuildingId");
            return room;
        }
    }
}
