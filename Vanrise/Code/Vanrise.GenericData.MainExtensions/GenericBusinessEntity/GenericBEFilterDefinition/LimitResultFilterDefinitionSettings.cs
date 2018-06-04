using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class LimitResultFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E2AE62B2-68D9-46C0-9071-58212741CD23"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericbe-filterruntime-limitresult"; }
        }

        public int DefaultLimitResult { get; set; }
    }
}
