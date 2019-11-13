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
    public class TechnicalAddressManager
    {
        GenericBusinessEntityManager _genericBEManager = new GenericBusinessEntityManager();
        
        #region Public Methods
        public GetTechnicalAddressOutput GetTechnicalAddress(TechnicalAddressNumberType numberType, string number)
        {
            NodeManager _nodeManager = new NodeManager();

            var output = new GetTechnicalAddressOutput
            {
                TechnologyItems = new List<GetTechnicalAddressOutputTechnologyItem>()
            };
            Guid businessEntityDefinitionId = Guid.Empty;
            switch (numberType)
            {
                case TechnicalAddressNumberType.DPNumber:
                    businessEntityDefinitionId = StaticBEDefinitionIDs.DPBEDefinitionId;
                    break;
                case TechnicalAddressNumberType.FDBNumber:
                    businessEntityDefinitionId = StaticBEDefinitionIDs.FDBBEDefinitionId;
                    break;
            }
            var nodeNeeded = _nodeManager.GetNodeByNumber(number, businessEntityDefinitionId);
            if (nodeNeeded == null)
            {
                output.TechnologyItems.Add(GetDPTechnicalAddress(null));
                output.TechnologyItems.Add(GetFDBTechnicalAddress(null));
                return output;

            }
            output.AreaId = nodeNeeded.AreaId;
            output.SiteId = nodeNeeded.SiteId;
            output.RegionId = nodeNeeded.RegionId;
            output.CityId = nodeNeeded.CityId;
            output.TownId = nodeNeeded.TownId;
            output.StreetId = nodeNeeded.StreetId;
            output.BuildingDetails = nodeNeeded.Building;


            switch (numberType)
            {
                case TechnicalAddressNumberType.DPNumber:
                    var fdbNode = _nodeManager.GetNodeByAddress(StaticBEDefinitionIDs.FDBBEDefinitionId, output.AreaId, output.SiteId, output.RegionId, output.CityId, output.TownId, output.StreetId, output.BuildingDetails);
                    output.TechnologyItems.Add(GetDPTechnicalAddress(nodeNeeded));
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(fdbNode));
                    break;
                case TechnicalAddressNumberType.FDBNumber:
                    var dpNode = _nodeManager.GetNodeByAddress(StaticBEDefinitionIDs.DPBEDefinitionId, output.AreaId, output.SiteId, output.RegionId, output.CityId, output.TownId, output.StreetId, output.BuildingDetails);
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(nodeNeeded));
                    output.TechnologyItems.Add(GetDPTechnicalAddress(dpNode));
                    break;
            }
            return output;
        }

        public GetTechnicalAddressOutput GetTechnicalAddressByPath(long pathId)
        {
            var pathConnections = _genericBEManager.GetAllGenericBusinessEntities(StaticBEDefinitionIDs.PathConnectionBEDefinitionId, null, new RecordFilterGroup
            {
                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = "Path",
                        Values = new List<object>{ pathId }
                    }
                }
            });

            var output = new GetTechnicalAddressOutput
            {
                TechnologyItems = new List<GetTechnicalAddressOutputTechnologyItem>()
            };

            if (pathConnections != null && pathConnections.Count > 0)
            {
                NodeManager nodeManager = new NodeManager();
                bool isFDBFound = false;
                bool isDPFound = false;
                Node nodeEntity = null;
                foreach (var pathConnection in pathConnections)
                {
                    var port1NodeTypeId = (Guid)pathConnection.FieldValues.GetRecord("Port1NodeType");
                    var port1NodeId = (long)pathConnection.FieldValues.GetRecord("Port1Node");
                    var port2NodeTypeId = (Guid)pathConnection.FieldValues.GetRecord("Port2NodeType");
                    var port2NodeId = (long)pathConnection.FieldValues.GetRecord("Port2Node");

                    if (port1NodeTypeId == StaticBEDefinitionIDs.DPNodeTypeId)
                    {
                        nodeEntity = nodeManager.GetNode(port1NodeId);
                        isDPFound = true;
                        break;
                    }
                    if (port2NodeTypeId == StaticBEDefinitionIDs.DPNodeTypeId)
                    {
                        nodeEntity = nodeManager.GetNode(port2NodeId);
                        isDPFound = true;
                        break;
                    }

                    if (port1NodeTypeId == StaticBEDefinitionIDs.FDBNodeTypeId)
                    {
                        nodeEntity = nodeManager.GetNode(port1NodeId);
                        isFDBFound = true;
                        break;
                    }
                    if (port2NodeTypeId == StaticBEDefinitionIDs.FDBNodeTypeId)
                    {
                        nodeEntity = nodeManager.GetNode(port2NodeId);
                        isFDBFound = true;
                        break;
                    }
                }

                output.AreaId = nodeEntity.AreaId;
                output.SiteId = nodeEntity.SiteId;
                output.RegionId = nodeEntity.RegionId;
                output.CityId = nodeEntity.CityId;
                output.TownId = nodeEntity.TownId;
                output.StreetId = nodeEntity.StreetId;
                output.BuildingDetails = nodeEntity.Building;

                if (isFDBFound)
                {
                    var dpNode = nodeManager.GetNodeByAddress(StaticBEDefinitionIDs.DPBEDefinitionId, output.AreaId, output.SiteId, output.RegionId, output.CityId, output.TownId, output.StreetId, output.BuildingDetails);
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(nodeEntity));
                    output.TechnologyItems.Add(GetDPTechnicalAddress(dpNode));
                }
                if(isDPFound)
                {
                    var fdbNode = nodeManager.GetNodeByAddress(StaticBEDefinitionIDs.FDBBEDefinitionId, output.AreaId, output.SiteId, output.RegionId, output.CityId, output.TownId, output.StreetId, output.BuildingDetails);
                    output.TechnologyItems.Add(GetFDBTechnicalAddress(fdbNode));
                    output.TechnologyItems.Add(GetDPTechnicalAddress(nodeEntity));
                }
            }
            else
            {
                output.TechnologyItems.Add(GetDPTechnicalAddress(null));
                output.TechnologyItems.Add(GetFDBTechnicalAddress(null));
            }
            return output;

        }
        #endregion

        #region Private Methods
        private GetTechnicalAddressOutputTechnologyItem GetFDBTechnicalAddress(Node node)
        {
            ConnectionManager _connectionManager = new ConnectionManager();
            NodeManager _nodeManager = new NodeManager();

            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.FiberTechnology,
                PanelNumber = node != null? node.Number:null
            };
            if (node == null)
                return item;

            bool targetReach = false;
            var nodeId = node.NodeId;
            item.NetworkElements = new List<GetTechnicalAddressOutputTechnologyItemNetworkElement>();
            item.Connections = new List<GetTechnicalAddressOutputTechnologyItemConnection>();
            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
            {
                ID = nodeId,
                Number = node.Number,
                Type = node.NodeTypeId
            });

            item.SubscriptionFeasible = _connectionManager.CheckConnectionWithFreePort(nodeId, null);
            List<long> usedNodeIds = new List<long>();
            while (!targetReach)
            {
                var connection = _connectionManager.GetConnection(nodeId, usedNodeIds);
                if (connection == null)
                    return item;

                item.Connections.Add(new GetTechnicalAddressOutputTechnologyItemConnection
                {
                    ConnectionId = connection.ConnectionId,
                    Port1Id = connection.Port1Id,
                    Port2Id = connection.Port2Id
                });

                usedNodeIds.Add(nodeId);
                if (connection.Port1NodeId != nodeId)
                {
                    nodeId = connection.Port1NodeId;
                }
                else if (connection.Port2NodeId != nodeId)
                {
                    nodeId = connection.Port2NodeId;
                }

                var nodeItem = _nodeManager.GetNode(nodeId);
                if (nodeItem == null)
                    return item;

                var nodeTypeId = nodeItem.NodeTypeId;
                item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                {
                    ID = nodeId,
                    Number = nodeItem.Number,
                    Type = nodeTypeId
                });

                if (nodeTypeId == StaticBEDefinitionIDs.OLTNodeTypeId)
                {
                    targetReach = true;
                    item.TechnologyAvailable = true;
                }
            }

            if (item.SubscriptionFeasible)
            {
                item.DataFeasible = true;
                item.TelephonyFeasible = true;
            }
            return item;
        }
        private GetTechnicalAddressOutputTechnologyItem GetDPTechnicalAddress(Node node)
        {
            ConnectionManager _connectionManager = new ConnectionManager();
            NodeManager _nodeManager = new NodeManager();

            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.CopperTechnology,
                PanelNumber = node != null ? node.Number : null
            };

            if (node == null)
                return item;

            bool targetReach = false;
            var nodeId =  node.NodeId;
            item.NetworkElements = new List<GetTechnicalAddressOutputTechnologyItemNetworkElement>();
            item.Connections = new List<GetTechnicalAddressOutputTechnologyItemConnection>();

            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
            {
                ID = nodeId,
                Number = node.Number,
                Type = node.NodeTypeId
            });

            List<long> usedNodeIds = new List<long>();
            var subscriptionFeasible = true;

            while (!targetReach)
            {
                var connection = _connectionManager.GetConnection(nodeId, usedNodeIds);
                if (connection == null)
                    return item;

                if (!_connectionManager.CheckConnectionWithFreePort(nodeId, usedNodeIds))
                {
                    subscriptionFeasible = false;
                }

                item.Connections.Add(new GetTechnicalAddressOutputTechnologyItemConnection
                {
                    ConnectionId = connection.ConnectionId,
                    Port1Id = connection.Port1Id,
                    Port2Id = connection.Port2Id
                });
                usedNodeIds.Add(nodeId);
                if (connection.Port1NodeId != nodeId)
                {
                    nodeId = connection.Port1NodeId;
                }
                else if (connection.Port2NodeId != nodeId)
                {
                    nodeId = connection.Port2NodeId;
                }

                var nodeItem = _nodeManager.GetNode(nodeId);
                if (nodeItem == null)
                    return item;

                var nodeTypeId = nodeItem.NodeTypeId;
                item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                {
                    ID = nodeId,
                    Number = nodeItem.Number,
                    Type = nodeTypeId
                });

                if (nodeTypeId == StaticBEDefinitionIDs.MDFTypeId)
                {
                    targetReach = true;
                    item.TechnologyAvailable = true;

                    if (subscriptionFeasible)
                    {
                        var switchItem = _nodeManager.GetSwitchBySiteId(nodeItem.SiteId);
                        if (switchItem != null)
                        {
                            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                            {
                                ID = switchItem.NodeId,
                                Number = switchItem.Number,
                                Type = switchItem.NodeTypeId
                            });
                            item.TelephonyFeasible = true;
                        }
                        var dslam = _nodeManager.GetDslamBySiteId(nodeItem.SiteId);
                        if (dslam != null)
                        {
                            item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                            {
                                ID = dslam.NodeId,
                                Number = dslam.Number,
                                Type = dslam.NodeTypeId
                            });
                            item.DataFeasible = true;
                        }

                    }
                }
            }

            item.SubscriptionFeasible = subscriptionFeasible;
            return item;
        }
        #endregion
    }
}
