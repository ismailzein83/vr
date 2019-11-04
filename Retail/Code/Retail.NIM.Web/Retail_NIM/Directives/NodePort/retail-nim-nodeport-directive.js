"use strict";

app.directive("retailNimNodeportDirective", ["VR_GenericData_GenericBusinessEntityAPIService", "Retail_NIM_NodePartAPIService", "Retail_NIM_NodePortService", "Retail_NIM_NodeService", "VRUIUtilsService", "UtilsService",
    function (VR_GenericData_GenericBusinessEntityAPIService, Retail_NIM_NodePartAPIService, Retail_NIM_NodePortService, Retail_NIM_NodeService, VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new NodePortCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_NIM/Directives/NodePort/Templates/NodePortDirectiveTemplate.html"

        };

        function NodePortCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var nodeId;
            var nodeNumber;
            var nodePartId;
            var nodePortTypeId;
            var nodeTypeId;
            var nodePortId;
            var nodePortBEDefinitionId;
            var nodeBEDefinitionId;
            var nodeEntity;
            var nodePortEntity;
            var nodePortDefinitionId;
            var nodeDefinitionId;
            var area;
            var site;



            var disableNode = false;
            var disableNodePart = false;
            var disableNodePort = false;

            var connectionDirection;

            var nodePortTypeSelectorAPI;
            var nodePortTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var nodePortTypeSelectedPromiseDeferred;

            var nodeTypeSelectorAPI;
            var nodeTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var nodeTypeSelectedPromiseDeferred;

            var nodeSelectorAPI;
            var nodeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var nodeSelectedPromiseDeferred;

            var nodeParts = [];
            var nodePartTreeAPI;
            var nodePartTreeReadyDeferred = UtilsService.createPromiseDeferred();

            var nodePortSelectorAPI;
            var nodePortSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var nodePartTreeSelectedDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.nodeParts = [];
                $scope.scopeModel.isLoadingNodeParts = true;
                $scope.scopeModel.isNodeSelected = false;
                $scope.scopeModel.isNodeDisabled = false;

                $scope.scopeModel.onNodePortTypeSelectorReady = function (api) {
                    nodePortTypeSelectorAPI = api;
                    nodePortTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onNodeTypeSelectorReady = function (api) {
                    nodeTypeSelectorAPI = api;
                    nodeTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onNodeSelectorReady = function (api) {
                    nodeSelectorAPI = api;
                    nodeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onNodePortSelectorReady = function (api) {
                    nodePortSelectorAPI = api;
                    nodePortSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onNodeTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        nodeBEDefinitionId = selectedItem.BusinessEntitityDefinitionId;
                        if (nodeTypeSelectedPromiseDeferred != undefined) {
                            nodeTypeSelectedPromiseDeferred.resolve();
                        } else {
                            var nodeSelectorPayload = {
                                businessEntityDefinitionId: nodeBEDefinitionId,
                                filter: {
                                    Filters: [{
                                        "$type": "Retail.NIM.Business.NodeInfoFilter, Retail.NIM.Business",
                                        AreaId: area,
                                        SiteId: site
                                    }]
                                },
                                selectIfSingleItem: true
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingNodeSelector = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, nodeSelectorAPI, nodeSelectorPayload, setLoader);
                        }
                    }
                };
                $scope.scopeModel.onNodePortTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        $scope.scopeModel.isNodePortTypeSelected = true;
                        nodePortBEDefinitionId = selectedItem.BusinessEntitityDefinitionId;
                        if (nodePortTypeSelectedPromiseDeferred != undefined) {
                            nodePortTypeSelectedPromiseDeferred.resolve();
                        } else {
                            getFilteredNodePorts();
                        }
                    }
                };
                $scope.scopeModel.onNodeSelectionChanged = function (selectedItem) {
                    $scope.scopeModel.isNodeSelected = selectedItem != undefined;
                    if (selectedItem != undefined) {
                        nodeId = selectedItem.GenericBusinessEntityId;
                        nodeNumber = selectedItem.Name;
                        if (nodeSelectedPromiseDeferred != undefined) {
                            nodeSelectedPromiseDeferred.resolve();
                        } else {
                            $scope.scopeModel.isLoadingNodeParts = true;
                            getNodeParts().then(function () {
                                $scope.scopeModel.isLoadingNodeParts = false;
                            });

                            getFilteredNodePorts();

                        }
                    } else {
                        $scope.scopeModel.isNodeSelected = false;
                    }
                };

                $scope.scopeModel.onNodePartTreeReady = function (api) {
                    nodePartTreeAPI = api;
                    nodePartTreeReadyDeferred.resolve();
                };
                $scope.scopeModel.loadChildNodeParts = function (currentNode) {
                    var childPartsPromiseDeffered = UtilsService.createPromiseDeferred();
                    childPartsPromiseDeffered.resolve(currentNode.childParts);
                    return childPartsPromiseDeffered.promise;
                };
                $scope.scopeModel.onNodePartTreeSelection = function () {
                    var selectedItem = $scope.scopeModel.currentNodePartNode;
                    if (selectedItem != undefined) {
                        if (nodePartTreeSelectedDeferred != undefined) {
                            nodePartTreeSelectedDeferred.resolve();
                        } else {
                            nodePartId = selectedItem.nodePartId;
                            if ($scope.scopeModel.isNodePortTypeSelected && $scope.scopeModel.isNodeSelected) {
                                getFilteredNodePorts();
                            }
                        }
                    }

                };

                UtilsService.waitMultiplePromises([nodePortTypeSelectorReadyDeferred.promise, nodeTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = { promises: [] };

                    nodePortDefinitionId = Retail_NIM_NodePortService.getNodePortDefinitionId();
                    nodeDefinitionId = Retail_NIM_NodeService.getNodeDefinitionId();

                    var parentFieldValues;

                    var isAddMode;

                    if (payload != undefined) {
                        nodePortId = payload.selectedIds;
                        parentFieldValues = payload.parentFieldValues;
                        area = payload.area;
                        site = payload.site;
                        isAddMode = payload.isAddMode;
                        connectionDirection = payload.connectionDirection;
                    }

                    if (parentFieldValues != undefined) {
                        disableNode = parentFieldValues.NodeId != undefined;
                        disableNodePart = parentFieldValues.NodePartId != undefined;
                        disableNodePort = parentFieldValues.NodePortId != undefined;
                        if (isAddMode) {
                            nodeId = parentFieldValues.NodeId;
                            nodePartId = parentFieldValues.NodePartId;
                            nodePortId = parentFieldValues.NodePortId;
                        }
                    }

                    if (area != undefined && site != undefined) {

                        var promises = [];

                        if (nodePortId != undefined) {
                            nodePortTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(getNodePort());
                        }

                        rootPromiseNode = {
                            promises: promises,
                            getChildNode: function () {
                                var secondPromises = [loadNodePortTypeSelector()];
                                if (nodeId != undefined) {
                                    nodeTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                    nodeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                                    secondPromises.push(getNode());
                                }
                                return {
                                    promises: secondPromises,
                                    getChildNode: function () {
                                        var thirdPromises = [loadNodeTypeSelector()];
                                        if (nodeId != undefined) {
                                            thirdPromises.push(loadNodeSelector());
                                        }

                                        return {
                                            promises: thirdPromises,
                                            getChildNode: function () {
                                                var fourthPromises = [];
                                                if (nodePartId != undefined) {
                                                    nodePartTreeSelectedDeferred = UtilsService.createPromiseDeferred();
                                                    fourthPromises.push(nodeSelectedPromiseDeferred.promise, nodePartTreeReadyDeferred.promise);
                                                }
                                                return {
                                                    promises: fourthPromises,
                                                    getChildNode: function () {
                                                        var fifthPromises = [];
                                                        if (nodePortId != undefined) {
                                                            fifthPromises.push(loadNodePortSelector());
                                                        }
                                                        if (nodeId != undefined) {
                                                            fifthPromises.push( getNodeParts());
                                                        }
                                                        return {
                                                            promises: fifthPromises
                                                        };
                                                    }
                                                };
                                            }
                                        };
                                    }
                                };
                            }
                        };
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        nodePortTypeSelectedPromiseDeferred = undefined;
                        nodeTypeSelectedPromiseDeferred = undefined;
                        nodeSelectedPromiseDeferred = undefined;
                        nodePartTreeSelectedDeferred = undefined;
                    });
                };

                api.clearDataSource = function () {
                    nodePortTypeSelectorAPI.clearDataSource();
                    nodeTypeSelectorAPI.clearDataSource();
                    nodeSelectorAPI.clearDataSource();
                    nodePortSelectorAPI.clearDataSource();
                };

                api.getSelectedIds = function () {
                    return nodePortSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadNodePortTypeSelector() {
                var nodePortTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                nodePortTypeSelectorReadyDeferred.promise.then(function () {
                    var nodePortTypeSelectorPayload = {
                        selectedIds: nodePortTypeId,
                        isDisabled: disableNodePort,
                        selectIfSingleItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(nodePortTypeSelectorAPI, nodePortTypeSelectorPayload, nodePortTypeSelectorLoadPromiseDeferred);

                });

                return nodePortTypeSelectorLoadPromiseDeferred.promise;
            }

            function loadNodeTypeSelector() {
                var nodeTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                nodeTypeSelectorReadyDeferred.promise.then(function () {
                    var nodeTypeSelectorPayload = {
                        selectedIds: nodeTypeId,
                        isDisabled: disableNode,
                        selectIfSingleItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(nodeTypeSelectorAPI, nodeTypeSelectorPayload, nodeTypeSelectorLoadPromiseDeferred);
                });

                return nodeTypeSelectorLoadPromiseDeferred.promise;
            }

            function loadNodePortSelector() {
                var nodePortSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                var promises = [nodePortSelectorReadyDeferred.promise, nodePortTypeSelectedPromiseDeferred.promise];
                if (nodePartId != undefined)
                    promises.push(nodePartTreeSelectedDeferred.promise);

                UtilsService.waitMultiplePromises(promises).then(function () {
                    var nodePortSelectorPayload = {
                        businessEntityDefinitionId: nodePortBEDefinitionId,
                        filter: {
                            Filters: [{
                                "$type": "Retail.NIM.Business.NodePortInfoFilter, Retail.NIM.Business",
                                NodeId: nodeId,
                                NodePartId: nodePartId,
                                ConnectionDirection: connectionDirection
                            }]
                        },
                        selectedIds: nodePortId,
                        isDisabled: disableNodePort,
                        selectIfSingleItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(nodePortSelectorAPI, nodePortSelectorPayload, nodePortSelectorLoadPromiseDeferred);
                });
                return nodePortSelectorLoadPromiseDeferred.promise;
            }

            function loadNodeSelector() {
                var nodeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([nodeTypeSelectedPromiseDeferred.promise, nodeSelectorReadyDeferred.promise]).then(function () {
                    var nodeSelectorPayload = {
                        businessEntityDefinitionId: nodeBEDefinitionId,
                        selectedIds: nodeId,
                        filter: {
                            Filters: [{
                                "$type": "Retail.NIM.Business.NodeInfoFilter, Retail.NIM.Business",
                                AreaId: area,
                                SiteId: site
                            }]
                        },
                        isDisabled: disableNode,
                        selectIfSingleItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(nodeSelectorAPI, nodeSelectorPayload, nodeSelectorLoadPromiseDeferred);
                });
                return nodeSelectorLoadPromiseDeferred.promise;

            }

            function getNodeParts() {
                nodeParts.length = 0;
                return Retail_NIM_NodePartAPIService.GetNodePartTree(nodeId).then(function (response) {
                    if (response != undefined) {
                        var rootNode = {
                            Number: nodeNumber,
                            ChildNodes: response.ChildNodes
                        };
                        nodeParts.push(rootNode);

                        buildNodePartTree();

                        $scope.scopeModel.currentNodePartNode = nodePartId != undefined ? nodePartTreeAPI.setSelectedNode($scope.scopeModel.nodeParts, nodePartId, "nodePartId", "childParts") : undefined;

                        nodePartTreeAPI.refreshTree($scope.scopeModel.nodeParts);
                        $scope.scopeModel.isLoadingNodeParts = false;

                    }
                });
            }

            function buildNodePartTree() {
                $scope.scopeModel.nodeParts.length = 0;
                for (var i = 0; i < nodeParts.length; i++) {
                    var nodePart = nodeParts[i];
                    $scope.scopeModel.nodeParts.push(mapNodePartToTreeItem(nodePart));
                }
            }

            function mapNodePartToTreeItem(nodePart) {
                var nodePartTreeItem = {
                    nodePartId: nodePart.Id,
                    nodePartName: nodePart.Number,
                    childParts: [],
                    hasRemoteChildren: true,
                    isDisabled: disableNodePart,
                    type: 'NodePart'
                };
                if (nodePart.ChildNodes != undefined) {
                    for (var i = 0; i < nodePart.ChildNodes.length; i++) {
                        var childPart = nodePart.ChildNodes[i];
                        nodePartTreeItem.childParts.push(mapNodePartToTreeItem(childPart));
                    }
                }
                return nodePartTreeItem;
            }

            function getNodePort() {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(nodePortDefinitionId, nodePortId).then(function (response) {
                    if (response != undefined) {
                        nodePortEntity = response.FieldValues;
                    }
                    if (nodePortEntity != undefined) {
                        nodePortTypeId = nodePortEntity.Type;
                        nodeId = nodePortEntity.Node;
                        nodePartId = nodePortEntity.Part;
                    }
                });
            }

            function getNode() {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(nodeDefinitionId, nodeId).then(function (response) {
                    if (response != undefined) {
                        nodeEntity = response.FieldValues;
                    }

                    if (nodeEntity != undefined) {
                        nodeTypeId = nodeEntity.NodeType;
                        nodeNumber = nodeEntity.Number;
                    }
                });
            }

            function getFilteredNodePorts() {
                if (nodeId != undefined && nodePortBEDefinitionId != undefined) {
                    var nodePortSelectorPayload = {
                        businessEntityDefinitionId: nodePortBEDefinitionId,
                        filter: {
                            Filters: [{
                                $type: "Retail.NIM.Business.NodePortInfoFilter, Retail.NIM.Business",
                                NodeId: nodeId,
                                NodePartId: nodePartId,
                                ConnectionDirection: connectionDirection
                            }]
                        },
                        selectIfSingleItem: true
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingNodePortSelector = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, nodePortSelectorAPI, nodePortSelectorPayload, setLoader);
                }
            }

        }
        return directiveDefinitionObject;
    }]);