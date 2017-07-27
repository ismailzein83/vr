using Demo.Module.Entities.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IRoomDataManager:IDataManager
    {
        List<Room> GetRooms();
        bool Insert(Room room, out int insertedId);
        bool Update(Room room);
        bool Delete(int Id);
        bool AreRoomsUpdated(ref object updateHandle);
    }
}
