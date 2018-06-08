using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
   public class Room
    {
        public long RoomId { get; set; }
        public string Name { get; set; }
        public long BuildingId { get; set; }
        public RoomSettings Settings { get; set; }

    }
   public class RoomSettings
   {
       public RoomShape RoomShape { get; set; }
   }
   public abstract class RoomShape
   {
       public abstract Guid ConfigId { get; }
       public abstract string GetRoomShapeDescription(IRoomShapeDescriptionContext context);
   }
   public interface IRoomShapeDescriptionContext
   {
       Room Room { get; }
   }

}
