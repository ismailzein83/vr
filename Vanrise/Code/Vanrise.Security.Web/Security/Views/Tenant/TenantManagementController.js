(function (appControllers) {
    'use strict';

    TenatManagementController.$inject = ['$scope', 'VR_Sec_TenantService', 'VR_Sec_TenantAPIService'];

    function TenatManagementController($scope, VR_Sec_TenantService, VR_Sec_TenantAPIService) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addTenant = function () {
                var onTenantAdded = function (tenantObj) {
                    gridAPI.onTenantAdded(tenantObj);
                };

                VR_Sec_TenantService.addTenant(onTenantAdded);
            };

            $scope.hasAddTenantPermission = function () {
                return VR_Sec_TenantAPIService.HasAddTenantPermission();
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('VR_Sec_TenantManagementController', TenatManagementController);

})(appControllers);
