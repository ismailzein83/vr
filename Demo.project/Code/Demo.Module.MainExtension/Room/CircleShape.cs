using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Room
{
    class CircleShape: RoomShape
    {
        public int Radius { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("A0EDCEF4-C75F-40AC-BA44-C2BC9D9A6BF0"); }
        }

        public override string GetRoomShapeDescription(IRoomShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Room.Name, Math.PI * Math.Pow(this.Radius, 2));
        }
    
    
    }
}
