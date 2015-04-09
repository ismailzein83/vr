using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public abstract class RoleMember : GenericEntity
    {
        public int RoleId { get; set; }

        public abstract string MemberName { get; }

        public override int? OwnerId
        {
            get
            {
                return this.RoleId;
            }
        }

        public override Type BaseType
        {
            get
            {
                return typeof(RoleMember);
            }
        }
    }
}
