using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace TestCallAnalysis.Business
{
    public class CaseCDRChangeStatusAction : GenericBEActionSettings
    {
        public override string ActionKind { get { return ""; } }

        public override Guid ConfigId { get { return new Guid("7ED6B43C-0470-4293-BD99-E6C7D1DA355D"); } }
    }
}
