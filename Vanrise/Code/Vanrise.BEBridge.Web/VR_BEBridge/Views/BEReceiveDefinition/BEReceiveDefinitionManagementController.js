(function (appControllers) {

    "use strict";

    function beReceiveDefinitionManagementController($scope, beReceiveDefinitionService, beRecieveDefinitionAPIService, VRUIUtilsService, UtilsService) {

        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function buildGridQuery() {
           
            return {
                Name: $scope.scopeModel.name,
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
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

            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
    }

    beReceiveDefinitionManagementController.$inject = ['$scope', 'VR_BEBridge_BEReceiveDefinitionService', 'VR_BEBridge_BERecieveDefinitionAPIService', 'VRUIUtilsService','UtilsService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionManagementController', beReceiveDefinitionManagementController);
})(appControllers);