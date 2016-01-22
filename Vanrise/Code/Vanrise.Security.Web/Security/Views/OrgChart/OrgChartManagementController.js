(function (appControllers) {

    'use strict';

    OrgChartManagementController.$inject = ['$scope', 'VR_Sec_OrgChartService', 'VRModalService', 'VRNotificationService'];

    function OrgChartManagementController($scope, VR_Sec_OrgChartService, VRModalService, VRNotificationService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };
            $scope.search = function () {
                gridAPI.loadGrid(getFilterObject());
            };
            $scope.addOrgChart = function () {
                var onOrgChartAdded = function (addedOrgChart) {
                    gridAPI.onOrgChartAdded(addedOrgChart);
                };
                VR_Sec_OrgChartService.addOrgChart(onOrgChartAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            return {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('VR_Sec_OrgChartManagementController', OrgChartManagementController);

})(appControllers);
