using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public abstract class GenericEntity
    {
        public int EntityId { get; set; }

        public virtual Type BaseType
        {
            get
            {
                return this.GetType();
            }
        }
        public int TypeId { get; set; }

        public virtual int? ParentId
        {
            get
            {
                return null;
            }
        }

        public virtual int? OwnerId
        {
            get
            {
                return null;
            }
        }

        public virtual int? LinkedToEntityId
        {
            get
            {
                return null;
            }
        }

        public GenericEntityStatus Status { get; set; }
    }
}
