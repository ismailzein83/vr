(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZonePackageAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZonePackageAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService) {

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

                $scope.saleZonePackageOnSelectionChanged = function () {
                    if ($scope.selectedSaleZonePackage != undefined)
                        $scope.saleZoneGroups.saleZonePackageId = $scope.selectedSaleZonePackage.SaleZonePackageId;
                }

                $scope.saleZoneGroupTemplates = [];
                $scope.selectedSaleZoneGroupTemplate = undefined;

                $scope.supplierGroupTemplates = [];
                $scope.selectedSupplierGroupTemplate = undefined;

                $scope.saleZonePackages = [];
                $scope.selectedSaleZonePackage = undefined;

                $scope.saleZoneGroups = {};
                $scope.supplierGroups = {};
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
                    angular.forEach(response, function (item) {
                        $scope.saleZonePackages.push(item);
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

            function loadSupplierGroupTemplates() {
                return WhS_BE_CarrierAccountAPIService.GetSupplierGroupTemplates().then(function (response) {

                    var defSupplierSelection = { TemplateConfigID: -1, Name: 'All Suppliers', TemplateURL: '' };
                    $scope.supplierGroupTemplates.push(defSupplierSelection);

                    angular.forEach(response, function (item) {
                        $scope.supplierGroupTemplates.push(item);
                    });

                    $scope.selectedSupplierGroupTemplate = defSupplierSelection;
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

            function fillScopeFromRoutingProductObj(routingProductObj) {
                $scope.routingProductName = routingProductObj.Name;
                $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, routingProductObj.SaleZonePackageId, "SaleZonePackageId");

                if (routingProductObj.Settings.SaleZoneGroupConfigId != null)
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupConfigId, "TemplateConfigID");

                if (routingProductObj.Settings.SupplierGroupConfigId != null)
                    $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.supplierGroupTemplates, routingProductObj.Settings.SupplierGroupConfigId, "TemplateConfigID");

                $scope.saleZoneGroups.saleZonePackageId = routingProductObj.SaleZonePackageId;
                $scope.saleZoneGroups.data = routingProductObj.Settings.SaleZoneGroupSettings;

                if ($scope.saleZoneGroups.loadTemplateData != undefined)
                    $scope.saleZoneGroups.loadTemplateData();

                $scope.supplierGroups.data = routingProductObj.Settings.SupplierGroupSettings;

                if ($scope.supplierGroups.loadTemplateData != undefined)
                    $scope.supplierGroups.loadTemplateData();

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

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
