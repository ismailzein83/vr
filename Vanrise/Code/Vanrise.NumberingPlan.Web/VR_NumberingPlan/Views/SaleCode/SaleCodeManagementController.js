(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','VRDateTimeService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {


        var gridAPI;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();
            $scope.searchClicked = function () {
                var queryHandler = {
                    $type: "Vanrise.NumberingPlan.Business.SaleCodeQueryHandler, Vanrise.NumberingPlan.Business"
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

            $scope.onSellingNumberPlanSelectItem = function () {
                if (sellingNumberPlanDirectiveAPI.getSelectedIds() != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value; };

                    var payload = {
                        sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
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
                VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, { selectifsingleitem: true }, loadSellingNumberPlanPromiseDeferred);
            });

            return loadSellingNumberPlanPromiseDeferred.promise;
        }
    }

    appControllers.controller('Vr_NP_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);