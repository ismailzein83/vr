using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class TechnicalAddressManager
    {

        public GetTechnicalAddressOutputTechnologyItem GetFDBTechnicalAddress(GenericBusinessEntity node)
        {
            ConnectionManager _connectionManager = new ConnectionManager();
            NodeManager _nodeManager = new NodeManager();

            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.FiberTechnology,
                PanelNumber = node != null ? node.FieldValues.GetRecord("Number").ToString() : null
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

            item.SubscriptionFeasible = _connectionManager.CheckConnectionWithFreePort(nodeId, null);
            List<long> usedNodeIds = new List<long>();
            while (!targetReach)
            {
                var connection = _connectionManager.GetConnection(nodeId, usedNodeIds);
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

                var nodeItem = _nodeManager.GetNode(nodeId);
                if (nodeItem == null)
                    return item;

                var nodeTypeId = (Guid)nodeItem.FieldValues.GetRecord("NodeType");
                item.NetworkElements.Add(new GetTechnicalAddressOutputTechnologyItemNetworkElement
                {
                    ID = nodeId,
                    Number = (string)nodeItem.FieldValues.GetRecord("Number"),
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

        public GetTechnicalAddressOutputTechnologyItem GetDPTechnicalAddress(GenericBusinessEntity node)
        {
            ConnectionManager _connectionManager = new ConnectionManager();
            NodeManager _nodeManager = new NodeManager();

            var item = new GetTechnicalAddressOutputTechnologyItem
            {
                Technology = StaticBEDefinitionIDs.CopperTechnology,
                PanelNumber = node != null ? node.FieldValues.GetRecord("Number").ToString() : null
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
                var connection = _connectionManager.GetConnection(nodeId, usedNodeIds);
                if (connection == null)
                    return item;

                if (!_connectionManager.CheckConnectionWithFreePort(nodeId, usedNodeIds))
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

                var nodeItem = _nodeManager.GetNode(nodeId);
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

                    if (subscriptionFeasible)
                    {
                        if (_nodeManager.GetSwitchBySiteId((int)nodeItem.FieldValues.GetRecord("Site")) != null)
                            item.TelephonyFeasible = true;
                        if (_nodeManager.GetDslamBySiteId((int)nodeItem.FieldValues.GetRecord("Site")) != null)
                            item.DataFeasible = true;
                    }
                }
            }

            item.SubscriptionFeasible = subscriptionFeasible;
            return item;
        }

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
            output.AreaId = (long)nodeNeeded.FieldValues.GetRecord("Area");
            output.SiteId = (long)nodeNeeded.FieldValues.GetRecord("Site");
            output.RegionId = (int)nodeNeeded.FieldValues.GetRecord("Region");
            output.CityId = (int)nodeNeeded.FieldValues.GetRecord("City");
            output.TownId = (int)nodeNeeded.FieldValues.GetRecord("Town");
            output.StreetId = (long)nodeNeeded.FieldValues.GetRecord("Street");
            output.BuildingDetails = nodeNeeded.FieldValues.GetRecord("Building") as string;


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

       

    }
}
