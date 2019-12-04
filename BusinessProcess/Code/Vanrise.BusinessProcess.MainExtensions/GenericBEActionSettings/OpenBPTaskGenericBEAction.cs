using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class OpenBPTaskGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("00D55F71-95F6-4C63-A823-F7F8E4CE1372"); }
        }
        public override string ActionTypeName { get { return "OpenBPTaskGenericBEAction"; } }

        public override string ActionKind { get { return "OpenBPTask"; } }

        public string TaskIdFieldName { get; set; }
    }
}
