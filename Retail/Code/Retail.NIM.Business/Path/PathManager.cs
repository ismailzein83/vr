using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class PathManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static string s_idFieldName = "ID";
        static string s_connectionIdFieldName = "Connection";
        static string s_pathIdFieldName = "Path";
        static string s_port1IdFieldName = "Port1";
        static string s_port1NodeIdFieldName = "Port1Node";
        static string s_port1NodeTypeIdFieldName = "Port1NodeType";

        static string s_port2IdFieldName = "Port2";
        static string s_port2NodeIdFieldName = "Port2Node";
        static string s_port2NodeTypeIdFieldName = "Port2NodeType";

        #region Public Methods
        public PathOutput CreatePath(PathInput pathInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Name", pathInput.Name }, { "Status", StaticBEDefinitionIDs.DraftPathStatusDefinitionId } }
            });

            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;
            return new PathOutput()
            {
                PathId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord("ID").Value
            };
        }

        public PathConnectionOutput AddConnectionToPath(PathConnectionInput pathConnectionInput)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathConnectionBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_pathIdFieldName, pathConnectionInput.PathId }, { s_connectionIdFieldName, pathConnectionInput.ConnectionId } }
            });


            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;

            return new PathConnectionOutput
            {
                PathConnectionId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord(s_idFieldName).Value
            };
        }
        public PathPortOutput AddPortToPath(PathPortInput input)
        {
            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathPortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Path", input.PathId }, { "Port", input.PortId } }
            });


            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;

            return new PathPortOutput
            {
                PathPortId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord("ID").Value
            };
        }
        public SetPathReadyOutput SetPathReady(SetPathReadyInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.PathBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.ReadyPathStatusDefinitionId } },
                GenericBusinessEntityId = input.PathId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.DraftPathStatusDefinitionId }
                        }
                   }
                }
            });

            return new SetPathReadyOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
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
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.PathConnectionBEDefinitionId, null, filter);
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
                PathId = (long)genericBusinessEntity.FieldValues.GetRecord(s_pathIdFieldName),
                Port1Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1IdFieldName),
                Port1NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeIdFieldName),
                Port2Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2IdFieldName),
                Port2NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeIdFieldName),
                Port1NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeTypeIdFieldName),
                Port2NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeTypeIdFieldName),
            };
        }
        #endregion
    }
   
}
