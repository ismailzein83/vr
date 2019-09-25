using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProvision.Entities
{
    public class ProvisionAction
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Guid GenerateCodeParameterRecordTypeId { get; set; }
        public Guid ProvisionParameterRecordTypeId { get; set; }
    }
}
