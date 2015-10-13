(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZonePackageAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZonePackageAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService) {

            var editMode;
            var routingProductId;

            var appendixData;

            var saleZonePackageDirectiveAPI;
            var saleZoneGroupSettingsDirectiveAPI;
            var supplierGroupSettingsDirectiveAPI;

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

                $scope.onSaleZonePackagesDirectiveReady = function (api) {
                    saleZonePackageDirectiveAPI = api;
                    load();
                }

                $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                    saleZoneGroupSettingsDirectiveAPI = api;

                    if (appendixData != undefined) {
                        loadAppendixDirectives();
                    }
                    else if (saleZoneGroupSettingsDirectiveAPI.load() != undefined) {
                        $scope.saleZonesAppendixLoader = true;
                        saleZoneGroupSettingsDirectiveAPI.load().catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.saleZonesAppendixLoader = false;
                        });

                    }
                }

                $scope.onSupplierGroupSettingsDirectiveReady = function (api) {
                    supplierGroupSettingsDirectiveAPI = api;

                    if (appendixData != undefined)
                    {
                        loadAppendixDirectives();
                    }
                    else if (supplierGroupSettingsDirectiveAPI.load() != undefined) {
                        $scope.suppliersAppendixLoader = true;
                        supplierGroupSettingsDirectiveAPI.load().catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.suppliersAppendixLoader = false;
                        });
                    }
                }

                $scope.onSaleZonePackageSelectionChanged = function () {
                    if (saleZonePackageDirectiveAPI != undefined && saleZonePackageDirectiveAPI.getData() != undefined)
                        $scope.selectedSaleZonePackageId = saleZonePackageDirectiveAPI.getData().SaleZonePackageId;
                    else
                        $scope.selectedSaleZonePackageId = undefined;
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

                $scope.saleZoneGroupTemplates = [];
                $scope.supplierGroupTemplates = [];
            }

            function load() {
                $scope.isGettingData = true;

                if (saleZonePackageDirectiveAPI == undefined)
                    return;

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
                    appendixData = routingProduct;
                    loadAppendixDirectives();

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });
            }

            function loadSaleZonePackages() {
                return saleZonePackageDirectiveAPI.load();
            }

            function loadSaleZoneGroupTemplates() {
                return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });
                });
            }

            function loadSaleZoneGroupSettings()
            {
                if ($scope.selectedSaleZoneGroupTemplate != undefined)
                    return saleZoneGroupSettingsDirectiveAPI.load();
            }

            function loadSuplierGroupSettings()
            {
                if ($scope.selectedSupplierGroupTemplate != undefined)
                    return supplierGroupSettingsDirectiveAPI.load();
            }

            function loadSupplierGroupTemplates() {
                return WhS_BE_CarrierAccountAPIService.GetSupplierGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.supplierGroupTemplates.push(item);
                    });
                });
            }

            function loadAppendixDirectives() {
                if ((saleZoneGroupSettingsDirectiveAPI == undefined && $scope.selectedSaleZoneGroupTemplate != undefined) ||
                    supplierGroupSettingsDirectiveAPI == undefined && $scope.selectedSupplierGroupTemplate != undefined)
                {
                    return;
                }

                UtilsService.waitMultipleAsyncOperations([loadSaleZoneGroupSettings, loadSuplierGroupSettings]).then(function () {

                    setAppendixDirectives();

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });
            }

            function setAppendixDirectives()
            {
                UtilsService.waitMultipleAsyncOperations([setSaleZoneGroupSettingsDirective, setSupplierGroupSettingsDirective]).then(function () {
                    appendixData = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }

            function setSaleZoneGroupSettingsDirective()
            {
                if ($scope.selectedSaleZoneGroupTemplate != undefined)
                    return saleZoneGroupSettingsDirectiveAPI.setData(appendixData.Settings.SaleZoneGroupSettings);
            }

            function setSupplierGroupSettingsDirective() {
                if ($scope.selectedSupplierGroupTemplate != undefined)
                    return supplierGroupSettingsDirectiveAPI.setData(appendixData.Settings.SupplierGroupSettings);
            }

            function buildRoutingProductObjFromScope() {

                var routingProduct = {
                    RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                    Name: $scope.routingProductName,
                    SaleZonePackageId: saleZonePackageDirectiveAPI.getData().SaleZonePackageId,
                    Settings: {
                        SaleZoneGroupSettings: getSaleZoneGroupSettings(),
                        SupplierGroupSettings: getSuppliersGroupSettings()
                    }
                };

                return routingProduct;
            }

            function fillScopeFromRoutingProductObj(routingProductObj) {
                $scope.routingProductName = routingProductObj.Name;
                saleZonePackageDirectiveAPI.setData(routingProductObj.SaleZonePackageId);

                if (routingProductObj.Settings != null)
                {
                    if (routingProductObj.Settings.SaleZoneGroupSettings != null && routingProductObj.Settings.SaleZoneGroupSettings.ConfigId != null) {
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupSettings.ConfigId, "TemplateConfigID");
                    }

                    if (routingProductObj.Settings != null && routingProductObj.Settings.SupplierGroupSettings != null && routingProductObj.Settings.SupplierGroupSettings.ConfigId != null) {
                        $scope.selectedSupplierGroupTemplate = UtilsService.getItemByVal($scope.supplierGroupTemplates, routingProductObj.Settings.SupplierGroupSettings.ConfigId, "TemplateConfigID");
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
