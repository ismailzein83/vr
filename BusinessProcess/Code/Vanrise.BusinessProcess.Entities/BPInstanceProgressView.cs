using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceProgressView
    {
        public string Title { get; set; }

        public BPInstanceProgressViewSettings Settings { get; set; }
    }

    public abstract class BPInstanceProgressViewSettings
    {
        public abstract string Editor { get; }

        public abstract bool LoadOnDemand { get; }
    }
}
