using Demo.Module.Business;
using Demo.Module.Entities.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Room")]
    public class Demo_Module_RoomController : BaseAPIController
    {
        RoomManager roomManager = new RoomManager();
        [HttpPost]
        [Route("GetFilteredRooms")]
        public object GetFilteredRooms(DataRetrievalInput<RoomQuery> input)
        {
            RoomManager roomManager = new RoomManager();
            return GetWebResponse(input, roomManager.GetFilteredRooms(input));
        }

        [HttpGet]
        [Route("GetRoomById")]
        public Room GetRoomById(int roomId)
        {
            return roomManager.GetRoomById(roomId);
        }

        [HttpGet]
        [Route("GetRoomsInfo")]
        public IEnumerable<RoomInfo> GetRoomsInfo(string filter = null)
        {
            RoomManager roomManager = new RoomManager();
            RoomInfoFilter roomFilter = (filter != null ) ? Vanrise.Common.Serializer.Deserialize<RoomInfoFilter>(filter) : null;
            return roomManager.GetRoomsInfo(roomFilter);
        }

        [HttpPost]
        [Route("AddRoom")]
        public InsertOperationOutput<RoomDetails> AddRoom(Room room)
        {
            return roomManager.AddRoom(room);
        }

        [HttpPost]
        [Route("UpdateRoom")]
        public UpdateOperationOutput<RoomDetails> UpdateRoom(Room room)
        {
            return roomManager.UpdateRoom(room);
        }

        [HttpGet]
        [Route("DeleteRoom")]
        public DeleteOperationOutput<RoomDetails> DeleteRoom(int roomId)
        {
            return roomManager.Delete(roomId);
        }
    }
}