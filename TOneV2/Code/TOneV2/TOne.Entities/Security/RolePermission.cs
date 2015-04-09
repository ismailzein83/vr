using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum PermissionType { Allow = 0, Deny = 1 }
    public abstract class RolePermission : GenericEntity
    {
        public int RoleId { get; set; }

        public PermissionType PermissionType { get; set; }

        public abstract string PermissionName { get; }

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
                return typeof(RolePermission);
            }
        }
    }
}
