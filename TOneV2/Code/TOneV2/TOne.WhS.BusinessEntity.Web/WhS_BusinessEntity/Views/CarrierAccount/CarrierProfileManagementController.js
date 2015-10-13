(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function carrierProfileManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }

            $scope.name;
            $scope.onCarrierProfileDirectiveReady = function (api) {
                carrierProfileDirectiveAPI = api;
                load();
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.AddNewCarrierProfile = AddNewCarrierProfile;
        }

        function load() {

            $scope.isGettingData = true;

            if (carrierProfileDirectiveAPI == undefined)
                return;

            carrierProfileDirectiveAPI.load().then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });

        }

        function getFilterObject() {
            var data = {
                CarrierProfileIds:  UtilsService.getPropValuesFromArray(carrierProfileDirectiveAPI.getData(), "CarrierProfileId"),
                Name: $scope.name,
            };
            return data;
        }

        function AddNewCarrierProfile() {
            var onCarrierProfileAdded = function (carrierProfileObj) {
                if (gridAPI != undefined)
                    gridAPI.onCarrierProfileAdded(carrierProfileObj);
            };
            WhS_BE_MainService.addCarrierProfile(onCarrierProfileAdded);
        }

    }

    appControllers.controller('WhS_BE_CarrierProfileManagementController', carrierProfileManagementController);
})(appControllers);