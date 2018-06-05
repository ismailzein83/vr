using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Demo.Module.Business
{
    public class RoomManager
    {

        #region Public Methods
        public IDataRetrievalResult<RoomDetails> GetFilteredRooms(DataRetrievalInput<RoomQuery> input)
        {
            var allRooms = GetCachedRooms();
            Func<Room, bool> filterExpression = (room) =>
            {
                if (input.Query.Name != null && !room.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allRooms.ToBigResult(input, filterExpression, RoomDetailMapper));

        }


        public InsertOperationOutput<RoomDetails> AddRoom(Room room)
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            InsertOperationOutput<RoomDetails> insertOperationOutput = new InsertOperationOutput<RoomDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long roomId = -1;

            bool insertActionSuccess = roomDataManager.Insert(room, out roomId);
            if (insertActionSuccess)
            {
                room.RoomId = roomId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RoomDetailMapper(room);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Room GetRoomById(long roomId)
        {
            var allRooms = GetCachedRooms();
            return allRooms.GetRecord(roomId);
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RoomDetailMapper(room);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return roomDataManager.AreCompaniesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Room> GetCachedRooms()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRooms", () =>
               {
                   IRoomDataManager roomDataManager = DemoModuleFactory.GetDataManager<IRoomDataManager>();
                   List<Room> rooms = roomDataManager.GetRooms();
                   return rooms.ToDictionary(room => room.RoomId, room => room);
               });
        }
        #endregion

        #region Mappers
        public RoomDetails RoomDetailMapper(Room room)
        {
            return new RoomDetails
            {
                Name = room.Name,
                RoomId = room.RoomId
            };
        }
        #endregion

    }

    
}
