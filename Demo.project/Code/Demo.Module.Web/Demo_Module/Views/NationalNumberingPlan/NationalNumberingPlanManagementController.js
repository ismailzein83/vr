(function (appControllers) {

    "use strict";

    nationalNumberingPlanManagementController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_NationalNumberingPlanService'];

    function nationalNumberingPlanManagementController($scope,  UtilsService, VRNotificationService, VRUIUtilsService, Demo_NationalNumberingPlanService) {
        var gridAPI;
        var nationalNumberingPlanDirectiveAPI;
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

            $scope.AddNewNationalNumberingPlan = AddNewNationalNumberingPlan;

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

        function AddNewNationalNumberingPlan() {
            var onNationalNumberingPlanAdded = function (nationalNumberingPlanObj) {
                gridAPI.onNationalNumberingPlanAdded(nationalNumberingPlanObj);
            };

            Demo_NationalNumberingPlanService.addNationalNumberingPlan(onNationalNumberingPlanAdded);
        }

    }

    appControllers.controller('Demo_NationalNumberingPlanManagementController', nationalNumberingPlanManagementController);
})(appControllers);