using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.MainExtentions.Child
{
    public class CircleShape : ChildShape
    {
        public int Radius { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("C6D35690-7FEF-48C2-8AC0-2CED1EF11B24"); }
        }

        public override string GetChildAreaDescription(IChildShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Child.Name, Math.PI * Math.Pow(this.Radius, 2));
        }
    }
}
