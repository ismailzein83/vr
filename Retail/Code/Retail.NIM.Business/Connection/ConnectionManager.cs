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
    public class ConnectionManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static string s_idFieldName = "ID";
        static string s_modelFieldName = "Model";
        static string s_areaFieldName = "Area";
        static string s_siteFieldName = "Site";
        static string s_port1FieldName = "Port1";
        static string s_port1StatusFieldName = "Port1Status";
        static string s_port1NodeFieldName = "Port1Node";
        static string s_port1PartFieldName = "Port1Part";
        static string s_port1PartTypeFieldName = "Port1PartType";
        static string s_port1NodeTypeFieldName = "Port1NodeType";
        static string s_port2FieldName = "Port2";
        static string s_port2StatusFieldName = "Port2Status";
        static string s_port2NodeFieldName = "Port2Node";
        static string s_port2PartFieldName = "Port2Part";
        static string s_port2PartTypeFieldName = "Port2PartType";
        static string s_port2NodeTypeFieldName = "Port2NodeType";


        #region Public Methods
        public ReserveConnectionOutput ReserveConnection(ReserveConnectionInput input)
        {

            var port1Type = new NodePortTypeManager().GetNodePortType(input.Port1TypeId);
            port1Type.ThrowIfNull("port1Type", input.Port1TypeId);


            var port2Type = new NodePortTypeManager().GetNodePortType(input.Port2TypeId);
            port2Type.ThrowIfNull("port2Type", input.Port2TypeId);



            NodePortManager _nodePortManager = new NodePortManager();

            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName =s_port1NodeFieldName,
                        Values =  new List<object> {input.Port1NodeId}
                    }
                   ,new ObjectListRecordFilter
                    {
                        FieldName =s_port2NodeFieldName,
                        Values =  new List<object> {input.Port2NodeId}
                    }
                   ,new ObjectListRecordFilter
                   {
                        FieldName = s_port1StatusFieldName,
                        Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                   ,new ObjectListRecordFilter
                   {
                       FieldName = s_port2StatusFieldName,
                       Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                }
            };

            if (input.Port1PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = s_port1PartTypeFieldName,
                    Values = new List<object> { input.Port1PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = s_port1PartTypeFieldName,
                });
            }

            if (input.Port2PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = s_port2PartTypeFieldName,
                    Values = new List<object> { input.Port2PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = s_port2PartTypeFieldName,
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var port1Id = (long)firstItem.FieldValues.GetRecord(s_port1FieldName);

            var port2Id = (long)firstItem.FieldValues.GetRecord(s_port2FieldName);

            var reservedPort1 = _nodePortManager.ReservePort(port1Id, input.Port1TypeId);

            var reservedPort2 = _nodePortManager.ReservePort(port2Id, input.Port2TypeId);

            return new ReserveConnectionOutput
            {
                Port1 = reservedPort1,
                Port2 = reservedPort2
            };
        }

        public ConnectionOutput AddConnection(ConnectionInput connectionInput)
        {
            var connectionType = new ConnectionTypeManager().GetConnectionType(connectionInput.ConnectionType);
            connectionType.ThrowIfNull("connectionType", connectionInput.ConnectionType);

            var insertedEntity = _genericBusinessEntityManager.AddGenericBusinessEntity(new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = connectionType.BusinessEntitityDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_modelFieldName, connectionInput.Model }, { s_port1FieldName, connectionInput.Port1 }, { s_port2FieldName, connectionInput.Port2 } }
            });

            if (insertedEntity.Result == InsertOperationResult.Failed)
                return null;


            return new ConnectionOutput
            {
                ConnectionId = (long)insertedEntity.InsertedObject.FieldValues.GetRecord(s_idFieldName).Value
            };
        }

        public void RemoveConnection(RemoveConnectionInput input)
        {
            var connectionType = new ConnectionTypeManager().GetConnectionType(input.ConnectionTypeId);
            connectionType.ThrowIfNull("connectionType", input.ConnectionTypeId);

            _genericBusinessEntityManager.DeleteGenericBusinessEntity(new DeleteGenericBusinessEntityInput
            {
                BusinessEntityDefinitionId = connectionType.BusinessEntitityDefinitionId,
                GenericBusinessEntityIds = new List<object>() { input.ConnectionId },

            });
        }
        #endregion

        #region Internal Methods
        internal Connection GetConnection(long nodeId, List<long> notIncludeNodes)
        {
            List<Object> excludedNodes = new List<object>();
            if (notIncludeNodes != null)
            {
                foreach (var notIncludeNode in notIncludeNodes)
                    excludedNodes.Add(notIncludeNode);
            }

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
                                 FieldName = "Port1Node",
                                 CompareOperator = ListRecordFilterOperator.In,
                                 Values =new List<object>{ nodeId }
                             },new ObjectListRecordFilter
                             {
                                FieldName = "Port2Node",
                               CompareOperator = ListRecordFilterOperator.In,
                               Values =new List<object>{ nodeId }
                             }
                        }
                    }
                },

            };
            if (excludedNodes.Count > 0)
            {
                filter.Filters.Add(new RecordFilterGroup
                {
                    LogicalOperator = RecordQueryLogicalOperator.Or,
                    Filters = new List<RecordFilter>
                        {
                             new ObjectListRecordFilter
                             {
                                 FieldName = s_port1NodeFieldName,
                                 CompareOperator = ListRecordFilterOperator.NotIn,
                                 Values = excludedNodes
                             },new ObjectListRecordFilter
                             {
                                FieldName = s_port2NodeFieldName,
                               CompareOperator = ListRecordFilterOperator.NotIn,
                               Values = excludedNodes
                             }
                        }
                });
            }
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return ConnectionMapper(items.First());

        }
        internal bool CheckConnectionWithFreePort(long nodeId, List<long> excludedNodes)
        {
            List<Object> notIncludeNodes = new List<object>();
            if (excludedNodes != null)
            {
                foreach (var excludedNode in excludedNodes)
                    notIncludeNodes.Add(excludedNode);
            }

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
                                 FieldName = s_port1NodeFieldName,
                                 CompareOperator = ListRecordFilterOperator.In,
                                 Values =new List<object>{ nodeId }
                             },new ObjectListRecordFilter
                             {
                                FieldName = s_port2NodeFieldName,
                               CompareOperator = ListRecordFilterOperator.In,
                               Values =new List<object>{ nodeId }
                             }
                        }
                    },new ObjectListRecordFilter
                   {
                        FieldName = s_port1StatusFieldName,
                        Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                   ,new ObjectListRecordFilter
                   {
                       FieldName = s_port2StatusFieldName,
                       Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }

                },

            };
            if (notIncludeNodes != null && notIncludeNodes.Count > 0)
                filter.Filters.Add(new RecordFilterGroup
                {
                    LogicalOperator = RecordQueryLogicalOperator.Or,
                    Filters = new List<RecordFilter>
                        {
                             new ObjectListRecordFilter
                             {
                                 FieldName = s_port1NodeFieldName,
                                 CompareOperator = ListRecordFilterOperator.NotIn,
                                 Values = notIncludeNodes
                             },new ObjectListRecordFilter
                             {
                                FieldName = s_port2NodeFieldName,
                               CompareOperator = ListRecordFilterOperator.NotIn,
                               Values = notIncludeNodes
                             }
                        }
                });
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);
            return items != null && items.Count > 0;
        }

        #endregion

        #region Mapper

        Connection ConnectionMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new Connection
            {
                ConnectionId = (long)genericBusinessEntity.FieldValues.GetRecord(s_idFieldName),
                ModelId = (int)genericBusinessEntity.FieldValues.GetRecord(s_modelFieldName),
                AreaId = (long)genericBusinessEntity.FieldValues.GetRecord(s_areaFieldName),
                SiteId = (long)genericBusinessEntity.FieldValues.GetRecord(s_siteFieldName),
                Port1Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1FieldName),
                Port1StatusId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port1StatusFieldName),
                Port1NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeFieldName),
                Port1PartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_port1PartFieldName),
                Port1PartTypeId = (Guid?)genericBusinessEntity.FieldValues.GetRecord(s_port1PartTypeFieldName),
                Port1NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port1NodeTypeFieldName),
                Port2Id = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2FieldName),
                Port2StatusId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port2StatusFieldName),
                Port2NodeId = (long)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeFieldName),
                Port2PartId = (long?)genericBusinessEntity.FieldValues.GetRecord(s_port2PartFieldName),
                Port2PartTypeId = (Guid?)genericBusinessEntity.FieldValues.GetRecord(s_port2PartTypeFieldName),
                Port2NodeTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord(s_port2NodeTypeFieldName),
            };    
        }
        #endregion

    }
}                
                 
                 
                 
                 
                 
                 
                 
                 