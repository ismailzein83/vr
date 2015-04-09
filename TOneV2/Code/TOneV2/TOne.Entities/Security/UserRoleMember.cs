using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public class UserRoleMember: RoleMember
    {

        public override string MemberName
        {
            get { throw new NotImplementedException(); }
        }

        public int UserId { get; set; }

        public override int? LinkedToEntityId
        {
            get
            {
                return this.UserId;
            }
        }
    }
}
