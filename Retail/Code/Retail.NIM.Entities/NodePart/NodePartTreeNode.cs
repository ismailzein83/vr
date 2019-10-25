using System.Collections.Generic;

namespace Retail.NIM.Entities
{
    public class NodePartTreeNode
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long? ParentPartId { get; set; }

        List<NodePartTreeNode> _childNodes = new List<NodePartTreeNode>();
        public List<NodePartTreeNode> ChildNodes { get { return _childNodes; } }
    }
}
