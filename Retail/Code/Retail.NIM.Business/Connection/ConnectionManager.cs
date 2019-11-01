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
    public class ConnectionManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public GenericBusinessEntity GetConnection(long nodeId, List<long> notIncludeNodes)
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
                                 FieldName = "Port1Node",
                                 CompareOperator = ListRecordFilterOperator.NotIn,
                                 Values = excludedNodes
                             },new ObjectListRecordFilter
                             {
                                FieldName = "Port2Node",
                               CompareOperator = ListRecordFilterOperator.NotIn,
                               Values = excludedNodes
                             }
                        }
                });
            }
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);
            if (items == null || items.Count == 0)
                return null;
            return items.First();

        }

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
                        FieldName ="Port1Node",
                        Values =  new List<object> {input.Port1NodeId}
                    }
                   ,new ObjectListRecordFilter
                    {
                        FieldName ="Port2Node",
                        Values =  new List<object> {input.Port2NodeId}
                    }
                   ,new ObjectListRecordFilter
                   {
                        FieldName = "Port1Status",
                        Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                   ,new ObjectListRecordFilter
                   {
                       FieldName = "Port2Status",
                       Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                }
            };

            if (input.Port1PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Port1PartType",
                    Values = new List<object> { input.Port1PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = "Port1PartType",
                });
            }

            if (input.Port2PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Port2PartType",
                    Values = new List<object> { input.Port2PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = "Port2PartType",
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var port1Id = (long)firstItem.FieldValues.GetRecord("Port1");

            var port2Id = (long)firstItem.FieldValues.GetRecord("Port2");

            var reservedPort1 =_nodePortManager.ReservePort(port1Id, input.Port1TypeId);

            var reservedPort2 = _nodePortManager.ReservePort(port2Id, input.Port2TypeId);

            return new ReserveConnectionOutput
            {
                Port1 = reservedPort1,
                Port2 = reservedPort2
            };
        }

        public bool CheckConnectionWithFreePort(long nodeId, List<long> excludedNodes)
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
                    },new ObjectListRecordFilter
                   {
                        FieldName = "Port1Status",
                        Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                   }
                   ,new ObjectListRecordFilter
                   {
                       FieldName = "Port2Status",
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
                                 FieldName = "Port1Node",
                                 CompareOperator = ListRecordFilterOperator.NotIn,
                                 Values = notIncludeNodes
                             },new ObjectListRecordFilter
                             {
                                FieldName = "Port2Node",
                               CompareOperator = ListRecordFilterOperator.NotIn,
                               Values = notIncludeNodes
                             }
                        }
                });
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);
            return items != null && items.Count > 0;
        }


        //public void AddConnection()
        //{

        //}

        //public void AddConnectionToPath()
        //{

        //}
        //public bool SetPathReady()
        //{

        //}
    }
}
