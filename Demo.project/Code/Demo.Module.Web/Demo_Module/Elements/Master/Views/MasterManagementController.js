(function (appControllers) {
    "use strict";
    masterManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_MasterService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_CompanyService'];
    function masterManagementController($scope, VRNotificationService, Demo_Module_MasterService, UtilsService, VRUIUtilsService, Demo_Module_CompanyService) {
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
                return gridApi.load(getFilter());
            };
            $scope.scopeModel.addMaster = function () {
                var onMasterAdded = function (master) {
                    if (gridApi != undefined) {
                        gridApi.onMasterAdded(master);
                    }
                };
                Demo_Module_MasterService.addMaster(onMasterAdded);
            };
        };
        function load() {
        }
        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };
    };

    appControllers.controller('Demo_Module_MasterManagementController', masterManagementController);
})(appControllers);