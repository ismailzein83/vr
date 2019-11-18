using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.NIM.Business
{
    public class PathConnectionManager
    {
        static Guid s_pathConnectionBEDefinitionId = new Guid("24364ed5-1795-468c-a27b-c00013e830ac");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        static string s_idFieldName = "ID";
        static string s_connectionIdFieldName = "Connection";
        static string s_connectionTypeIdFieldName = "ConnectionType";
        static string s_pathIdFieldName = "Path";
        static string s_port1IdFieldName = "Port1";
        static string s_port1NodeIdFieldName = "Port1Node";
        static string s_port1NodeTypeIdFieldName = "Port1NodeType";
        static string s_port1NodePartIdFieldName = "Port1NodePart";
        static string s_port1NodePartTypeIdFieldName = "Port1NodePartType";
        static string s_port2IdFieldName = "Port2";
        static string s_port2NodeIdFieldName = "Port2Node";
        static string s_port2NodeTypeIdFieldName = "Port2NodeType";
        static string s_port2NodePartIdFieldName = "Port2NodePart";
        static string s_port2NodePartTypeIdFieldName = "Port2NodePartType";

        #region Public Methods
        public PathConnectionOutput AddConnectionToPath(PathConnectionInput pathConnectionInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_pathConnectionBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_pathIdFieldName, pathConnectionInput.PathId }, { s_connectionIdFieldName, pathConnectionInput.ConnectionId } }
            });


            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;

            return new PathConnectionOutput
            {
                PathConnectionId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord(s_idFieldName).Value
            };
        }
        public void RemovePathConnection(RemovePathConnectionInput input)
        {
            _genericBusinessEntityManager.DeleteGenericBusinessEntity(new DeleteGenericBusinessEntityInput
            {
                BusinessEntityDefinitionId = s_pathConnectionBEDefinitionId,
                GenericBusinessEntityIds = new List<object>() { input.PathConnectionId },

            });
        }
        public List<PathConnection> GetPathConnections(long pathId)
        {
            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new RecordFilterGroup
                    {
                        LogicalOperator = RecordQueryLogicalOperator.Or,
                        Filters = new List<RecordFilter>
                        {
                             new ObjectListRecordFilter
                             {
                                 FieldName = s_pathIdFieldName,
                                 CompareOperator = ListRecordFilterOperator.In,
                                 Values =new List<object>{ pathId }
                             }
                        }
                    }
                },

            };
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(s_pathConnectionBEDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return items.MapRecords(PathConnectionMapper).ToList();
        }
        #endregion
      
        #region Mapper
        PathConnection PathConnectionMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new PathConnection
            {
                PathConnectionId = (long)genericBusinessEntity.FieldValues.GetRecord(s_idFieldName),
                ConnectionId = (long)genericBusinessEntity.FieldValues.GetRecord(s_connectionIdFieldName),
                ConnectionTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_connectionTypeIdFieldName),
                PathId = (long)genericBusinessEntity.FieldValues.GetRecord(s_pathIdFieldName),
                Port1Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1IdFieldName),
                Port1NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeIdFieldName),
                Port2Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2IdFieldName),
                Port2NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeIdFieldName),
                Port1NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeTypeIdFieldName),
                Port2NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeTypeIdFieldName),
                Port1NodePartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_port1NodePartIdFieldName),
                Port1NodePartTypeId = (Guid?)genericBusinessEntity.FieldValues.GetRecord(s_port1NodePartTypeIdFieldName),
                Port2NodePartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_port2NodePartIdFieldName),
                Port2NodePartTypeId = (Guid?)genericBusinessEntity.FieldValues.GetRecord(s_port2NodePartTypeIdFieldName),
            };
        }
        #endregion
    }
}
