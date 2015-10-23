(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_CarrierAccountAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SellingNumberPlanAPIService, WhS_BE_SaleZoneAPIService, WhS_BE_CarrierAccountAPIService,
        UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

            var editMode;
            var routingProductId;

            var directiveAppendixData;

            var sellingNumberPlanDirectiveAPI;
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

                $scope.onSellingNumberPlansDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    load();
                }

                $scope.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                    saleZoneGroupSettingsDirectiveAPI = api;

                    if (directiveAppendixData != undefined)
                        tryLoadAppendixDirectives();
                    else
                        VRUIUtilsService.loadDirective($scope, saleZoneGroupSettingsDirectiveAPI, 'saleZonesAppendixLoader');
                }

                $scope.onSupplierGroupSettingsDirectiveReady = function (api) {
                    supplierGroupSettingsDirectiveAPI = api;

                    if (directiveAppendixData != undefined)
                        tryLoadAppendixDirectives();
                    else
                        VRUIUtilsService.loadDirective($scope, supplierGroupSettingsDirectiveAPI, 'suppliersAppendixLoader');
                }

                $scope.onSellingNumberPlanSelectionChanged = function () {
                    if (sellingNumberPlanDirectiveAPI != undefined)
                    {
                        var sellingNumberPlanObj = sellingNumberPlanDirectiveAPI.getData();
                        if (sellingNumberPlanObj != undefined)
                            $scope.selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getData().SellingNumberPlanId;
                    }
                    else
                    {
                        $scope.selectedSellingNumberPlanId = undefined;
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

                $scope.saleZoneGroupTemplates = [];
                $scope.supplierGroupTemplates = [];
            }

            function load() {
                $scope.isGettingData = true;

                if (sellingNumberPlanDirectiveAPI == undefined)
                    return;

                return UtilsService.waitMultipleAsyncOperations([sellingNumberPlanDirectiveAPI.load, loadSaleZoneGroupTemplates, loadSupplierGroupTemplates]).then(function () {
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
                    directiveAppendixData = routingProduct;
                    tryLoadAppendixDirectives();

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
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

            function tryLoadAppendixDirectives() {

                var loadOperations = [];
                var setDirectivesDataOperations = [];
                if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                    if (saleZoneGroupSettingsDirectiveAPI == undefined)
                        return;
                    loadOperations.push(saleZoneGroupSettingsDirectiveAPI.load);

                    setDirectivesDataOperations.push(setSaleZoneGroupSettingsDirective);
                }

                if ($scope.selectedSupplierGroupTemplate != undefined) {
                    if (supplierGroupSettingsDirectiveAPI == undefined)
                        return;

                    loadOperations.push(supplierGroupSettingsDirectiveAPI.load);

                    setDirectivesDataOperations.push(setSupplierGroupSettingsDirective);
                }

                UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {

                    setAppendixDirectives();

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });

                function setAppendixDirectives() {
                    UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {
                        directiveAppendixData = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.isGettingData = false;
                    });
                }

                function setSaleZoneGroupSettingsDirective() {                    
                        return saleZoneGroupSettingsDirectiveAPI.setData(directiveAppendixData.Settings.SaleZoneGroupSettings);
                }

                function setSupplierGroupSettingsDirective() {                    
                    return supplierGroupSettingsDirectiveAPI.setData(directiveAppendixData.Settings.SupplierGroupSettings);
                }
            }
                    
            function buildRoutingProductObjFromScope() {

                var routingProduct = {
                    RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                    Name: $scope.routingProductName,
                    SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getData().SellingNumberPlanId,
                    Settings: {
                        SaleZoneGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsDirectiveAPI, 'selectedSaleZoneGroupTemplate'),
                        SupplierGroupSettings: VRUIUtilsService.getSettingsFromDirective($scope, supplierGroupSettingsDirectiveAPI, 'selectedSupplierGroupTemplate')
                    }
                };

                return routingProduct;
            }

            function fillScopeFromRoutingProductObj(routingProductObj) {
                $scope.routingProductName = routingProductObj.Name;
                sellingNumberPlanDirectiveAPI.setData(routingProductObj.SellingNumberPlanId);

                if (routingProductObj.Settings != null)
                {
                    if (routingProductObj.Settings.SaleZoneGroupSettings != null) {
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routingProductObj.Settings.SaleZoneGroupSettings.ConfigId, "TemplateConfigID");
                    }

                    if (routingProductObj.Settings.SupplierGroupSettings != null) {
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
    }

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
