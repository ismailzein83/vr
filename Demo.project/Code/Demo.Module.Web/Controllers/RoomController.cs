using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Room")]
    [JSONWithTypeAttribute]
    public class Demo_Module_RoomController : BaseAPIController
    {
        RoomManager roomManager = new RoomManager();
        [HttpPost]
        [Route("GetFilteredRooms")]
        public object GetFilteredRooms(DataRetrievalInput<RoomQuery> input)
        {
            return GetWebResponse(input, roomManager.GetFilteredRooms(input));
        }

        [HttpGet]
        [Route("GetRoomById")]
        public Room GetRoomById(long roomId)
        {
            return roomManager.GetRoomById(roomId);
        }

        [HttpPost]
        [Route("UpdateRoom")]
        public UpdateOperationOutput<RoomDetails> UpdateRoom(Room room)
        {
            return roomManager.UpdateRoom(room);
        }

        [HttpPost]
        [Route("AddRoom")]
        public InsertOperationOutput<RoomDetails> AddRoom(Room room)
        {
            return roomManager.AddRoom(room);
        }
        [HttpGet]
        [Route("GetRoomShapeConfigs")]
        public IEnumerable<RoomShapeConfig> GetRoomShapeConfigs()
        {
            return roomManager.GetRoomShapeConfigs();
        }
    }
}
