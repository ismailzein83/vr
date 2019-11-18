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
    public class NodePortTypeManager
    {
        static Guid _definitionId = new Guid("8F4AC01A-82F2-45E7-A792-EE06CC16AE31");

        #region Public Methods
        public NodePortType GetNodePortType(Guid nodePortTypeId)
        {
            return GetCachedNodePortTypes().GetRecord(nodePortTypeId);
        }
        public IEnumerable<NodePortTypeInfo> GetNodePortTypeInfo(NodePortTypeInfoFilter nodePortTypeInfoFilter)
        {
            Func<NodePortType, bool> filterExpression = (nodePortTypeEntity) =>
            {
                return true;
            };
            return GetCachedNodePortTypes().MapRecords(NodePortTypeInfoMapper, filterExpression);
        }
      
        #endregion

        #region Private Methods

        private Dictionary<Guid, NodePortType> GetCachedNodePortTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNodePortTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, NodePortType> result = new Dictionary<Guid, NodePortType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        NodePortType nodePortType = new NodePortType()
                        {
                            NodePortTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            BusinessEntitityDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("BusinessEntityDefinition"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(nodePortType.NodePortTypeId, nodePortType);
                    }
                }
                return result;
            });
        }

        #endregion

        #region Mappers
        private NodePortTypeInfo NodePortTypeInfoMapper(NodePortType nodePortType)
        {
            return new NodePortTypeInfo()
            {
                NodePortTypeId = nodePortType.NodePortTypeId,
                Name = nodePortType.Name,
                BusinessEntitityDefinitionId = nodePortType.BusinessEntitityDefinitionId
            };
        }
        #endregion
    }
}
