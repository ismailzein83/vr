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
                $scope.supplierGroupTemplates = [];
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
                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });
                });
            }

            function loadSupplierGroupTemplates() {
                return WhS_BE_CarrierAccountAPIService.GetSupplierGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.supplierGroupTemplates.push(item);
                    });
                });
            }

            function buildRoutingProductObjFromScope() {

                var routingProduct = {
                    RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                    Name: $scope.routingProductName,
                    SaleZonePackageId: $scope.selectedSaleZonePackage.SaleZonePackageId,
                    Settings: {
                        SaleZoneGroupSettings: getSaleZoneGroupSettings(),
                        SupplierGroupSettings: getSuppliersGroupSettings()
                    }
                };

                return routingProduct;
            }

            function fillScopeFromRoutingProductObj(routingProductObj) {
                $scope.routingProductName = routingProductObj.Name;
                $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, routingProductObj.SaleZonePackageId, "SaleZonePackageId");

                if (routingProductObj.Settings != null)
                {
                    if (routingProductObj.Settings.SaleZoneGroupSettings != null && routingProductObj.Settings.SaleZoneGroupSettings.ConfigId != null) {
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupSettings.ConfigId, "TemplateConfigID");
                        saleZoneGroupSettings = routingProductObj.Settings.SaleZoneGroupSettings;
                    }

                    if (routingProductObj.Settings != null && routingProductObj.Settings.SupplierGroupSettings != null && routingProductObj.Settings.SupplierGroupSettings.ConfigId != null) {
                        $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.supplierGroupTemplates, routingProductObj.Settings.SupplierGroupSettings.ConfigId, "TemplateConfigID");
                        supplierGroupSettings = routingProductObj.Settings.SupplierGroupSettings;
                    }
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

            function getSaleZoneGroupSettings() {
                if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                    var settings = saleZoneGroupSettingsDirectiveAPI.getData();
                    settings.ConfigId = $scope.selectedSaleZoneGroupTemplate.TemplateConfigID;
                    return settings;
                }
                else
                    return null;
            }

            function getSuppliersGroupSettings() {
                if ($scope.selectedSupplierGroupTemplate != undefined) {
                    var settings = supplierGroupSettingsDirectiveAPI.getData();
                    settings.ConfigId = $scope.selectedSupplierGroupTemplate.TemplateConfigID;
                    return settings;
                }
                else
                    return null;
            }
    }

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
