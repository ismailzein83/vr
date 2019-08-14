(function (appControllers) {
    "use strict";

    vrDynamicAPIModuleManagementController.$inject = ['$scope', 'VRNotificationService', 'VRCommon_DynamicAPIModuleService', 'VRCommon_VRDynamicAPIModuleAPIService','VRCommon_VRDynamicAPIAPIService','UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIModuleManagementController($scope, VRNotificationService, VRCommon_DynamicAPIModuleService, VRCommon_VRDynamicAPIModuleAPIService, VRCommon_VRDynamicAPIAPIService, UtilsService, VRUIUtilsService) {

        var gridApi;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
      load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };
            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
            $scope.scopeModel.search = function () {
                VRCommon_VRDynamicAPIAPIService.BuildAllDynamicAPIControllers();
                return gridApi.load(getFilter());
            };

            $scope.scopeModel.addVRDynamicAPIModule = function () {
                var onVRDynamicAPIModuleAdded = function (dynamicAPI) {
                    if (gridApi != undefined) {
                        gridApi.onVRDynamicAPIModuleAdded(dynamicAPI);
                    }
                };
                VRCommon_DynamicAPIModuleService.addVRDynamicAPIModule(onVRDynamicAPIModuleAdded);
            };

            $scope.scopeModel.hasAddVRDynamicAPIModulePermission = function () {
                return VRCommon_VRDynamicAPIModuleAPIService.HasAddVRDynamicAPIModulePermission();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
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


        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
                }
            };
        }
    }

    appControllers.controller('VR_Dynamic_API_ModuleManagementController', vrDynamicAPIModuleManagementController);
})(appControllers);