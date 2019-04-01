using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseBPTaskTypeSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string Editor { get; set; }

        public virtual bool AutoOpenTask { get; set; }
    }

    public class BPTaskTypeSettings : BaseBPTaskTypeSettings
    {
        public override Guid ConfigId => new Guid("7E158311-F841-4569-BF44-3F7F8B10CF90");
    }
}
