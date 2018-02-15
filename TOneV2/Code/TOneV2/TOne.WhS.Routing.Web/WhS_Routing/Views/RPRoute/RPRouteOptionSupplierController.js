(function (appControllers) {

    "use strict";

    RPRouteOptionSupplierController.$inject = ["$scope", "WhS_Routing_RPRouteAPIService", "WhS_Routing_RouteOptionRuleService", "WhS_BE_CarrierAccountAPIService", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function RPRouteOptionSupplierController($scope, WhS_Routing_RPRouteAPIService, WhS_Routing_RouteOptionRuleService, WhS_BE_CarrierAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var routingProductId;
        var saleZoneId;
        var supplierId;
        var routingDatabaseId;
        var currencyId;
        var supplierName;
        var saleRate;

        var rpRouteOptionGridAPI;
        var rpRouteOptionGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                routingDatabaseId = parameters.RoutingDatabaseId;
                routingProductId = parameters.RoutingProductId;
                saleZoneId = parameters.SaleZoneId;
                supplierId = parameters.SupplierId;
                currencyId = parameters.CurrencyId;
                saleRate = parameters.SaleRate;
            }
        }
        function defineScope() {
            $scope.supplierZones = [];

            $scope.onGridReady = function (api) {
                rpRouteOptionGridAPI = api;
                rpRouteOptionGridReadyDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;

            getSupplierName().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }

        function getSupplierName() {
            return WhS_BE_CarrierAccountAPIService.GetCarrierAccountName(supplierId).then(function (response) {
                if (response != null) {
                    supplierName = response;
                }
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = "Supplier: " + supplierName;
        }
        function loadGrid() {
            var loadRpRouteOptionGridPromiseDeferred = UtilsService.createPromiseDeferred();

            rpRouteOptionGridReadyDeferred.promise.then(function () {

                var payload = {
                    routingProductId: routingProductId,
                    saleZoneId: saleZoneId,
                    supplierId: supplierId,
                    routingDatabaseId: routingDatabaseId,
                    currencyId: currencyId,
                    saleRate: saleRate
                };

                VRUIUtilsService.callDirectiveLoad(rpRouteOptionGridAPI, payload, loadRpRouteOptionGridPromiseDeferred);
            });

            return loadRpRouteOptionGridPromiseDeferred.promise;
        }

        //function extendSupplierZoneObject(supplierZone) {
        //    supplierZone.supplierZoneLoadDeferred = UtilsService.createPromiseDeferred();
        //    supplierZone.onServiceViewerReady = function (api) {
        //        supplierZone.serviceViewerAPI = api;
        //        var serviceViewerPayload;
        //        if (supplierZone.Entity != undefined) {
        //            serviceViewerPayload = {
        //                selectedIds: supplierZone.Entity.ExactSupplierServiceIds
        //            };
        //        }
        //        VRUIUtilsService.callDirectiveLoad(supplierZone.serviceViewerAPI, serviceViewerPayload, supplierZone.supplierZoneLoadDeferred);
        //    };
        //}
    }

    appControllers.controller("WhS_Routing_RPRouteOptionSupplierController", RPRouteOptionSupplierController);

})(appControllers);
