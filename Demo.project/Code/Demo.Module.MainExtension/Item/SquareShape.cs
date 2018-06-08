using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Item
{
    public class SquareShape : ItemShape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("{5AFEAE65-E561-4B8B-8148-3C51185B4188}"); }
        }

        public override string GetItemAreaDescription(IItemShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Item.Name, this.Height * this.Width);
        }
    }
}
