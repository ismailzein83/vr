(function (appControllers) {

    "use strict";

    carrierProfileManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_CarrierProfileService'];

    function carrierProfileManagementController($scope,  UtilsService, VRNotificationService, VRUIUtilsService, Demo_CarrierProfileService) {
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

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewCarrierProfile = AddNewCarrierProfile;

            function getFilterObject() {
                var data = {
                    Name: $scope.name,
                    CountriesIds: countryDirectiveApi.getSelectedIds(),
                    Company: $scope.company
                };

                return data;
            }
        }

        function load() {
            $scope.isLoadingFilters = true;
             loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountries])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadCountries() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, undefined, loadCountryPromiseDeferred);
            });

            return loadCountryPromiseDeferred.promise;
        }

        function AddNewCarrierProfile() {
            var onCarrierProfileAdded = function (carrierProfileObj) {
                gridAPI.onCarrierProfileAdded(carrierProfileObj);
            };

            Demo_CarrierProfileService.addCarrierProfile(onCarrierProfileAdded);
        }

    }

    appControllers.controller('Demo_CarrierProfileManagementController', carrierProfileManagementController);
})(appControllers);