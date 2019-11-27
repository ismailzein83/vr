using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.NIM.Entities;

namespace Retail.NIM.Business
{
    public class NodePortManager
    {
        static Guid s_nodePortBEDefinitionId = new Guid("04868fe5-9944-4e2b-b4d2-de9c5f73e2f4");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static string s_portIdFieldName = "ID";
        static string s_nodeIdFieldName = "Node";
        static string s_statusIdFieldName = "Status";
        static string s_portTypeIdFieldName = "Type";
        static string s_numberFieldName = "Number";
        static string s_nodeTypeIdFieldName = "NodeType";
        static string s_partTypeIdFieldName = "PartType";
        static string s_partIdFieldName = "Part";

        #region Port Status

        public static Guid s_freePortStatusDefinitionId = new Guid("a11d2835-89ed-442c-9646-c1f9b23ff213");
        public static Guid s_reservedPortStatusDefinitionId = new Guid("c51bb41b-b31a-45ba-b12e-8f521b0323eb");
        public static Guid s_usedPortStatusDefinitionId = new Guid("a9e7f47a-d908-4fd1-b8a6-6a7ab6828a8f");
        public static Guid s_faultyPortStatusDefinitionId = new Guid("09e09543-7267-46c8-a235-bf2ec936d33e");
        #endregion

        #region Public Methods
        public ReservePortOutput ReservePort(ReservePortInput input)
        {

            var portType = new NodePortTypeManager().GetNodePortType(input.PortTypeId);
            portType.ThrowIfNull("portType", input.PortTypeId);


            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName =s_nodeIdFieldName,
                        Values =  new List<object> {input.NodeId}
                    },
                    new ObjectListRecordFilter{
                              FieldName = s_statusIdFieldName,
                              Values = new List<object> { s_freePortStatusDefinitionId.ToString() }
                    }, new ObjectListRecordFilter{
                              FieldName = s_portTypeIdFieldName,
                              Values = new List<object> { input.PortTypeId.ToString() }
                    }
                }
            };

            if (input.PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = s_partTypeIdFieldName,
                    Values = new List<object> { input.PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = s_partTypeIdFieldName,
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(portType.BusinessEntitityDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var portid = (long)firstItem.FieldValues.GetRecord(s_portIdFieldName);
            return ReservePort(portid, input.PortTypeId);
        }
        public SetPortUsedOutput SetPortUsed(SetPortUsedInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_nodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName,s_usedPortStatusDefinitionId } },
                GenericBusinessEntityId = input.PortId
            });

            return new SetPortUsedOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }
        public SetPortFaultyOutput SetPortFaulty(SetPortFaultyInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_nodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_faultyPortStatusDefinitionId } },
                GenericBusinessEntityId = input.PortId
            });

            return new SetPortFaultyOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }
        public SetPortFreeOutput SetPortFree(SetPortFreeInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_nodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_freePortStatusDefinitionId} },
                GenericBusinessEntityId = input.PortId
            });

            return new SetPortFreeOutput
            {
                IsSucceeded = (updatedEntity.Result == UpdateOperationResult.Succeeded)
            };
        }
        public NodePortInfo GetPortInfo(long portId)
        {
            var nodePort = GetPort(portId);
            if (nodePort == null)
                return null;

            var nodePortInfo = new NodePortInfo
            {
                NodeId = (long)nodePort.FieldValues.GetRecord(s_nodeIdFieldName),
                NodeTypeId = (Guid)nodePort.FieldValues.GetRecord(s_nodeTypeIdFieldName),
                PortId = (long)nodePort.FieldValues.GetRecord(s_portIdFieldName),
                PortNumber = nodePort.FieldValues.GetRecord(s_numberFieldName) as string,
                PortTypeId = (Guid)nodePort.FieldValues.GetRecord(s_portTypeIdFieldName),
            };

            var node = new NodeManager().GetNode(nodePortInfo.NodeId);
            if(node != null)
            {
                nodePortInfo.NodeNumber = node.Number;
            }

            var nodePartId = (long?)nodePort.FieldValues.GetRecord(s_partIdFieldName);
            if(nodePartId.HasValue)
            {

                nodePortInfo.NodeParts = new List<NodePortPartInfo>();

                var nodeParts = new NodePartManager().GetNodePartsByNodeId(nodePortInfo.NodeId);
                if(nodeParts != null && nodeParts.Count > 0)
                {

                    long? reachedPartId = nodePartId.Value;
                    while(reachedPartId != null)
                    {
                        foreach (var part in nodeParts)
                        {
                            var partId = part.NodePartId;
                            var partTypeId = part.NodePartTypeId;
                            var parentPartId = part.ParentPartId;
                            var number = part.Number;

                            if (reachedPartId == partId)
                            {
                                nodePortInfo.NodeParts.Add(new NodePortPartInfo
                                {
                                    NodePartId = partId,
                                    NodePartNumber = number,
                                    NodePartTypeId = partTypeId,
                                });
                                reachedPartId = parentPartId;
                                break;
                            }
                        }
                    }
                    nodePortInfo.NodeParts.Reverse();
                }
            }
            return nodePortInfo;

        }
        #endregion

        #region Internal Methods
        internal GenericBusinessEntity GetPort(long portId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(portId, s_nodePortBEDefinitionId);
        }
        internal ReservePortOutput ReservePort(long portId, Guid portTypeId)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_nodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { s_statusIdFieldName, s_reservedPortStatusDefinitionId} },
                GenericBusinessEntityId = portId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = s_statusIdFieldName,
                              Values = new List<object> { s_freePortStatusDefinitionId}
                        }
                   }
                }
            });

            if (updatedEntity.Result == UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = portId,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord(s_numberFieldName).Description
            };
        }

        internal bool CheckFreePortByNodeId(long nodeId)
        {
            var items = _genericBusinessEntityManager.GetAllGenericBusinessEntities(s_nodePortBEDefinitionId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = s_nodeIdFieldName,
                        Values= new List<object>{ nodeId }
                    },
                    new ObjectListRecordFilter
                    {
                        FieldName = s_statusIdFieldName,
                        Values= new List<object>{ s_freePortStatusDefinitionId.ToString() }
                    }
                }
            });
            if (items == null || items.Count == 0)
                return false;
            return true;
        }

        #endregion
        
        #region Private Methods
        #endregion
    }



   
}
