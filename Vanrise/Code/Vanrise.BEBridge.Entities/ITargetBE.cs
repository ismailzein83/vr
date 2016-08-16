using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class TargetBEBatch
    {
        public IEnumerable<ITargetBE> TargetBEs { get; set; }
    }

    public interface ITargetBE
    {
        object TargetBEId { get; }

        object SourceBEId { get; }
    }
}
