using Demo.Module.Data;
using Demo.Module.Entities.Building;
using Demo.Module.Entities.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class RoomManager
    {
        public IDataRetrievalResult<RoomDetails> GetFilteredRooms(DataRetrievalInput<RoomQuery> input)
        {
            var allRooms = GetCachedRooms();
            Func<Room,bool> filterExpression= (room)=>
                {
                    if(input.Query.Name!=null && !room.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;
                    if (input.Query.BuildingIds != null && !input.Query.BuildingIds.Contains(room.BuildingId))
                        return false;
                    return true;
                };
            return DataRetrievalManager.Instance.ProcessResult(input, allRooms.ToBigResult(input, filterExpression, RoomDetailMapper));
        }
        public Room GetRoomById(int roomId)
        {
            var allRooms=GetCachedRooms();
            return (allRooms.GetRecord(roomId));
        }
        public IEnumerable<RoomInfo> GetRoomsInfo(RoomInfoFilter filter)
        {
            
            var allRooms = GetCachedRooms();
            Func<Demo.Module.Entities.Room.Room, bool> filterFunc = null;
            if (filter == null || filter.BuildingIds == null)
                return null;
            else
            {
                filterFunc = (room) =>
                {
                    if (!filter.BuildingIds.Contains(room.BuildingId))
                        return false;
                    return true;
                };
            }
            IEnumerable<Demo.Module.Entities.Room.Room> filteredRooms = (filterFunc != null) ? allRooms.FindAllRecords(filterFunc) : allRooms.MapRecords(u=>u.Value);
            return filteredRooms.MapRecords(RoomInfoMapper).OrderBy(room => room.Name );
        }
        public InsertOperationOutput<RoomDetails> AddRoom (Room room)
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            InsertOperationOutput<RoomDetails> insertOperationOutput = new InsertOperationOutput<RoomDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int roomId = -1;
            bool insertActionSuccess = roomDataManager.Insert(room, out roomId);
            if(insertActionSuccess)
            {
                room.RoomId = roomId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RoomDetailMapper(room);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public UpdateOperationOutput<RoomDetails> UpdateRoom(Room room)
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            UpdateOperationOutput<RoomDetails> updateOperationOutput = new UpdateOperationOutput<RoomDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = roomDataManager.Update(room);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RoomDetailMapper(room);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public DeleteOperationOutput<RoomDetails> Delete(int Id)
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            DeleteOperationOutput<RoomDetails> deleteOperationOutput = new DeleteOperationOutput<RoomDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = roomDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }
        private Dictionary<int,Room> GetCachedRooms()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCacheRooms", () =>
                {
                    IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
                    List<Room> rooms = roomDataManager.GetRooms();
                    return rooms.ToDictionary(room => room.RoomId, room=>room);
                });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return roomDataManager.AreRoomsUpdated(ref _updateHandle);
            }
        }
        public RoomDetails RoomDetailMapper(Room room)
        {
            RoomDetails roomDetails = new RoomDetails();
            roomDetails.Entity = room;

            BuildingManager buildingManager = new BuildingManager();
            Building buildingInfo = buildingManager.GetBuildingById(room.BuildingId);
            roomDetails.BuildingName = buildingInfo.Name;

            return roomDetails;
        }
        public RoomInfo RoomInfoMapper(Room room)
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.RoomId = room.RoomId;
            roomInfo.Name = room.Name;
            return roomInfo;
        }
    }
}
