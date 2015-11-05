(function (appControllers) {

    "use strict";

    rateTypeManagementController.$inject = ['$scope', 'WhS_BE_MainService'];

    function rateTypeManagementController($scope, WhS_BE_MainService) {
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
                if (gridAPI != undefined) {
                    gridAPI.onRateTypeAdded(rateTypeObj);
                }


            };
            WhS_BE_MainService.addRateType(onRateTypeAdded);
        }

    }

    appControllers.controller('WhS_BE_RateTypeManagementController', rateTypeManagementController);
})(appControllers);