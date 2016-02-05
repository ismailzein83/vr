(function (appControllers) {

    "use strict";

    rateTypeManagementController.$inject = ['$scope', 'VRCommon_RateTypeService'];

    function rateTypeManagementController($scope, VRCommon_RateTypeService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject()
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {

                gridAPI = api;
                api.loadGrid(filter);
            }
            $scope.addNewRateType = addNewRateType;
        }

        function load() {



        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
            };

        }

        function addNewRateType() {
            var onRateTypeAdded = function (rateTypeObj) {
                gridAPI.onRateTypeAdded(rateTypeObj);
            };
            VRCommon_RateTypeService.addRateType(onRateTypeAdded);
        }

    }

    appControllers.controller('VRCommon_RateTypeManagementController', rateTypeManagementController);

})(appControllers);