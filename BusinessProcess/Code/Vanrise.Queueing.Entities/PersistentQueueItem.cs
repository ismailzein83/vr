using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public abstract class PersistentQueueItem
    {        
        public long ExecutionFlowTriggerItemId { get; set; }
        
        public virtual byte[] Serialize()
        {
            return Vanrise.Common.ProtoBufSerializer.Serialize(this);
        }

        public virtual T Deserialize<T>(byte[] serializedBytes) where T : PersistentQueueItem
        {
            return Vanrise.Common.ProtoBufSerializer.Deserialize<T>(serializedBytes);
        }

        public abstract string GenerateDescription();

        public virtual string ItemTypeTitle
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public virtual QueueSettings DefaultQueueSettings
        {
            get
            {
                return new QueueSettings();
            }
        }
    }
}
