(function (appControllers) {

    "use strict";

    RatePlanChangesController.$inject = ["$scope", "WhS_Sales_RatePlanAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function RatePlanChangesController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var ownerType;
        var ownerId;
        var changes;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                ownerType = parameters.OwnerType;
                ownerId = parameters.OwnerId;
            }
        }

        function defineScope() {
            $scope.defaultItem;

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            var promises = [];

            var getChangesPromise = getChanges();
            promises.push(getChangesPromise);
            
            getChangesPromise.then(function () {
                if (defaultChangesExist()) {
                    var getDefaultItemPromise = getDefaultItem();
                    promises.push(getDefaultItemPromise);
                }
            });

            UtilsService.waitMultiplePromises(promises).then(function () {
                console.log($scope.defaultItem);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function getChanges() {
                return WhS_Sales_RatePlanAPIService.GetChanges(ownerType, ownerId).then(function (response) {
                    changes = response;

                    if (defaultChangesExist()) {
                        var newDefaultRoutingProduct = changes.DefaultChanges.NewDefaultRoutingProduct;
                        var defaultRoutingProductChange = changes.DefaultChanges.DefaultRoutingProductChange;

                        $scope.defaultItem.newRoutingProductName = newDefaultRoutingProduct ? newDefaultRoutingProduct.Name : defaultRoutingProductChange.Name;
                        $scope.defaultItem.newRoutingProductEED = newDefaultRoutingProduct ? newDefaultRoutingProduct.EED : defaultRoutingProductChange.EED;
                    }
                });
            }

            function getDefaultItem() {
                return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerType, ownerId).then(function (response) {
                    $scope.defaultItem.currentRoutingProductName = response.CurrentRoutingProductName;
                    $scope.defaultItem.currentRoutingProductEED = response.CurrentRoutingProductEED;
                });
            }

            function defaultChangesExist() {
                return (changes && changes.DefaultChanges && (changes.DefaultChanges.NewDefaultRoutingProduct || changes.DefaultChanges.DefaultRoutingProductChange))
            }
        }

        function setDefaultChanges() {
            var oldChanges = oldState ? oldState.DefaultChanges : null;
            var newChanges = newState ? newState.DefaultChanges : null;

            console.log(oldChanges);
            console.log(newChanges);

            if ((oldChanges && newChanges) || (!oldChanges && newChanges)) {
                var dataItem = {};

                if (oldChanges && oldChanges.NewDefaultRoutingProduct) {
                    var routingProduct = UtilsService.getItemByVal(routingProducts, oldChanges.NewDefaultRoutingProduct.DefaultRoutingProductId, "RoutingProductId");
                    dataItem.oldRPName = routingProduct ? routingProduct.Name : null;
                    dataItem.oldRPEED = oldChanges.NewDefaultRoutingProduct.EED;
                }

                if (newChanges && newChanges.NewDefaultRoutingProduct) {
                    var routingProduct = UtilsService.getItemByVal(routingProducts, newChanges.NewDefaultRoutingProduct.DefaultRoutingProductId, "RoutingProductId");
                    dataItem.newRPName = routingProduct ? routingProduct.Name : null;
                    dataItem.newRPEED = newChanges.NewDefaultRoutingProduct.EED;
                }

                $scope.defaultRoutingProductChanges.push(dataItem);
            }
        }

        function setZoneChanges() {
            var oldChanges = oldState ? oldState.ZoneChanges : null;
            var newChanges = newState ? newState.ZoneChanges : null;

            if (oldChanges && newChanges) {
                for (var i = 0; i < oldChanges.length; i++) {
                    var oldItem = oldChanges[i];
                    var newItem = UtilsService.getItemByVal(newChanges, oldItem.ZoneId, "ZoneId");
                    addDataItem(oldItem, newItem);
                }
            }
            else if (oldChanges) {
                for (var i = 0; i < oldChanges.length; i++) {
                    var oldItem = oldChanges[i];
                    addDataItem(oldItem, null);
                }
            }
            else if (newChanges) {
                for (var i = 0; i < newChanges.length; i++) {
                    var newItem = newChanges[i];
                    addDataItem(null, newItem);
                }
            }

            function addDataItem(oldItem, newItem) {
                var dataItem = {};

                if (oldItem) {
                    dataItem.name = oldItem.ZoneId;
                    if (oldItem.NewRate) {
                        dataItem.oldRate = oldItem.NewRate.NormalRate;
                        dataItem.oldRateEEd = oldItem.NewRate.EED;
                    }
                    if (oldItem.NewRoutingProduct) {
                        var oldRoutingProduct = UtilsService.getItemByVal(routingProducts, oldItem.NewRoutingProduct.ZoneRoutingProductId, "RoutingProductId");
                        dataItem.oldRPName = oldRoutingProduct ? oldRoutingProduct.Name : null;
                        dataItem.oldRPEED = oldItem.NewRoutingProduct.EED;
                    }
                }
                
                if (newItem) {
                    if (!dataItem.name)
                        dataItem.name = newItem.ZoneId;
                    if (newItem.NewRate) {
                        dataItem.newRate = newItem.NewRate.NormalRate;
                        dataItem.newRateEED = newItem.NewRate.EED;
                    }
                    if (newItem.NewRoutingProduct) {
                        var newRoutingProduct = UtilsService.getItemByVal(routingProducts, newItem.NewRoutingProduct.ZoneRoutingProductId, "RoutingProductId");
                        dataItem.newRPName = newRoutingProduct ? newRoutingProduct.Name : null;
                        dataItem.newRPEED = newItem.NewRoutingProduct.EED;
                    }
                }

                $scope.zoneChanges.push(dataItem);
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanChangesController", RatePlanChangesController);

})(appControllers);
