(function (appControllers) {

    "use strict";

    VRObjectTypeDefinitionManagementController.$inject = ['$scope', 'VRCommon_VRObjectTypeDefinitionAPIService', 'VRCommon_VRObjectTypeDefinitionService', 'UtilsService', 'VRUIUtilsService','VRNotificationService'];

    function VRObjectTypeDefinitionManagementController($scope, VRCommon_VRObjectTypeDefinitionAPIService, VRCommon_VRObjectTypeDefinitionService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onVRObjectTypeDefinitionAdded = function (addedVRObjectTypeDefinition) {
                    gridAPI.onVRObjectTypeDefinitionAdded(addedVRObjectTypeDefinition);
                };
                VRCommon_VRObjectTypeDefinitionService.addVRObjectTypeDefinition(onVRObjectTypeDefinitionAdded);
            };

            $scope.scopeModel.hasAddVRObjectTypeDefinitionPermission = function () {
                return VRCommon_VRObjectTypeDefinitionAPIService.HasAddVRObjectTypeDefinitionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VRCommon_VRObjectTypeDefinitionManagementController', VRObjectTypeDefinitionManagementController);

})(appControllers);