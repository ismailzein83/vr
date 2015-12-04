(function (appControllers) {

    "use strict";

    RPRouteOptionSupplierController.$inject = ["$scope", "WhS_Routing_RPRouteAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RPRouteOptionSupplierController($scope, WhS_Routing_RPRouteAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var gridReadyDeferred = UtilsService.createPromiseDeferred();
        var routingDatabaseId;
        var routingProductId;
        var saleZoneId;
        var supplierId;

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
            }
        }

        function defineScope() {
            $scope.supplierZones = [];

            $scope.onGridReady = function (api) {
                gridReadyDeferred.resolve();
                
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            gridReadyDeferred.promise.then(function () {
                $scope.isLoading = true;

                WhS_Routing_RPRouteAPIService.GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId).then(function (response) {
                    if (response) {
                        $scope.title = "Supplier: " + response.SupplierName;

                        for (var i = 0; i < response.SupplierZones.length; i++) {
                            $scope.supplierZones.push(response.SupplierZones[i]);
                        }
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });
            });
        }
    }

    appControllers.controller("WhS_Sales_RPRouteOptionSupplierController", RPRouteOptionSupplierController);

})(appControllers);
