using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class HistoryGenericBEDefinitionView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("77F7DCB8-E42F-4EC3-8F46-0D655FD519B0"); }
        }
        public override string RuntimeDirective 
        {
            get { return "vr-genericdata-genericbe-historygridview-runtime"; }
        }
    }
}
