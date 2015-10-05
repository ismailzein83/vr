(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZonePackageAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZonePackageAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService) {

            var editMode;
            var routingProductId;

            var saleZoneGroupSettingsDirectiveAPI;
            var saleZoneGroupSettings;

            var supplierGroupSettingsDirectiveAPI;
            var supplierGroupSettings;

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

                $scope.onSaleZoneGroupSettingsDirectiveLoaded = function (api) {
                    saleZoneGroupSettingsDirectiveAPI = api;

                    if (saleZoneGroupSettings != undefined) {
                        saleZoneGroupSettingsDirectiveAPI.setData(saleZoneGroupSettings);
                        saleZoneGroupSettings = undefined;
                    }
                }

                $scope.onSupplierGroupSettingsDirectiveLoaded = function (api) {
                    supplierGroupSettingsDirectiveAPI = api;

                    if (supplierGroupSettings != undefined) {
                        supplierGroupSettingsDirectiveAPI.setData(supplierGroupSettings);
                        supplierGroupSettings = undefined;
                    }
                }

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

                $scope.saleZonePackages = [];
                $scope.selectedSaleZonePackage = undefined;

                $scope.saleZoneGroupTemplates = [];
                $scope.selectedSaleZoneGroupTemplate = undefined;

                $scope.supplierGroupTemplates = [];
                $scope.selectedSupplierGroupTemplate = undefined;

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

                    var defSaleZoneSelection = { TemplateConfigID: -1, Name: 'No Filter', Editor: '' };
                    $scope.saleZoneGroupTemplates.push(defSaleZoneSelection);

                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    $scope.selectedSaleZoneGroupTemplate = defSaleZoneSelection;
                });
            }

            function loadSupplierGroupTemplates() {
                return WhS_BE_CarrierAccountAPIService.GetSupplierGroupTemplates().then(function (response) {

                    var defSupplierSelection = { TemplateConfigID: -1, Name: 'No Filter', Editor: '' };
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
                        SaleZoneGroupSettings: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID != -1 ? saleZoneGroupSettingsDirectiveAPI.getData() : null,
                        SupplierGroupConfigId: $scope.selectedSupplierGroupTemplate.TemplateConfigID != -1 ? $scope.selectedSupplierGroupTemplate.TemplateConfigID : null,
                        SupplierGroupSettings: $scope.selectedSupplierGroupTemplate.TemplateConfigID != -1 ? supplierGroupSettingsDirectiveAPI.getData() : null
                    }
                };

                return routingProduct;
            }

            function fillScopeFromRoutingProductObj(routingProductObj) {
                $scope.routingProductName = routingProductObj.Name;
                $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, routingProductObj.SaleZonePackageId, "SaleZonePackageId");

                if (routingProductObj.Settings.SaleZoneGroupConfigId != null)
                {
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupConfigId, "TemplateConfigID");
                    saleZoneGroupSettings = routingProductObj.Settings.SaleZoneGroupSettings;
                }
                    
                if (routingProductObj.Settings.SupplierGroupConfigId != null)
                {
                    $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.supplierGroupTemplates, routingProductObj.Settings.SupplierGroupConfigId, "TemplateConfigID");
                    supplierGroupSettings = routingProductObj.Settings.SupplierGroupSettings;
                }
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
