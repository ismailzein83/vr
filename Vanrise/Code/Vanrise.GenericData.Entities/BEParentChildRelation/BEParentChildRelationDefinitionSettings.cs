using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BEParentChildRelationDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid ParentBEDefinitionId { get; set; }

        public Guid ChildBEDefinitionId { get; set; }
    }
}