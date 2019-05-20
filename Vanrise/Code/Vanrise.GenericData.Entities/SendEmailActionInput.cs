using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class SendEmailActionInput
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public object GenericBusinessEntityId { get; set; }
        public Guid GenericBEActionId { get; set; }
        public VRMailEvaluatedTemplate EmailTemplate { get; set; }
        public List<long> AttachementFileIds { get; set; }
    }
}
