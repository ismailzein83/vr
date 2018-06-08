using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Room
{
    class RectangleShape : RoomShape
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("20357C6C-D2AA-4C4D-94E3-0E9E003A4F48"); }
        }

        public override string GetRoomShapeDescription(IRoomShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Room.Name, this.Length * this.Width);
        }
    
    }
}
