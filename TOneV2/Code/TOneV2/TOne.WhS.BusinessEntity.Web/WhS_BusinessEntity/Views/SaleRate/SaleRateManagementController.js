(function (appControllers) {

    "use strict";

    saleRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function saleRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {


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
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            $scope.onSellingNumberPlanSelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                    var payload = {
                        sellingNumberPlanId: selectedItem.SellingNumberPlanId,
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                }
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;            
               
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
                var payload = {};
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, payload, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
        function setFilterObject() {
            filter = {                
                EffectiveOn: $scope.effectiveOn
            };
           
        }

    }

    appControllers.controller('WhS_BE_SaleRateManagementController', saleRateManagementController);
})(appControllers);