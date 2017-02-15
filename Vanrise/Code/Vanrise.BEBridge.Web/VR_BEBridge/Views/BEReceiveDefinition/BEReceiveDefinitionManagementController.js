(function (appControllers) {

    "use strict";

    function beReceiveDefinitionManagementController($scope, beReceiveDefinitionService, beRecieveDefinitionAPIService) {

        var gridAPI;
        defineScope();


        function buildGridQuery() {
           
            return {
                Name: $scope.scopeModel.name
            };
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.name = "";
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onReceiveDefinitionAdded = function (addReceiveDefinition) {
                    gridAPI.onReceiveDefinitionAdded(addReceiveDefinition);
                };
                beReceiveDefinitionService.addReceiveDefinition(onReceiveDefinitionAdded);
            };

            $scope.scopeModel.hasAddBEReceiveDefinitionPermission = function () {
                return beRecieveDefinitionAPIService.HasAddReceiveDefinitionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
    }

    beReceiveDefinitionManagementController.$inject = ['$scope', 'VR_BEBridge_BEReceiveDefinitionService', 'VR_BEBridge_BERecieveDefinitionAPIService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionManagementController', beReceiveDefinitionManagementController);
})(appControllers);