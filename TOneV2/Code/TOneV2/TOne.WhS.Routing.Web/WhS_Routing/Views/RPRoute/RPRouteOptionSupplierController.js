(function (appControllers) {

    "use strict";

    RPRouteOptionSupplierController.$inject = ["$scope", "WhS_Routing_RPRouteAPIService", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function RPRouteOptionSupplierController($scope, WhS_Routing_RPRouteAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var routingProductId;
        var saleZoneId;
        var supplierId;
        var routingDatabaseId;
        var currencyId;


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
            }
        }
        function defineScope() {
            $scope.supplierZones = [];

            $scope.onGridReady = function (api) {
                $scope.isLoading = true;

                WhS_Routing_RPRouteAPIService.GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, currencyId).then(function (response) {
                    if (response) {
                        var _supplierZoneServiceViewerPromises = [];

                        $scope.title = "Supplier: " + response.SupplierName;

                        for (var i = 0; i < response.SupplierZones.length; i++) {
                            var supplierZone = response.SupplierZones[i];
                            $scope.supplierZones.push(supplierZone);
                            extendSupplierZoneObject(supplierZone);
                            _supplierZoneServiceViewerPromises.push(supplierZone.supplierZoneLoadDeferred.promise);
                        }

                        UtilsService.waitMultiplePromises(_supplierZoneServiceViewerPromises).then(function () {
                            $scope.isLoading = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        })
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {

        }

        function extendSupplierZoneObject(supplierZone) {
            supplierZone.supplierZoneLoadDeferred = UtilsService.createPromiseDeferred();
            supplierZone.onServiceViewerReady = function (api) {
                supplierZone.serviceViewerAPI = api;

                var serviceViewerPayload;
                if (supplierZone.Entity != undefined) {
                    serviceViewerPayload = {
                        selectedIds: supplierZone.Entity.ExactSupplierServiceIds
                    };
                }

                VRUIUtilsService.callDirectiveLoad(supplierZone.serviceViewerAPI, serviceViewerPayload, supplierZone.supplierZoneLoadDeferred);
            };
        }
    }

    appControllers.controller("WhS_Routing_RPRouteOptionSupplierController", RPRouteOptionSupplierController);

})(appControllers);
