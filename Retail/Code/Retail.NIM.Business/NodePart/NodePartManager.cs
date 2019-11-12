using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{

    public class NodePartManager
    {
        static Guid _definitionId = new Guid("ADFFB988-63C8-4C62-B084-4AE4FBEA3C3C");
        GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

        public NodePartTreeNode GetNodePartTree(long nodeId)
        {
            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);

            long nodePartNodeId;
            Dictionary<long, NodePartTreeNode> nodePartTrees = new Dictionary<long, NodePartTreeNode>();
            foreach (var genericBusinessEntity in genericBusinessEntities)
            {
                nodePartNodeId = (long)genericBusinessEntity.FieldValues.GetRecord("Node");
                if (nodePartNodeId.Equals(nodeId))
                {
                    NodePartTreeNode node = new NodePartTreeNode
                    {
                        Id = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                        Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                        ParentPartId = (long?)genericBusinessEntity.FieldValues.GetRecord("ParentPart"),
                    };
                    nodePartTrees.Add(node.Id, node);
                }
            }

            foreach (var node in nodePartTrees.Values)
            {

                if (node.ParentPartId.HasValue)
                {
                    NodePartTreeNode parentNode;
                    if (nodePartTrees.TryGetValue(node.ParentPartId.Value, out parentNode))
                    {
                        parentNode.ChildNodes.Add(node);
                    }
                }
            }

            NodePartTreeNode nodePartTreeNode = new NodePartTreeNode();
            nodePartTreeNode.Id = nodeId;
            foreach (var node in nodePartTrees.Values)
            {
                if (node.ParentPartId == null)
                {
                    nodePartTreeNode.ChildNodes.Add(node);
                }
            }

            return nodePartTreeNode;
        }


        internal List<GenericBusinessEntity> GetNodePartsByNodeId(long nodeId)
        {
            return  genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId,null,new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName ="Node",
                        Values = new List<object>{ nodeId }
                    }
                }
            });
        }
    }
}
