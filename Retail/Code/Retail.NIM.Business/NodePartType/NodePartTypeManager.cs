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
    public class NodePartTypeManager
    {

        static Guid _definitionId = new Guid("FC3B7620-849A-4F18-88BD-DBE02FBBC72B");
        public IEnumerable<NodePartTypeInfo> GetNodePartTypeInfo(NodePartTypeInfoFilter nodePartTypeInfoFilter)
        {
            Func<NodePartType, bool> filterExpression = (nodePartTypeEntity) =>
            {
                return true;
            };
            return GetCachedNodePartTypes().MapRecords(NodePartTypeInfoMapper, filterExpression);
        }

        #region Private Methods

        private Dictionary<Guid, NodePartType> GetCachedNodePartTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNodePartTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, NodePartType> result = new Dictionary<Guid, NodePartType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        NodePartType nodePartType = new NodePartType()
                        {
                            NodePartTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            BusinessEntitityDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("BusinessEntityDefinition"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(nodePartType.NodePartTypeId, nodePartType);
                    }
                }
                return result;
            });
        }

        #endregion

        #region Mappers
        private NodePartTypeInfo NodePartTypeInfoMapper(NodePartType nodePartType)
        {
            return new NodePartTypeInfo()
            {
                NodePartTypeId = nodePartType.NodePartTypeId,
                Name = nodePartType.Name,
                BusinessEntitityDefinitionId = nodePartType.BusinessEntitityDefinitionId
            };
        }
        #endregion
    }
}
