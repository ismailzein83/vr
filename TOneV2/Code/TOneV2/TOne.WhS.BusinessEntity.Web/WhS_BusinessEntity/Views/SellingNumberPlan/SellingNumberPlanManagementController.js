(function (appControllers) {

    "use strict";

    SellingNumberPlanManagementController.$inject = ['$scope', 'WhS_BE_SellingNumberPlanService', 'WhS_BE_SellingNumberPlanAPIService'];

    function SellingNumberPlanManagementController($scope, WhS_BE_SellingNumberPlanService, WhS_BE_SellingNumberPlanAPIService) {
        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.hasAddSellingNumberPlanPermission = function () {
                return WhS_BE_SellingNumberPlanAPIService.HasAddSellingNumberPlanPermission();
            };

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };

            $scope.addNewSellingNumberPlan = addNewSellingNumberPlan;
        }

        function load() {
            //$scope.isGettingData = true;

        }
        function setFilterObject() {
            filter = {
                Name: $scope.name
            };
        }

        function addNewSellingNumberPlan() {
            var onSellingNumberPlanAdded = function (sellingNumberPlanObj) {
                if (gridAPI != undefined)
                    gridAPI.onSellingNumberPlanAdded(sellingNumberPlanObj);
            };
            WhS_BE_SellingNumberPlanService.addSellingNumberPlan(onSellingNumberPlanAdded);
        }

    }

    appControllers.controller('WhS_BE_SellingNumberPlanManagementController', SellingNumberPlanManagementController);
})(appControllers);