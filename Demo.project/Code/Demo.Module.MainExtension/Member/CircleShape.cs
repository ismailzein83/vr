using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Member
{
   public class CircleShape : MemberShape
    {
        public int Radius { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("AF5FF492-BE84-4AC5-85CC-E13ECEF01932"); }
        }

        public override string GetMemberAreaDescription(IMemberShapeDescriptionContext context)
        {
            return string.Format("The area of {0} is: {1}", context.Member.Name, Math.PI * Math.Pow(this.Radius, 2));
        }
    }
}
