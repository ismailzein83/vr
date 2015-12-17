(function (appControllers) {

    "use strict";

    zoneManagementController.$inject = ['$scope' ,'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function zoneManagementController($scope,UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
         
            $scope.onCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
            }
          
        }
        function load() {
            $scope.isGettingData = true;
            loadAllControls();

        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountrySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function setFilterObject() {
            filter = {
                Name: $scope.name,
                EffectiveOn: $scope.effectiveOn,
                Countries: countryDirectiveApi.getSelectedIds()
            };
           
        }

    }

    appControllers.controller('QM_BE_ZoneManagementController', zoneManagementController);
})(appControllers);