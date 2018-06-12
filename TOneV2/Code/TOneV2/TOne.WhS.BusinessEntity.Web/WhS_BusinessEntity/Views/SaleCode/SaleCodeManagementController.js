(function (appControllers) {

    "use strict";

    saleCodeManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRDateTimeService'];

    function saleCodeManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService) {

        var saleZoneSelectorAPI;
        var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;


        defineScope();
        load();

        function defineScope() {
            $scope.effectiveOn = VRDateTimeService.getNowDateTime();

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneSelectorAPI = api;
                saleZoneSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.searchClicked = function () {

                var queryHandler = {
                    $type: "TOne.WhS.BusinessEntity.Business.SaleCodeQueryHandler, TOne.WhS.BusinessEntity.Business"
                };

                queryHandler.Query = {
                    SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                    ZonesIds: saleZoneSelectorAPI.getSelectedIds(),
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
            return UtilsService.waitMultipleAsyncOperations([loadSaleZoneSelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
              .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
        function loadSaleZoneSelector() {
            var loadSaleZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, undefined, loadSaleZoneSelectorPromiseDeferred);
            });

            return loadSaleZoneSelectorPromiseDeferred.promise;
        }
    }

    appControllers.controller('WhS_BE_SaleCodeManagementController', saleCodeManagementController);
})(appControllers);