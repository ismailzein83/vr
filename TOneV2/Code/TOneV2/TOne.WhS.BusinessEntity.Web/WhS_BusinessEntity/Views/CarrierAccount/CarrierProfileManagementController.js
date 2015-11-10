(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService'];

    function carrierProfileManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        var countryDirectiveApi;
        defineScope();
        load();

        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                api.load();
            }

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                   return gridAPI.loadGrid(getFilterObject());
            };

            $scope.name;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.AddNewCarrierProfile = AddNewCarrierProfile;
        }

        function load() {
        }

        function getFilterObject() {
            var data = {
                Name: $scope.name,
                CountriesIds: countryDirectiveApi.getIdsData(),
                Company: $scope.company,
                BillingEmail: $scope.billingEmail
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