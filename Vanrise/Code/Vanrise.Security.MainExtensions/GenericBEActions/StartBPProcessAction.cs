using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Security.MainExtensions.GenericBEActions
{
    public class StartBPProcessAction : GenericBEActionSettings
    {
        public override Guid ConfigId { get { return new Guid("0E9730F4-1197-456B-9261-E745FE4AFB3B"); } }
        public override string ActionTypeName { get { return "StartBPProcess"; } }
        public override string ActionKind { get { return "StartBPProcessAction"; } }
    }
}
