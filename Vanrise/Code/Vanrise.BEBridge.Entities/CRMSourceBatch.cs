using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class CRMSourceBatch : SourceBEBatch
    {
        public override string BatchName
        {
            get { return this.EntityName; }
        }
        public string EntityName { get; set; }
        public List<dynamic> EntityList { get; set; }
    }
}
