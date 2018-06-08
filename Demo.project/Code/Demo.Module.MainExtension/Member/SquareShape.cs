using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Member
{
    public class SquareShape:MemberShape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("F4D86734-1B71-4563-9B81-17E73D17FE3E"); }
        }

        public override string GetMemberAreaDescription(IMemberShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Member.Name, this.Height * this.Width);
        }
    }
}
