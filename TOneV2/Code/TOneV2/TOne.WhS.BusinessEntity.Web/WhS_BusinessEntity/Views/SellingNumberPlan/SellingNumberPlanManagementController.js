(function (appControllers) {

    "use strict";

    SellingNumberPlanManagementController.$inject = ['$scope', 'WhS_BE_SellingNumberPlanService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SellingNumberPlanManagementController($scope, WhS_BE_SellingNumberPlanService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};

        var lobSelectorAPI;
        var lobSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onLOBSelectorReady = function (api) {
                lobSelectorAPI = api;
                lobSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(filter);
            };

            $scope.addNewSellingNumberPlan = addNewSellingNumberPlan;
        }

        function load() {
            $scope.isGettingData = true;
            loadLOBSelector().finally(function () {
                $scope.isGettingData = false;
            });
        }
        function setFilterObject() {
            filter = {
                Name: $scope.name,
                LOBIds: lobSelectorAPI.getSelectedIds()
            };
        }

        function addNewSellingNumberPlan() {
            var onSellingNumberPlanAdded = function (sellingNumberPlanObj) {
                if (gridAPI != undefined)
                    gridAPI.onSellingNumberPlanAdded(sellingNumberPlanObj);
            };
            WhS_BE_SellingNumberPlanService.addSellingNumberPlan(onSellingNumberPlanAdded);
        }

        function loadLOBSelector() {
            var lobSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            lobSelectorReadyDeferred.promise.then(function () {
                var lobSelectorPayload;

                VRUIUtilsService.callDirectiveLoad(lobSelectorAPI, lobSelectorPayload, lobSelectorLoadPromiseDeferred);
            });
            return lobSelectorLoadPromiseDeferred.promise;
        }

    }

    appControllers.controller('WhS_BE_SellingNumberPlanManagementController', SellingNumberPlanManagementController);
})(appControllers);