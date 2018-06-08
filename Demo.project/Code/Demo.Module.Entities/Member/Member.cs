using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities.Member
{
   public class Member
    {
        public long MemberId { get; set; }
        public string Name { get; set; }
        public long FamilyId { get; set; }
        public MemberSettings Settings { get; set; }

    }
   public class MemberSettings
   {
       public MemberShape MemberShape { get; set; }
   }
   public abstract class MemberShape
   {
       public abstract Guid ConfigId { get; }
       public abstract string GetMemberAreaDescription(IMemberShapeDescriptionContext context);
   }
   public interface IMemberShapeDescriptionContext
   {
       Member Member { get; }
   }

}
