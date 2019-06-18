using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericItemInformationView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("A4C1333D-886B-4C94-BEFF-E1A32ADD69BE"); } }
        public override string RuntimeDirective { get { return "vr-genericdata-genericbe-genericiteminfoview-runtime"; } }
        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
        public VRGenericEditorDefinitionSetting GenericEditorDefinitionSetting { get; set; }
    }
}
