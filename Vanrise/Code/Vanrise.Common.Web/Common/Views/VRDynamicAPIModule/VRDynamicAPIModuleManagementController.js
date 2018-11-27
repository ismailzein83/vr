(function (appControllers) {
    "use strict";

    vrDynamicAPIModuleManagementController.$inject = ['$scope', 'VRNotificationService', 'VR_Dynamic_API_ModuleService', 'VRCommon_VRDynamicAPIModuleAPIService','VRCommon_VRDynamicAPIAPIService','UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIModuleManagementController($scope, VRNotificationService, VR_Dynamic_API_ModuleService, VRCommon_VRDynamicAPIModuleAPIService, VRCommon_VRDynamicAPIAPIService, UtilsService, VRUIUtilsService) {

        var gridApi;
        defineScope();
      load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
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
                VR_Dynamic_API_ModuleService.addVRDynamicAPIModule(onVRDynamicAPIModuleAdded);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
                 $scope.scopeModel.isLoading = false;
        }
   

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                }
            };
        }
    }

    appControllers.controller('VR_Dynamic_API_ModuleManagementController', vrDynamicAPIModuleManagementController);
})(appControllers);