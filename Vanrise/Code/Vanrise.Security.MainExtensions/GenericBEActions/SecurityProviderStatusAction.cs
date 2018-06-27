using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Security.MainExtensions.GenericBEActions
{
    public class SecurityProviderStatusAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9A1E4FB5-D732-4A8A-8827-7C06719C2C24"); }
        }
        public bool SetEnable { get; set; }
        public override string ActionTypeName { get { return "SecurityProviderStatus"; } }

    }
}
