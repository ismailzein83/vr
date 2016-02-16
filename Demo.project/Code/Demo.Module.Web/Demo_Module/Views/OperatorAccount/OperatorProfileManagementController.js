(function (appControllers) {

    "use strict";

    operatorProfileManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_OperatorProfileService'];

    function operatorProfileManagementController($scope,  UtilsService, VRNotificationService, VRUIUtilsService, Demo_OperatorProfileService) {
        var gridAPI;
        var operatorProfileDirectiveAPI;
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

            $scope.AddNewOperatorProfile = AddNewOperatorProfile;

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

        function AddNewOperatorProfile() {
            var onOperatorProfileAdded = function (operatorProfileObj) {
                gridAPI.onOperatorProfileAdded(operatorProfileObj);
            };

            Demo_OperatorProfileService.addOperatorProfile(onOperatorProfileAdded);
        }

    }

    appControllers.controller('Demo_OperatorProfileManagementController', operatorProfileManagementController);
})(appControllers);