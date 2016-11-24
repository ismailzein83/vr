(function (appControllers) {

    "use strict";

    SellingNumberPlanManagementController.$inject = ['$scope', 'Vr_NP_SellingNumberPlanService', 'Vr_NP_SellingNumberPlanAPIService'];

    function SellingNumberPlanManagementController($scope, Vr_NP_SellingNumberPlanService, Vr_NP_SellingNumberPlanAPIService) {
        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.hasAddSellingNumberPlanPermission = function () {
                return Vr_NP_SellingNumberPlanAPIService.HasAddSellingNumberPlanPermission();
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
            Vr_NP_SellingNumberPlanService.addSellingNumberPlan(onSellingNumberPlanAdded);
        }

    }

    appControllers.controller('Vr_NP_SellingNumberPlanManagementController', SellingNumberPlanManagementController);
})(appControllers);