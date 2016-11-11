(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {


        var gridAPI;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = Date.now();
            $scope.searchClicked = function () {
                var queryHandler = {
                    $type: "TOne.WhS.BusinessEntity.Business.SaleCodeQueryHandler, TOne.WhS.BusinessEntity.Business"
                };

                queryHandler.Query = {
                    SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                    ZonesIds: saleZoneDirectiveAPI.getSelectedIds(),
                    Code: $scope.code,
                    EffectiveOn: $scope.effectiveOn
                };

                var payload = {
                    queryHandler: queryHandler
                };

                return gridAPI.loadGrid(payload);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;

            };
            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onSellingNumberPlanSelectItem = function (selectedItem) {
                if (selectedItem != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value; };

                    var payload = {
                        sellingNumberPlanId: selectedItem.SellingNumberPlanId
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                }

            };
        }
        function load() {           
            $scope.isLoadingFilter = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlan])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
        function loadSellingNumberPlan() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_BE_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);