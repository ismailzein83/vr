using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.MainExtentions.Child
{
    public class SquareShape : ChildShape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("714A2C1C-71B1-4F81-B19C-2D0D7DDCD6C3"); }
        }

        public override string GetChildAreaDescription(IChildShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Child.Name,this.Height * this.Width);
        }
    }
}
