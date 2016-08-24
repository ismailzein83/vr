(function (appControllers) {

    "use strict";

    RPRouteOptionSupplierController.$inject = ["$scope", "WhS_Routing_RPRouteAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RPRouteOptionSupplierController($scope, WhS_Routing_RPRouteAPIService, UtilsService, VRNavigationService, VRNotificationService) {

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
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

        }
    }

    appControllers.controller("WhS_Routing_RPRouteOptionSupplierController", RPRouteOptionSupplierController);

})(appControllers);
