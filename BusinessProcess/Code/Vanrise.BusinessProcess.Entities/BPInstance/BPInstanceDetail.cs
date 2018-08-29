using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceDetail
    {
        public BPInstance Entity { get; set; }
        public string StatusDescription { get { if (this.Entity != null) return this.Entity.Status.ToString(); return null; } }

        public string DefinitionTitle { get; set; }

        public string UserName { get; set; }
    }
}
