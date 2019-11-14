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
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

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
                        FieldName ="Node",
                        Values =  new List<object> {input.NodeId}
                    },
                    new ObjectListRecordFilter{
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId.ToString() }
                    }, new ObjectListRecordFilter{
                              FieldName = "Type",
                              Values = new List<object> { input.PortTypeId.ToString() }
                    }
                }
            };

            if (input.PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "PartType",
                    Values = new List<object> { input.PartTypeId.Value.ToString() }
                });
            }
            else
            {
                filter.Filters.Add(new EmptyRecordFilter
                {
                    FieldName = "PartType",
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(portType.BusinessEntitityDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var portid = (long)firstItem.FieldValues.GetRecord("ID");
            return ReservePort(portid, input.PortTypeId);
        }
        public SetPortUsedOutput SetPortUsed(SetPortUsedInput input)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.UsedPortStatusDefinitionId } },
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
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.FaultyPortStatusDefinitionId } },
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
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.FreePortStatusDefinitionId } },
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
                NodeId = (long)nodePort.FieldValues.GetRecord("Node"),
                NodeTypeId = (Guid)nodePort.FieldValues.GetRecord("NodeType"),
                PortId = (long)nodePort.FieldValues.GetRecord("ID"),
                PortNumber = nodePort.FieldValues.GetRecord("Number") as string,
                PortTypeId = (Guid)nodePort.FieldValues.GetRecord("Type"),
            };

            var node = new NodeManager().GetNode(nodePortInfo.NodeId);
            if(node != null)
            {
                nodePortInfo.NodeNumber = node.Number;
            }

            var nodePartId = (long?)nodePort.FieldValues.GetRecord("Part");
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
                            var partId = (long)part.FieldValues.GetRecord("ID");
                            var partTypeId = (Guid)part.FieldValues.GetRecord("NodePartType");
                            var parentPartId = (long?)part.FieldValues.GetRecord("ParentPart");
                            var number = part.FieldValues.GetRecord("Number") as string;

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
            return _genericBusinessEntityManager.GetGenericBusinessEntity(portId, StaticBEDefinitionIDs.NodePortBEDefinitionId);
        }
        internal ReservePortOutput ReservePort(long portId, Guid portTypeId)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.ReservedPortStatusDefinitionId } },
                GenericBusinessEntityId = portId,
                FilterGroup = new RecordFilterGroup
                {
                    Filters = new List<RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId }
                        }
                   }
                }
            });

            if (updatedEntity.Result == UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = portId,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord("Number").Description
            };
        }

        #endregion
        
        #region Private Methods
        #endregion
    }



   
}
