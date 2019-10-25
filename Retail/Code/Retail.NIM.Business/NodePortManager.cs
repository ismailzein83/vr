using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.NIM.Entities;

namespace Retail.NIM.Business
{
    public class NodePortManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public ReserveConnectionOutput ReserveConnection(ReserveConnectionInput input)
        {
            var filter = new Vanrise.GenericData.Entities.RecordFilterGroup
            {
                Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
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
                    Values = new List<object> { input.Port1PartTypeId.Value }
                });
            }

            if (input.Port2PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Port2PartType",
                    Values = new List<object> { input.Port2PartTypeId.Value }
                });
            }

            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.ConnectionBEDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var port1id = (long)firstItem.FieldValues.GetRecord("Port1");

            var port2id = (long)firstItem.FieldValues.GetRecord("Port2");

            var reservedPort1 = ReservePort(port1id);

            var reservedPort2 = ReservePort(port2id);

            return new ReserveConnectionOutput
            {
                Port1 = reservedPort1,
                Port2 = reservedPort2
            };
        }

        public ReservePortOutput ReservePort(ReservePortInput input)
        {

            var filter = new Vanrise.GenericData.Entities.RecordFilterGroup
            {
                Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName ="Node",
                        Values =  new List<object> {input.NodeId}
                    }
                }
            };

            if (input.PartTypeId.HasValue)
            {
                filter.Filters.Add(new ObjectListRecordFilter
                {
                    FieldName = "Part",
                    Values = new List<object> { input.PartTypeId.Value }
                });
            }
            var entities = _genericBusinessEntityManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.NodePortBEDefinitionId, null, filter);

            if (entities == null || entities.Count() == 0)
                return null;

            var firstItem = entities.First();

            var portid = (long)firstItem.FieldValues.GetRecord("ID");
            return ReservePort(portid);
        }
        public ReservePortOutput ReservePort(long portId)
        {
            var updatedEntity = _genericBusinessEntityManager.UpdateGenericBusinessEntity(new GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = StaticBEDefinitionIDs.NodePortBEDefinitionId,
                FieldValues = new Dictionary<string, object> { { "Status", StaticBEDefinitionIDs.ReservedPortStatusDefinitionId } },
                GenericBusinessEntityId = portId,
                FilterGroup =new RecordFilterGroup
                {
                    Filters = new List<Vanrise.GenericData.Entities.RecordFilter>
                    {
                        new ObjectListRecordFilter
                        {
                              FieldName = "Status",
                              Values = new List<object> { StaticBEDefinitionIDs.FreePortStatusDefinitionId }
                        }
                   }
                }
            });

            if (updatedEntity.Result == Vanrise.Entities.UpdateOperationResult.Failed)
                return null;

            return new ReservePortOutput
            {
                PortId = portId,
                Number = updatedEntity.UpdatedObject.FieldValues.GetRecord("Number").Description
            };
        }





        public GenericBusinessEntity GetConnection(long nodeId, List<long> notIncludeNodes)
        {


            List<Object> excludedNodes = new List<object>();
            if(notIncludeNodes  != null)
            {
                foreach(var notIncludeNode in notIncludeNodes)
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
            if(excludedNodes.Count > 0)
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
        public bool CheckCoonectionWithFreePort(long nodeId, List<long> excludedNodes)
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
            if(notIncludeNodes != null && notIncludeNodes.Count >0)
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

        public GenericBusinessEntity GetNode(long nodeId, Guid businessEntityDefinitionId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(nodeId, businessEntityDefinitionId);
        }
        public GenericBusinessEntity GetNode(long nodeId)
        {
            return _genericBusinessEntityManager.GetGenericBusinessEntity(nodeId, StaticBEDefinitionIDs.NodeBEDefinitionId);
        }
        public GenericBusinessEntity GetNodeByNumber(string number, Guid businessEntityDefinitionId)
        {
            var dpItems = _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                        {
                            new StringRecordFilter
                            {
                                FieldName ="Number",
                                Value = number ,
                                CompareOperator = StringRecordFilterOperator.Equals
                            }
                        }
            });

            if (dpItems == null || dpItems.Count == 0)
                return null;
            return dpItems.First();
        }
        //public GenericBusinessEntity GetSwitchBySiteId(long siteId)
        //{

        //}
        //public GenericBusinessEntity GetDslamBySiteId(long siteId)
        //{

        //}
        public GenericBusinessEntity GetNodeByAddress(Guid businessEntityDefinitionId, long areadId, long siteId, int regionId,int cityId, int townId, long streetId)
        {
            var filter = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = "Area",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ areadId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Site",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ siteId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Region",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ regionId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "City",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ cityId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Town",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ townId }
                    },new ObjectListRecordFilter
                    {
                        FieldName = "Street",
                        CompareOperator = ListRecordFilterOperator.In,
                        Values =new List<object>{ streetId }
                    }
                }
            };
            var items =  _genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId,null, filter);
            if (items == null || items.Count == 0)
                return null;
            return items.First();

        }


        public GetTechnicalAddressOutputTechnologyItem GetDPTechnicalAddress(GenericBusinessEntity node)
        {
            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.CopperTechnology,
            };
            if (node == null)
                return item;


            bool targetReach = false;
            var nodeId = (long)node.FieldValues.GetRecord("ID");
            item.NetworkElements = new List<GetTechnicalAddressOutputTechnologyItemNetworkElement>();

            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
            {
                ID = nodeId,
                Number = (string)node.FieldValues.GetRecord("Number"),
                Type = (Guid)node.FieldValues.GetRecord("NodeType")
            });

            List<long> usedNodeIds = new List<long>();
            var subscriptionFeasible = true;
            while (!targetReach)
            {
                var connection = GetConnection(nodeId, usedNodeIds);
                if (connection == null)
                    return item;
                if (!CheckCoonectionWithFreePort(nodeId, usedNodeIds))
                {
                    subscriptionFeasible = false;
                }
                long port1NodeId = (long)connection.FieldValues.GetRecord("Port1Node");
                long port2NodeId = (long)connection.FieldValues.GetRecord("Port2Node");
                usedNodeIds.Add(nodeId);
                if (port1NodeId != nodeId)
                {
                    nodeId = port1NodeId;
                }
                if (port2NodeId != nodeId)
                {
                    nodeId = port2NodeId;
                }
                var nodeItem = GetNode(nodeId);
                if (nodeItem == null)
                    return item;
                var nodeTypeId = (Guid)nodeItem.FieldValues.GetRecord("NodeType");
                item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                {
                    ID = nodeId,
                    Number = (string)nodeItem.FieldValues.GetRecord("Number"),
                    Type = nodeTypeId
                });
                if (nodeTypeId == StaticBEDefinitionIDs.MDFTypeId)
                {
                    targetReach = true;
                    item.TechnologyAvailable = true;
                }
            }

            item.SubscriptionFeasible = subscriptionFeasible;
            return item;
        }
        public GetTechnicalAddressOutputTechnologyItem GetFDBTechnicalAddress(GenericBusinessEntity node)
        {
            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.FiberTechnology,
            };
            if (node == null)
                return item;


            bool targetReach = false;
            var nodeId = (long)node.FieldValues.GetRecord("ID");
            item.NetworkElements = new List<GetTechnicalAddressOutputTechnologyItemNetworkElement>();
            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
            {
                ID = nodeId,
                Number = (string)node.FieldValues.GetRecord("Number"),
                Type = (Guid)node.FieldValues.GetRecord("NodeType")
            });
            item.SubscriptionFeasible = CheckCoonectionWithFreePort(nodeId, null);
            List<long> usedNodeIds = new List<long>();
            while(!targetReach)
            {
                var connection = GetConnection(nodeId, usedNodeIds);
                if (connection == null)
                    return item;
                long port1NodeId = (long)connection.FieldValues.GetRecord("Port1Node");
                long port2NodeId = (long)connection.FieldValues.GetRecord("Port2Node");
                usedNodeIds.Add(nodeId);
                if (port1NodeId != nodeId)
                {
                    nodeId = port1NodeId;
                }
                else if (port2NodeId != nodeId)
                {
                    nodeId = port2NodeId;
                }
                var nodeItem = GetNode(nodeId);
                if (nodeItem == null)
                    return item;
                var nodeTypeId = (Guid)nodeItem.FieldValues.GetRecord("NodeType");
                item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                {
                    ID = nodeId,
                    Number = (string)nodeItem.FieldValues.GetRecord("Number"),
                    Type = nodeTypeId
                });
                if(nodeTypeId == StaticBEDefinitionIDs.OLTNodeTypeId)
                {
                    targetReach = true;
                    item.TechnologyAvailable = true;
                }
            }

            if(item.SubscriptionFeasible)
            {
                item.DataFeasible = true;
                item.TelephonyFeasible = true;
            }
            return item;
        }
        public enum NumberType { NearbyNumber = 1, FDBNumber = 2, DPNumber = 3 }

        public GetTechnicalAddressOutput GetTechnicalAddress(NumberType numberType, string number)
        {

         
            var output = new GetTechnicalAddressOutput
            {
                TechnologyItems = new List<GetTechnicalAddressOutputTechnologyItem>()
            };
            Guid businessEntityDefinitionId = Guid.Empty;
            switch (numberType)
            {
                case NumberType.DPNumber:
                   businessEntityDefinitionId = StaticBEDefinitionIDs.DPBEDefinitionId;
                    break;
                case NumberType.FDBNumber:
                   businessEntityDefinitionId = StaticBEDefinitionIDs.FDBBEDefinitionId;
                    break;
            }
            var nodeNeeded = GetNodeByNumber(number, businessEntityDefinitionId);
            if(nodeNeeded == null)
            {
                output.TechnologyItems.Add(GetDPTechnicalAddress(null));
                output.TechnologyItems.Add(GetFDBTechnicalAddress(null));
                return output;

            }
            output.AreaId = (long)nodeNeeded.FieldValues.GetRecord("Area");
            output.SiteId = (long)nodeNeeded.FieldValues.GetRecord("Site");
            output.RegionId = (int)nodeNeeded.FieldValues.GetRecord("Region");
            output.CityId = (int)nodeNeeded.FieldValues.GetRecord("City");
            output.TownId = (int)nodeNeeded.FieldValues.GetRecord("Town");
            output.StreetId = (long)nodeNeeded.FieldValues.GetRecord("Street");
            output.BuildingDetails = (string)nodeNeeded.FieldValues.GetRecord("BuildingDetails");


            switch (numberType)
            {
                case NumberType.DPNumber:
                    output.TechnologyItems.Add(GetDPTechnicalAddress(nodeNeeded));
                    var fdbNode = GetNodeByAddress(StaticBEDefinitionIDs.FDBBEDefinitionId, output.AreaId, output.SiteId,output.RegionId, output.CityId, output.TownId, output.StreetId);
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(fdbNode));
                    break;
                case NumberType.FDBNumber:
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(nodeNeeded));
                    var dpNode = GetNodeByAddress(StaticBEDefinitionIDs.DPBEDefinitionId, output.AreaId, output.SiteId, output.RegionId, output.CityId, output.TownId, output.StreetId);
                    output.TechnologyItems.Add(GetDPTechnicalAddress(dpNode));
                    break;
            }
            return output;
        }
    }

    public class GetTechnicalAddressOutput
    {
        public long AreaId { get; set; }

        public long SiteId { get; set; }

        public int RegionId { get; set; }

        public int CityId { get; set; }

        public int TownId { get; set; }

        public long StreetId { get; set; }

        public string BuildingDetails { get; set; }

        public List<GetTechnicalAddressOutputTechnologyItem> TechnologyItems { get; set; }
    }

    public class GetTechnicalAddressOutputTechnologyItem
    {
        public Guid Technology { get; set; }
        public bool TechnologyAvailable { get; set; }
        public bool SubscriptionFeasible { get; set; }
        public bool TelephonyFeasible { get; set; }
        public bool DataFeasible { get; set; }
        public string PanelNumber { get; set; }
        public List<GetTechnicalAddressOutputTechnologyItemNetworkElement> NetworkElements { get; set; }
    }

    public class GetTechnicalAddressOutputTechnologyItemNetworkElement
    {
        public long ID { get; set; }
        public Guid Type { get; set; }
        public string Number { get; set; }
    }
}
