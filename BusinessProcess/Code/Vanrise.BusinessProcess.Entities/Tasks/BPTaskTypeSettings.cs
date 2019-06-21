using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseBPTaskTypeSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string Editor { get; set; }

        public virtual bool AutoOpenTask { get; set; }

        public virtual string SerializeTaskData(BPTaskData taskData)
        {
            return Serializer.Serialize(taskData);
        }

        public virtual BPTaskData DeserializeTaskData(string serializedTaskData)
        {
            if (serializedTaskData != null)
                return Serializer.Deserialize(serializedTaskData).CastWithValidate<BPTaskData>("deserializedTaskData");
            else
                return null;

        }

    }

    public class BPTaskTypeSettings : BaseBPTaskTypeSettings
    {
        public override Guid ConfigId => new Guid("7E158311-F841-4569-BF44-3F7F8B10CF90");
    }
}
