(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_CarrierProfileService'];

    function carrierProfileManagementController($scope,  UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_CarrierProfileService) {
        var gridAPI;
        var carrierProfileDirectiveAPI;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();

        function defineScope() {

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
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
            $scope.isLoading = true;
             loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountries])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                var payload = {  };
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }
        function getFilterObject() {
            var data = {
                Name: $scope.name,
                CountriesIds: countryDirectiveApi.getSelectedIds(),
                Company: $scope.company
            };
            
            return data;
        }

        function AddNewCarrierProfile() {
            var onCarrierProfileAdded = function (carrierProfileObj) {
                if (gridAPI != undefined)
                    gridAPI.onCarrierProfileAdded(carrierProfileObj);
            };
            WhS_BE_CarrierProfileService.addCarrierProfile(onCarrierProfileAdded);
        }

    }

    appControllers.controller('WhS_BE_CarrierProfileManagementController', carrierProfileManagementController);
})(appControllers);