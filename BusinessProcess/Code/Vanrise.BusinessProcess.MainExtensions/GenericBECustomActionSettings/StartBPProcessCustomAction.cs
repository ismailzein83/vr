using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class StartBPProcessCustomAction : GenericBECustomActionSettings
    {
        public override Guid ConfigId { get { return new Guid("F13D539A-3688-427A-818B-7B4717BA1D26"); } }
        public override string ActionTypeName { get { return "StartBPProcessCustomAction"; } }
        public Guid BPDefinitionId { get; set; }
        public List<InputArgumentMapping> InputArgumentsMapping { get; set; }
    }
}
