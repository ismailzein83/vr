"use strict";

app.directive("retailNimFtthpathDesigner", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FTTHPathDesigner($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_NIM/Directives/FTTHPath/Templates/FTTHPathTemplate.html"
        };

        function FTTHPathDesigner($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var networkPathDesignerDirectiveAPI;
            var networkPathDesignerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onNetworkPathDesignerReady = function (api) {
                    networkPathDesignerDirectiveAPI = api;
                    networkPathDesignerDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedValues;

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                    }

                    var promises = [];

                    var loadNetworkPathDesignerDirectivePromise = loadNetworkPathDesignerDirective();
                    promises.push(loadNetworkPathDesignerDirectivePromise);


                    function loadNetworkPathDesignerDirective() {

                        var networkPathDesignerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        networkPathDesignerDirectiveReadyDeferred.promise.then(function () {

                            var networkPathDesignerDirectivePayload = {
                                networkNodes: buildNetworkNodes(selectedValues),
                                networkConnectors: buildConnetorNodes(selectedValues)
                            }
                            VRUIUtilsService.callDirectiveLoad(networkPathDesignerDirectiveAPI, networkPathDesignerDirectivePayload, networkPathDesignerDirectiveLoadDeferred);
                        });

                        return networkPathDesignerDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildNetworkNodes(selectedValues) {
                if (selectedValues == undefined)
                    return;

                var networkNodes = [];

                var ims = { Id: buildNodeId("IMS", selectedValues.IMS), Name: selectedValues.IMSName, Type: "default" };
                networkNodes.push(ims);

                var olt = { Id: buildNodeId("OLT", selectedValues.OLT), Name: selectedValues.OLTName, Type: "default" };
                networkNodes.push(olt);

                var splitter = { Id: buildNodeId("Splitter", selectedValues.Splitter), Name: selectedValues.SplitterName, Type: "splitter" };
                networkNodes.push(splitter);

                var fdb = { Id: buildNodeId("FDB", selectedValues.FDB), Name: selectedValues.FDBName, Type: "default" };
                networkNodes.push(fdb);

                return networkNodes;
            }

            function buildConnetorNodes(selectedValues) {
                if (selectedValues == undefined)
                    return;

                var connectorNodes = [];

                var imsOLTConnector = {
                    SourceNodeId: buildNodeId("IMS", selectedValues.IMS),
                    SourcePortNumber: selectedValues.IMSTID,
                    DestinationNodeId: buildNodeId("OLT", selectedValues.OLT),
                    DestinationPortNumber: selectedValues.OLTHorizontalPort
                };
                connectorNodes.push(imsOLTConnector);

                var splitterFDBConnector = {
                    SourceNodeId: buildNodeId("Splitter", selectedValues.Splitter),
                    SourcePortNumber: selectedValues.SplitterOutPort,
                    DestinationNodeId: buildNodeId("FDB", selectedValues.FDB),
                    DestinationPortNumber: selectedValues.FDBPort
                };
                connectorNodes.push(splitterFDBConnector);

                return connectorNodes;
            }

            function buildNodeId(nodeName, nodeId) {
                return nodeName + "_" + nodeId;
            }
        }

        return directiveDefinitionObject;
    }
]);