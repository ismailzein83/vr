(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'WhS_BE_MainService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function carrierProfileManagementController($scope, WhS_BE_CarrierProfileAPIService, WhS_BE_MainService, UtilsService, VRModalService, VRNotificationService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (carrierProfileDirectiveAPI != undefined && gridAPI != undefined)
                    gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {
                }
                api.loadGrid(filter);
            }
            $scope.name;
            $scope.selectedProfile;
            $scope.onCarrierProfileDirectiveLoaded = function (api) {
                carrierProfileDirectiveAPI = api;

            }
            $scope.onCarrierProfileSelectionChanged = function () {
                if (carrierProfileDirectiveAPI != undefined)
                    $scope.selectedProfile = carrierProfileDirectiveAPI.getData();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }
            $scope.AddNewCarrierProfile = AddNewCarrierProfile;
        }

        function load() {
        }

        function getFilterObject() {
            var selectedProfile;
            if ($scope.selectedProfile != undefined)
                selectedProfile = $scope.selectedProfile.CarrierProfileId;
            var data = {
                CarrierProfileId: selectedProfile,
                Name: $scope.name,

            };
            return data;
        }
        function AddNewCarrierProfile() {
            var onCarrierProfileAdded = function (carrierProfileObj) {
                //if (gridAPI != undefined)
                //    gridAPI.onCustomerPricingProductAdded(customerPricingProductObj);
            };

            WhS_BE_MainService.addCarrierProfile(onCarrierProfileAdded);
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileManagementController', carrierProfileManagementController);
})(appControllers);