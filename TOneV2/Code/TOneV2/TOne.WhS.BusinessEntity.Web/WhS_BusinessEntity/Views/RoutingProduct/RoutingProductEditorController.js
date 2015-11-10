(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_Be_RoutingProductSaleZoneRelationTypeEnum', 'WhS_Be_RoutingProductSupplierRelationTypeEnum',
        'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_Be_RoutingProductSaleZoneRelationTypeEnum, WhS_Be_RoutingProductSupplierRelationTypeEnum,
        UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var routingProductId
        var routingProductEntity;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                routingProductId = parameters.routingProductId;
            }

            isEditMode = (routingProductId != undefined);
        }

        function defineScope() {

            $scope.onSellingNumberPlansSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.onSaleZoneSelectorReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.onSellingNumberPlanSelectionChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                if (selectedSellingNumberPlanId != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value };

                    var payload = {
                        sellingNumberPlanId: selectedSellingNumberPlanId,
                    }

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
                }
            }

            $scope.onSaleZoneRelationTypeSelectionChanged = function () {
                if ($scope.selectedSaleZones != undefined && $scope.selectedSaleZoneRelationType == WhS_Be_RoutingProductSaleZoneRelationTypeEnum.AllZones) {
                    $scope.selectedSaleZones.length = 0;
                }

                $scope.showSaleZoneSelector = ($scope.selectedSaleZoneRelationType == WhS_Be_RoutingProductSaleZoneRelationTypeEnum.SpecificZones);
            }

            $scope.onSupplierRelationTypeSelectionChanged = function () {
                if ($scope.selectedSuppliers != undefined && $scope.selectedSupplierRelationType == WhS_Be_RoutingProductSupplierRelationTypeEnum.AllSuppliers) {
                    $scope.selectedSuppliers.length = 0;
                }

                $scope.showSupplierSelector = ($scope.selectedSupplierRelationType == WhS_Be_RoutingProductSupplierRelationTypeEnum.SpecificSuppliers);
            }

            $scope.SaveRoutingProduct = function () {
                if (isEditMode) {
                    return updateRoutingProduct();
                }
                else {
                    return insertRoutingProduct();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getRoutingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routingProductEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getRoutingProduct() {
            return WhS_BE_RoutingProductAPIService.GetRoutingProduct(routingProductId).then(function (routingProduct) {
                routingProductEntity = routingProduct;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, loadSaleZoneRelationTypes,
                loadSaleZoneSelector, loadSupplierRelationTypes, loadSupplierSelector, loadStaticSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadSellingNumberPlanSelector() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: routingProductEntity != undefined ? routingProductEntity.SellingNumberPlanId : undefined
                    }

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }

        function loadSaleZoneRelationTypes() {
            $scope.saleZoneRelationType = UtilsService.getArrayEnum(WhS_Be_RoutingProductSaleZoneRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_Be_RoutingProductSaleZoneRelationTypeEnum, 'value', routingProductEntity.Settings.ZoneRelationType);
            else
                $scope.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_Be_RoutingProductSaleZoneRelationTypeEnum, 'value', WhS_Be_RoutingProductSaleZoneRelationTypeEnum.AllZones.value);
        }

        function loadSaleZoneSelector() {
            var saleZoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise
                .then(function () {

                    if (routingProductEntity != undefined) {
                        var zoneIds = [];
                        if (routingProductEntity.Settings != null && routingProductEntity.Settings.Zones != null) {
                            for (var i = 0; i < routingProductEntity.Settings.Zones.length; i++) {
                                zoneIds.push(routingProductEntity.Settings.Zones[i].ZoneId);
                            }
                        }

                        var saleZonePayload = {
                            sellingNumberPlanId: routingProductEntity.SellingNumberPlanId,
                            selectedIds: zoneIds.length > 0 ? zoneIds : undefined
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, saleZonePayload, saleZoneLoadPromiseDeferred);
                });

            return saleZoneLoadPromiseDeferred.promise;
        }

        function loadSupplierRelationTypes() {
            $scope.supplierRelationType = UtilsService.getArrayEnum(WhS_Be_RoutingProductSupplierRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.selectedSupplierRelationType = UtilsService.getEnum(WhS_Be_RoutingProductSupplierRelationTypeEnum, 'value', routingProductEntity.Settings.SupplierRelationType);
            else
                $scope.selectedSupplierRelationType = UtilsService.getEnum(WhS_Be_RoutingProductSupplierRelationTypeEnum, 'value', WhS_Be_RoutingProductSupplierRelationTypeEnum.AllSuppliers.value);
        }

        function loadSupplierSelector() {
            var carrierAccountLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise
                .then(function () {

                    if (routingProductEntity != undefined) {
                        var supplierIds = [];
                        if (routingProductEntity.Settings != null && routingProductEntity.Settings.Suppliers != null) {
                            for (var i = 0; i < routingProductEntity.Settings.Suppliers.length; i++) {
                                supplierIds.push(routingProductEntity.Settings.Suppliers[i].SupplierId);
                            }
                        }

                        var carrierAccountPayload = {
                            selectedIds: supplierIds.length > 0 ? supplierIds : undefined
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, carrierAccountLoadPromiseDeferred);
                });

            return carrierAccountLoadPromiseDeferred.promise;
        }

        function loadStaticSection() {
            if (routingProductEntity == undefined)
                return;

            $scope.routingProductName = routingProductEntity.Name;
        }

        function buildRoutingProductObjFromScope() {
            var routingProduct = {
                RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                Name: $scope.routingProductName,
                SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                Settings: {
                    ZoneRelationType: $scope.selectedSaleZoneRelationType.value,
                    Zones: buildRoutingProductZoneObj(),
                    SupplierRelationType: $scope.selectedSupplierRelationType.value,
                    Suppliers: buildRoutingProductSupplierObj()
                }
            };

            return routingProduct;
        }

        function buildRoutingProductZoneObj() {
            var routingProductZones = undefined;
            var zoneIds = saleZoneDirectiveAPI.getSelectedIds();
            
            if (zoneIds != undefined && zoneIds.length > 0)
            {
                routingProductZones = [];

                for (var i = 0; i < zoneIds.length; i++) {
                    routingProductZones.push({
                        ZoneId: zoneIds[i]
                    });
                }
            }

            return routingProductZones;
        }

        function buildRoutingProductSupplierObj()
        {
            var routingProductSuppliers = undefined;
            var supplierIds = carrierAccountDirectiveAPI.getSelectedIds();

            if (supplierIds != undefined && supplierIds.length > 0)
            {
                routingProductSuppliers = [];

                for (var i = 0; i < supplierIds.length; i++) {
                    routingProductSuppliers.push({
                        SupplierId: supplierIds[i]
                    });
                }
            }

            return routingProductSuppliers;
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
