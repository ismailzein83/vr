RoutingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZonePackageAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

function RoutingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZonePackageAPIService, UtilsService, VRNotificationService, VRNavigationService) {

    var editMode;
    var routingProductId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            routingProductId = parameters.routingProductId;
        }

        editMode = (routingProductId != undefined);
    }

    function defineScope() {
        $scope.SaveRoutingProduct = function () {
            if (editMode) {
                return updateRoutingProduct();
            }
            else {
                return insertRoutingProduct();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.saleZoneGroupTemplates = [];
        $scope.selectedSaleZoneGroupTemplate = undefined;

        $scope.suppliersGroupTemplates = [];
        $scope.selectedSupplierGroupTemplate = undefined;

        $scope.saleZonePackages = [];
        $scope.selectedSaleZonePackage = undefined;

        $scope.saleZoneGroups = {};
        $scope.suppliersGroups = {};
    }

    function load() {
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations([loadSaleZonePackages, loadSaleZoneGroupTemplates, loadSupplierGroupTemplates]).then(function () {
            if (editMode) {
                getRoutingProduct();
            }
            else {
                $scope.isGettingData = false;
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getRoutingProduct() {
        return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (routingProduct) {

            fillScopeFromRoutingProductObj(routingProduct);

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
    }

    function loadSaleZonePackages() {
        return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
            angular.foreach(response, function (item) {
                $scope.saleZonePackages.push(item);
            });
        });
    }

    function loadSaleZoneGroupTemplates() {
        return WhS_BE_RoutingProductAPIService.GetSaleZoneGroupTemplates().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.saleZoneGroupTemplates.push(item);
            });
        });
    }

    function loadSupplierGroupTemplates() {
        return WhS_BE_RoutingProductAPIService.GetSupplierGroupTemplates().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.suppliersGroupTemplates.push(item);
            });
        });
    }

    function buildRoutingProductObjFromScope() {

        var routingProduct = {
            RoutingProductId: (routingProductId != null) ? routingProductId : 0,
            Name: $scope.routingProductName,
            SaleZonePackageId: $scope.selectedSaleZonePackage.SaleZonePackageId,
            Settings: {
                SaleZoneGroupConfigId: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID,
                SaleZoneGroupSettings: $scope.saleZoneGroups.getData(),
                SuppliersGroupConfigId: $scope.selectedSupplierGroupTemplate.TemplateConfigID,
                SupplierGroupSettings: $scope.suppliersGroup.getData()
            }
        };

        return routingProduct;
    }

    function fillScopeFromRoutingProductObj(routingProductObj) {
        $scope.routingProductName = routingProductObj.Name;
        $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, routingProductObj.SaleZonePackageId, "SaleZonePackageId");

        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.SaleZoneGroupConfigId, "TemplateConfigID");
        $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.suppliersGroupTemplates, routingProductObj.SuppliersGroupConfigId, "TemplateConfigID");

        $scope.saleZoneGroups.data = routingProductObj.Settings.Zones;

        if ($scope.saleZoneGroups.loadTemplateData != undefined)
            $scope.saleZoneGroups.loadTemplateData();

        $scope.suppliersGroup.data = routingProductObj.Settings.Suppliers;

        if ($scope.suppliersGroup.loadTemplateData != undefined)
            $scope.suppliersGroup.loadTemplateData();
    }

    function insertRoutingProduct() {
        var routingProductObject = buildRoutingProductObjFromScope();
        return WhS_BE_RoutingProductAPIService.AddRoutingProduct(routingProductObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Routing Product", response)) {
                if ($scope.onRoutingProductAdded != undefined)
                    $scope.onRoutingProductAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateRoutingProduct() {
        var routingProductObject = buildRoutingProductObjFromScope();
        WhS_BE_RoutingProductAPIService.UpdateRoutingProduct(routingProductObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Routing Product", response)) {
                if ($scope.onRoutingProductUpdated != undefined)
                    $scope.onRoutingProductUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('WhS_BE_RoutingProductEditorController', RoutingProductEditorController);
