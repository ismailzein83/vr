using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.NIM.Entities
{
    public class PathDiagramViewDefinition : GenericBEViewDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("5046F437-65A4-4306-BE74-32FED4E4E647"); } }

        public override string RuntimeDirective { get { return "retail-nim-pathdiagram-viewruntime"; } }

        public string ParentFieldName { get; set; }
    }
}
