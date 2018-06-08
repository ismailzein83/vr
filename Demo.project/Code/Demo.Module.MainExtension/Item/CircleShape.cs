using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Item
{
    public class CircleShape : ItemShape
    {
        public int Radius { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("{C7FA4919-9FA2-41E9-B895-343A86CF9B13}"); }
        }

        public override string GetItemAreaDescription(IItemShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Item.Name, Math.PI * Math.Pow(this.Radius, 2));
        }
    }
}
