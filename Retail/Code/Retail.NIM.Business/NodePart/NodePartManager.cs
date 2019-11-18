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
        static string s_partIdFieldName = "ID";
        static string s_numberFieldName = "Number";
        static string s_modelFieldName = "Model";
        static string s_nodeIdFieldName = "Node";
        static string s_parentPartIdFieldName = "ParentPart";
        static string s_createdByFieldName = "CreatedBy";
        static string s_createdTimeFieldName = "CreatedTime";
        static string s_lastModifiedByFieldName = "LastModifiedBy";
        static string s_lastModifiedTimeFieldName = "CreatedTime";
        static string s_nodePartTypeIdFieldName = "NodePartType";
      
        #region Public Methods
        public NodePartTreeNode GetNodePartTree(long nodeId)
        {
            var nodeParts = GetNodePartsByNodeId(nodeId);

            Dictionary<long, NodePartTreeNode> nodePartTrees = new Dictionary<long, NodePartTreeNode>();
            foreach (var nodePart in nodeParts)
            {
                NodePartTreeNode node = new NodePartTreeNode
                {
                    Id = (long)nodePart.NodePartId,
                    Number = nodePart.Number,
                    ParentPartId = nodePart.ParentPartId,
                };
                nodePartTrees.Add(node.Id, node);
            }



            NodePartTreeNode nodePartTreeNode = new NodePartTreeNode();
            nodePartTreeNode.Id = nodeId;

            foreach (var node in nodePartTrees.Values)
            {

                if (node.ParentPartId.HasValue)
                {
                    NodePartTreeNode parentNode = nodePartTrees.GetRecord(node.ParentPartId.Value);
                    parentNode.ChildNodes.Add(node);
                }
                else
                {
                    nodePartTreeNode.ChildNodes.Add(node);
                }
            }
            return nodePartTreeNode;
        }
        #endregion

        #region Internal Methods
        internal List<NodePart> GetNodePartsByNodeId(long nodeId)
        {
            var items =   genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId,null,new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName =s_nodeIdFieldName,
                        Values = new List<object>{ nodeId }
                    }
                }
            });
            if (items == null)
                return null;
            return items.MapRecords(NodePartMapper).ToList();
        }
        #endregion

        #region Mapper
        NodePart NodePartMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new NodePart
            {
                NodePartId = (long)genericBusinessEntity.FieldValues.GetRecord(s_partIdFieldName),
                Model = (int)genericBusinessEntity.FieldValues.GetRecord(s_modelFieldName),
                NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_nodeIdFieldName),
                Number = genericBusinessEntity.FieldValues.GetRecord(s_numberFieldName) as string,
                ParentPartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_parentPartIdFieldName),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord(s_createdByFieldName),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord(s_createdTimeFieldName),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord(s_lastModifiedByFieldName),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord(s_lastModifiedTimeFieldName),
                NodePartTypeId=(Guid)genericBusinessEntity.FieldValues.GetRecord(s_nodePartTypeIdFieldName),
            };
        }
        #endregion
    }
}
