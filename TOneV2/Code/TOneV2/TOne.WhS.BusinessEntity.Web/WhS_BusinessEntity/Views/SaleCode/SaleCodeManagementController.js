(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {


        var gridAPI;
        var codeGroupDirectiveAPI;
        var codeGroupReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
            }
            $scope.onCodeGroupDirectiveReady = function (api) {
                codeGroupDirectiveAPI = api;
                codeGroupReadyPromiseDeferred.resolve();
            }

        }
        function load() {           
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCodeGroup])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }
      
        function loadCodeGroup() {
            var loadCodeGroupPromiseDeferred = UtilsService.createPromiseDeferred();
            codeGroupReadyPromiseDeferred.promise.then(function () {
                var payload = {};
                VRUIUtilsService.callDirectiveLoad(codeGroupDirectiveAPI, payload, loadCodeGroupPromiseDeferred);
            });
            return loadCodeGroupPromiseDeferred.promise;
        }
       
        function setFilterObject() {
            filter = {                
                EffectiveOn: $scope.effectiveOn
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);