using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.WebConfig.Entities
{
    public class ModuleRolePermission : RolePermission
    {
        public override string PermissionName
        {
            get { throw new NotImplementedException(); }
        }

        public int ModuleId { get; set; }

        public override int? LinkedToEntityId
        {
            get
            {
                return this.ModuleId;
            }
        }
    }
}
