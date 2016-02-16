(function (appControllers) {

    "use strict";

    codeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function codeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {


        var gridAPI;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
           
            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }
        }
        function load() {           
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlan])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoading = false;
             });
        }
        function loadSellingNumberPlan() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        
       
        function setFilterObject() {
            filter = {
                ZonesIds: saleZoneDirectiveAPI.getSelectedIds(),
                EffectiveOn: $scope.effectiveOn
            };
           
        }

    }

    appControllers.controller('CodeManagementController', codeManagementController);
})(appControllers);