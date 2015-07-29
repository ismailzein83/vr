//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Queueing.Entities;

//namespace Vanrise.Queueing
//{
//    public abstract class PersistentQueueItem
//    {
//        public virtual byte[] Serialize()
//        {
//            return Vanrise.Common.ProtoBufSerializer.Serialize(this);
//        }

//        public virtual T Deserialize<T>(byte[] serializedBytes) where T : PersistentQueueItem 
//        {
//            return Vanrise.Common.ProtoBufSerializer.Deserialize<T>(serializedBytes);
//        }

//        public abstract string GenerateDescription();

//        internal protected virtual string ItemTypeTitle
//        {
//            get
//            {
//                return this.GetType().Name;
//            }
//        }

//        internal protected virtual QueueSettings DefaultQueueSettings
//        {
//            get
//            {
//                return new QueueSettings();
//            }
//        }
//    }
//}
