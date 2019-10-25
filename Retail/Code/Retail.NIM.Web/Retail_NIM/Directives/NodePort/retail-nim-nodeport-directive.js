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
            var nodePartId;
            var nodePortTypeId;
            var nodeTypeId;
            var nodePortId;
            var nodePortBEDefinitionId;
            var nodeBEDefinitionId;

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
                    if (selectedItem != undefined) {
                        $scope.scopeModel.isNodeSelected = true;
                        nodeId = selectedItem.GenericBusinessEntityId;
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
                    var nodePortDefinitionId = Retail_NIM_NodePortService.getNodePortDefinitionId();
                    var nodeDefinitionId = Retail_NIM_NodeService.getNodeDefinitionId();

                    var rootPromiseNode;
                    var nodeEntity;
                    var nodePortEntity;


                    if (payload != undefined) {
                        nodePortId = payload.selectedIds;
                    }

                    if (nodePortId != undefined) {
                        nodePortTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        nodeTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        nodeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        nodePartTreeSelectedDeferred = UtilsService.createPromiseDeferred();

                        rootPromiseNode = { 
                            promises: [getNodePort()],
                            getChildNode: function () {
                                return {
                                    promises: [loadNodePortTypeSelector()],
                                    getChildNode: function () {
                                        return {
                                            promises: [getNode()],
                                            getChildNode: function () {
                                                var promises = [loadNodeTypeSelector(), loadNodeSelector(), loadNodePortSelector()];
                                                return {
                                                    promises: promises,
                                                    getChildNode: function () {
                                                        return {
                                                            promises: [nodePartTreeReadyDeferred.promise, nodeSelectedPromiseDeferred.promise],
                                                            getChildNode: function () {
                                                                return {
                                                                    promises: [getNodeParts()]
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
                        };

                    } else {

                        rootPromiseNode = {
                            promises: [loadNodePortTypeSelector(), loadNodeTypeSelector()]
                        };
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        nodePortTypeSelectedPromiseDeferred = undefined;
                        nodeTypeSelectedPromiseDeferred = undefined;
                        nodeSelectedPromiseDeferred = undefined;
                        nodePartTreeSelectedDeferred = undefined;
                    });

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
                            }
                        });
                    }
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
                        selectedIds: nodePortTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(nodePortTypeSelectorAPI, nodePortTypeSelectorPayload, nodePortTypeSelectorLoadPromiseDeferred);

                });

                return nodePortTypeSelectorLoadPromiseDeferred.promise;
            }

            function loadNodeTypeSelector() {
                var nodeTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                nodeTypeSelectorReadyDeferred.promise.then(function () {
                    var nodeTypeSelectorPayload = {
                        selectedIds: nodeTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(nodeTypeSelectorAPI, nodeTypeSelectorPayload, nodeTypeSelectorLoadPromiseDeferred);
                });

                return nodeTypeSelectorLoadPromiseDeferred.promise;
            }

            function loadNodePortSelector() {
                var nodePortSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                UtilsService.waitMultiplePromises([nodePortSelectorReadyDeferred.promise, nodePortTypeSelectedPromiseDeferred.promise, nodeSelectedPromiseDeferred.promise]).then(function () {
                    var nodePortSelectorPayload = {
                        businessEntityDefinitionId: nodePortBEDefinitionId,
                        filter: {
                            Filters: [{
                                "$type": "Retail.NIM.Business.NodePortInfoFilter, Retail.NIM.Business",
                                NodeId: nodeId,
                                NodePartId: nodePartId
                            }]
                        },
                        selectedIds: nodePortId,
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
                        selectIfSingleItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(nodeSelectorAPI, nodeSelectorPayload, nodeSelectorLoadPromiseDeferred);
                });
                return nodeSelectorLoadPromiseDeferred.promise;
            }

            function getNodeParts() {
                nodeParts.length = 0;
                return Retail_NIM_NodePartAPIService.GetNodePartTree(nodeId).then(function (response) {
                    if (response.ChildNodes != undefined) {
                        for (var i = 0; i < response.ChildNodes.length; i++) {
                            nodeParts.push(response.ChildNodes[i]);
                        }
                        buildNodePartTree();
                        if (nodePartTreeSelectedDeferred != undefined) {
                            $scope.scopeModel.currentNodePartNode = nodePartTreeAPI.setSelectedNode($scope.scopeModel.nodeParts, nodePartId, "nodePartId", "childParts");
                            nodePartTreeAPI.refreshTree($scope.scopeModel.nodeParts);
                            $scope.scopeModel.isLoadingNodeParts = false;
                        }
                    }
                });
            }

            function buildNodePartTree() {
                $scope.scopeModel.nodeParts.length = 0;
                for (var i = 0; i < nodeParts.length; i++) {
                    var nodePart = nodeParts[i];
                    $scope.scopeModel.nodeParts.push(mapNodePartToTreeItem(nodePart));
                }
                nodePartTreeAPI.refreshTree($scope.scopeModel.nodeParts);
            }

            function mapNodePartToTreeItem(nodePart) {
                var nodePartTreeItem = {
                    nodePartId: nodePart.Id,
                    nodePartName: nodePart.Number,
                    childParts: [],
                    hasRemoteChildren: true,
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

            function getFilteredNodePorts() {
                if (nodeId != undefined && nodePortBEDefinitionId != undefined) {
                    var nodePortSelectorPayload = {
                        businessEntityDefinitionId: nodePortBEDefinitionId,
                        filter: {
                            Filters: [{
                                $type: "Retail.NIM.Business.NodePortInfoFilter, Retail.NIM.Business",
                                NodeId: nodeId,
                                NodePartId: nodePartId
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