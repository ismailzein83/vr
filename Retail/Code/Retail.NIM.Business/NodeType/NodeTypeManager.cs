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
   public class NodeTypeManager
    {

        static Guid _definitionId = new Guid("EAB80DB1-59AC-4D1F-874A-290E94EC1837");
       
        #region Public Methods
        public IEnumerable<NodeTypeInfo> GetNodeTypeInfo(NodeTypeInfoFilter nodeTypeInfoFilter)
        {
            Func<NodeType, bool> filterExpression = (nodeTypeEntity) =>
            {
                return true;
            };
            return GetCachedNodeTypes().MapRecords(NodeTypeInfoMapper, filterExpression);
        }
        #endregion


        #region Private Methods

        private Dictionary<Guid, NodeType> GetCachedNodeTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNodeTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, NodeType> result = new Dictionary<Guid, NodeType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        NodeType nodeType = new NodeType()
                        {
                            NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            BusinessEntitityDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("BusinessEntityDefinition"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(nodeType.NodeTypeId, nodeType);
                    }
                }
                return result;
            });
        }

        #endregion

        #region Mappers
        private NodeTypeInfo NodeTypeInfoMapper(NodeType nodeType)
        {
            return new NodeTypeInfo()
            {
                NodeTypeId = nodeType.NodeTypeId,
                Name = nodeType.Name,
                BusinessEntitityDefinitionId = nodeType.BusinessEntitityDefinitionId
            };
        }
        #endregion
    }
}
