using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBEDefinitionView : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("77EEF180-EED0-46A8-BBAB-F19BFDB43F60"); }
        }

        public override string RuntimeDirective
        {
            get { return ""; }
        }
        public override bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
        public Guid GenericBEDefinitionId { get; set; }

        public List<GenericBEDefinitionViewColumnMapping> Mappings { get; set; }

    }
    public class GenericBEDefinitionViewColumnMapping
    {
        public string ParentColumnName { get; set; }

        public string SubviewColumnName { get; set; }
    }
}
