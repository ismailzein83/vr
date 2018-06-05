using System;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IRoomDataManager : IDataManager
    {
        bool AreCompaniesUpdated(ref object updateHandle);
        List<Room> GetRooms();
        bool Insert(Room room, out long insertedId);
        bool Update(Room room);
    }
}
