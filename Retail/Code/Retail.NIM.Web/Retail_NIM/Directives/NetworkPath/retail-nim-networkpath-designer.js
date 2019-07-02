"use strict";

app.directive("retailNimNetworkpathDesigner", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new NetworkpathDesigner($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_NIM/Directives/NetworkPath/Templates/NetworkPathTemplate.html"

        };

        function NetworkpathDesigner($scope, ctrl, $attrs) {
            ctrl.networkNodes = [];
            ctrl.networkConnecters = [];
            this.initializeController = initializeController;
            $scope.scopeModel = {};
                      

            ctrl.getOutConnecterPortNumber = function (nodeId) {
                var connection = UtilsService.getItemByVal(ctrl.networkConnecters, nodeId, 'SourceNodeId');
                return connection && connection.SourcePortNumber;
            };

            ctrl.getInConnecterPortNumber = function (nodeId) {
                var connection = UtilsService.getItemByVal(ctrl.networkConnecters, nodeId, 'SourceNodeId');
                return connection && connection.DestinationPortNumber;
            };

            

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    ctrl.networkNodes.length = 0 ;
                    ctrl.networkConnecters.length = 0;
                    if (payload) {
                        ctrl.networkNodes = payload.networkNodes || [];
                        ctrl.networkConnecters = payload.networkConnecters || [];
                    }                   
                    return UtilsService.waitMultiplePromises(promises);

                };
               
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);