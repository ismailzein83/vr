using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{   

    public class BE360DegreeNodeType : Vanrise.Entities.VRComponentType<BE360DegreeNodeTypeSettings>
    {
    }

    public class BE360DegreeNodeType<T> where T :BE360DegreeNodeTypeExtendedSettings
    {
        public BE360DegreeNodeType NodeType { get; set; }

        public T ExtendedSettings { get; set; }
    }

    public class BE360DegreeNodeTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("E439D307-8F5E-4716-BE97-049F6DBB569A"); }
        }

        public BE360DegreeNodeTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class BE360DegreeNodeTypeExtendedSettings
    {
        public virtual string Editor { get; set; }

        public virtual List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            return null;
        }

        public virtual BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            return context.Node;
        }
    }

    public interface IBE360DegreeNodeGetChildNodesContext
    {
        BE360DegreeNode Node { get; }
    }

    public interface IBE360DegreeNodeRefreshNodeContext
    {
        BE360DegreeNode Node { get; }
    }
}
