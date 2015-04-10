using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.WebConfig.Entities
{
    public enum MenuItemLinkedEntityType { None = 0, Module = 1, Page = 2 }

    public class MenuItem : GenericEntity
    {
        public int MenuId { get; set; }

        public int? ParentMenuItemId { get; set; }

        public MenuItemLinkedEntityType LinkedEntityType { get; set; }

        public int? LinkedEntityId { get; set; }

        public override int? OwnerId
        {
            get
            {
                return this.MenuId;
            }
        }

        public override int? ParentId
        {
            get
            {
                return this.ParentMenuItemId;
            }
        }

        public override int? LinkedToEntityId
        {
            get
            {
                return this.LinkedEntityId;
            }
        }
    }
}
