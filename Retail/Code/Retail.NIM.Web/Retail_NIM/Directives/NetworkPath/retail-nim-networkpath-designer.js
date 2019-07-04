"use strict";

app.directive("retailNimNetworkpathDesigner", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NetworkPathDesigner($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_NIM/Directives/NetworkPath/Templates/NetworkPathTemplate.html"
        };

        function NetworkPathDesigner($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.networkNodes = [];
                ctrl.networkConnectors = [];

                ctrl.getInConnectorPortNumber = function (nodeId) {
                    var connection = UtilsService.getItemByVal(ctrl.networkConnectors, nodeId, 'SourceNodeId');
                    return connection && connection.DestinationPortNumber ? connection.DestinationPortNumber : undefined;
                };

                ctrl.getOutConnectorPortNumber = function (nodeId) {
                    var connection = UtilsService.getItemByVal(ctrl.networkConnectors, nodeId, 'SourceNodeId');
                    return connection && connection.SourcePortNumber ? connection.SourcePortNumber : undefined;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.networkNodes.length = 0;
                    ctrl.networkConnectors.length = 0;

                    if (payload) {
                        if (payload.networkNodes != undefined)
                            ctrl.networkNodes = payload.networkNodes;

                        if (payload.networkConnectors != undefined)
                            ctrl.networkConnectors = payload.networkConnectors;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);