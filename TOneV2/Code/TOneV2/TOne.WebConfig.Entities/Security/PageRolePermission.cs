using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.WebConfig.Entities
{
    public class PageRolePermission : RolePermission
    {
        public override string PermissionName
        {
            get { throw new NotImplementedException(); }
        }

        public int PageId { get; set; }

        public override int? LinkedToEntityId
        {
            get
            {
                return this.PageId;
            }
        }
    }
}
