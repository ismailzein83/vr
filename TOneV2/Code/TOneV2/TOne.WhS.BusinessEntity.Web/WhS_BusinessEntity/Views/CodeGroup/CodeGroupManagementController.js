(function (appControllers) {

    "use strict";

    codeGroupManagementController.$inject = ['$scope', 'WhS_BE_MainService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function codeGroupManagementController($scope, WhS_BE_MainService, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        var filter = {};
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined) {
                    setFilterObject();
                    return gridAPI.loadGrid(filter);
                }
                    
            };
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
                api.loadGrid(filter);
            }

            $scope.addNewCodeGroup = addNewCodeGroup;
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
                Code: $scope.code,
                CountriesIds: countryDirectiveApi.getSelectedIds()
            };
        }

        function addNewCodeGroup() {
            var onCodeGroupAdded = function (codeGroupObj) {
                if (gridAPI != undefined)
                    gridAPI.onCodeGroupAdded(codeGroupObj);
            };
            WhS_BE_MainService.addCodeGroup(onCodeGroupAdded);
        }

    }

    appControllers.controller('WhS_BE_CodeGroupManagementController', codeGroupManagementController);
})(appControllers);