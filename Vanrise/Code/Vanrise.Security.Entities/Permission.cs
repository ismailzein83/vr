using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Permission
    {
        public string PermissionPath { get; set; }

        public HolderType HolderType { get; set; }

        public string HolderName { get; set; }

        public string HolderId { get; set; }

        public EntityType EntityType { get; set; }

        public string EntityId { get; set; }

        public List<PermissionFlag> PermissionFlags { get; set; }
    }

    public class PermissionFlag
    {
        public string Name { get; set; }

        public Flag Value { get; set; }
    }

    public enum Flag
    {
        NONE,
        ALLOW,
        DENY
    }

    public enum HolderType
    {
        USER,
        GROUP
    }

}
