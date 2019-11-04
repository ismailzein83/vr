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
    public class ConnectionTypeManager
    {
        static Guid _definitionId = new Guid("A0AF0077-04F1-4C46-A829-B093084112F0");

        public ConnectionTypeEntity GetConnectionType(Guid connectionTypeId)
        {
            return GetCachedConnectionTypes().GetRecord(connectionTypeId);
        }

        public IEnumerable<ConnectionTypeInfo> GetConnectionTypeInfo(ConnectionTypeInfoFilter connectionTypeInfoFilter)
        {
            Func<ConnectionTypeEntity, bool> filterExpression = (connectionTypeEntity) =>
            {
                return true;
            };
            return GetCachedConnectionTypes().MapRecords(ConnectionTypeInfoMapper, filterExpression);
        }

        #region Private Methods

        private Dictionary<Guid, ConnectionTypeEntity> GetCachedConnectionTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedConnectionTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, ConnectionTypeEntity> result = new Dictionary<Guid, ConnectionTypeEntity>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        ConnectionTypeEntity portConnectionType = new ConnectionTypeEntity()
                        {
                            ConnectionTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            BusinessEntitityDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("BusinessEntityDefinition"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        result.Add(portConnectionType.ConnectionTypeId, portConnectionType);
                    }
                }
                return result;
            });
        }

        #endregion

        #region Mappers
        private ConnectionTypeInfo ConnectionTypeInfoMapper(ConnectionTypeEntity portConnectionType)
        {
            return new ConnectionTypeInfo()
            {
                ConnectionTypeId = portConnectionType.ConnectionTypeId,
                Name = portConnectionType.Name,
                BusinessEntitityDefinitionId = portConnectionType.BusinessEntitityDefinitionId
            };
        }
        #endregion
    }
}
