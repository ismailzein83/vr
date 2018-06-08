(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRDateTimeService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;


        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            $scope.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };

            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
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

                var payload = { queryHandler: queryHandler };
                return gridAPI.loadGrid(payload);
            };
        }
        function load() {
            $scope.isLoadingFilter = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
        function loadSellingNumberPlanSelector() {
            var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                var sellingNumberPlanPayload = { selectifsingleitem: true };
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_BE_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);