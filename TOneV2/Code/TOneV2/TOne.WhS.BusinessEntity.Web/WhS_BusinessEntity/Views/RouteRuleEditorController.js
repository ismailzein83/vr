RouteRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

function RouteRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService, UtilsService, VRNotificationService, VRNavigationService) {

    var editMode;
    var routeRuleId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            routeRuleId = parameters.routeRuleId;
        }

        editMode = (routeRuleId != undefined);
    }

    function defineScope() {
        $scope.SaveRouteRule = function () {
            if (editMode) {
                return updateRouteRule();
            }
            else {
                return insertRouteRule();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.routingProductsOnSelectionChanged = function () {
            if ($scope.selectedSaleZonePackage != undefined)
            {
                //Hide the section of customer and code
            }
        }

        $scope.saleZoneGroupTemplates = [];
        $scope.selectedSaleZoneGroupTemplate = undefined;

        $scope.routingProducts = [];
        $scope.selectedRoutingProduct = undefined;

        $scope.saleZoneGroups = {};
    }

    function load() {
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations([loadRoutingProducts, loadSaleZoneGroupTemplates]).then(function () {
            if (editMode) {
                getRouteRule();
            }
            else {
                $scope.isGettingData = false;
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getRouteRule() {
        return WhS_BE_RouteRuleAPIService.GetRouteRule(routeRuleId).then(function (routeRule) {

            fillScopeFromRouteRuleObj(routeRule);

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
    }

    function loadRoutingProducts() {
        return WhS_BE_RoutingProductAPIService.GetRoutingProducts().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.RoutingProducts.push(item);
            });
        });
    }

    function loadSaleZoneGroupTemplates() {
        return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {

            var defSaleZoneSelection = { TemplateConfigID: -1, Name: 'All Sale Zones', TemplateURL: '' };
            $scope.saleZoneGroupTemplates.push(defSaleZoneSelection);

            angular.forEach(response, function (item) {
                $scope.saleZoneGroupTemplates.push(item);
            });

            $scope.selectedSaleZoneGroupTemplate = defSaleZoneSelection;
        });
    }

    function buildRoutingProductObjFromScope() {

        var routingProduct = {
            RoutingProductId: (routingProductId != null) ? routingProductId : 0,
            Name: $scope.routingProductName,
            SaleZonePackageId: $scope.selectedSaleZonePackage.SaleZonePackageId,
            Settings: {
                SaleZoneGroupConfigId: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID != -1 ? $scope.selectedSaleZoneGroupTemplate.TemplateConfigID : null,
                SaleZoneGroupSettings: $scope.saleZoneGroups.getData != undefined ? $scope.saleZoneGroups.getData() : null,
                SupplierGroupConfigId: $scope.selectedSupplierGroupTemplate.TemplateConfigID != -1 ? $scope.selectedSupplierGroupTemplate.TemplateConfigID : null,
                SupplierGroupSettings: $scope.supplierGroups.getData != undefined ? $scope.supplierGroups.getData() : null
            }
        };

        return routingProduct;
    }

    function fillScopeFromRoutingProductObj(routeRuleObj) {
        //$scope.routingProductName = routingProductObj.Name;
        //$scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, routingProductObj.SaleZonePackageId, "SaleZonePackageId");

        //if (routingProductObj.Settings.SaleZoneGroupConfigId != null)
        //    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupConfigId, "TemplateConfigID");

        //if (routingProductObj.Settings.SupplierGroupConfigId != null)
        //    $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.supplierGroupTemplates, routingProductObj.Settings.SupplierGroupConfigId, "TemplateConfigID");

        //$scope.saleZoneGroups.saleZonePackageId = routingProductObj.SaleZonePackageId;
        //$scope.saleZoneGroups.data = routingProductObj.Settings.SaleZoneGroupSettings;

        //if ($scope.saleZoneGroups.loadTemplateData != undefined)
        //    $scope.saleZoneGroups.loadTemplateData();

        //$scope.supplierGroups.data = routingProductObj.Settings.SupplierGroupSettings;

        //if ($scope.supplierGroups.loadTemplateData != undefined)
        //    $scope.supplierGroups.loadTemplateData();

    }

    function insertRoutingProduct() {
        //var routingProductObject = buildRoutingProductObjFromScope();
        //return WhS_BE_RoutingProductAPIService.AddRoutingProduct(routingProductObject)
        //.then(function (response) {
        //    if (VRNotificationService.notifyOnItemAdded("Routing Product", response)) {
        //        if ($scope.onRoutingProductAdded != undefined)
        //            $scope.onRoutingProductAdded(response.InsertedObject);
        //        $scope.modalContext.closeModal();
        //    }
        //}).catch(function (error) {
        //    VRNotificationService.notifyException(error, $scope);
        //});

    }

    function updateRoutingProduct() {
        //var routingProductObject = buildRoutingProductObjFromScope();
        //WhS_BE_RoutingProductAPIService.UpdateRoutingProduct(routingProductObject)
        //.then(function (response) {
        //    if (VRNotificationService.notifyOnItemUpdated("Routing Product", response)) {
        //        if ($scope.onRoutingProductUpdated != undefined)
        //            $scope.onRoutingProductUpdated(response.UpdatedObject);
        //        $scope.modalContext.closeModal();
        //    }
        //}).catch(function (error) {
        //    VRNotificationService.notifyException(error, $scope);
        //});
    }
}
appControllers.controller('WhS_BE_RouteRuleEditorController', RouteRuleEditorController);
