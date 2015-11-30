(function (appControllers) {

    "use strict";

    RPRouteOptionSupplierController.$inject = ["$scope", "WhS_Routing_RPRouteAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RPRouteOptionSupplierController($scope, WhS_Routing_RPRouteAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var routingProductId;
        var saleZoneId;
        var supplierId;
        var supplierName;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                routingProductId = parameters.RoutingProductId;
                saleZoneId = parameters.SaleZoneId;
                supplierId = parameters.SupplierId;
                //supplierName = parameters.SupplierName;
            }
        }

        function defineScope() {
            $scope.supplierZones = [];

            $scope.onGridReady = function (api) {
                $scope.isLoading = true;

                WhS_Routing_RPRouteAPIService.GetRPRouteOptionSupplier(routingProductId, saleZoneId, supplierId).then(function (response) {
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

    appControllers.controller("WhS_Sales_RPRouteOptionSupplierController", RPRouteOptionSupplierController);

})(appControllers);
