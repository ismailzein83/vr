using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBEStatusHistorySubview : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("03F03D74-D44B-45BC-A2D3-79512D5D2C53"); }
        }
        public override string RuntimeDirective 
        {
            get { return "vr-genericdata-genericbe-statushistorysubview-runtime"; }
        }
        public string StatusMappingFiled { get; set; }
        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
}
