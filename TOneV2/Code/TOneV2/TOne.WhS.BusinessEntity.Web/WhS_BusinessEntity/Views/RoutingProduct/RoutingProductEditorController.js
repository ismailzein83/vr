(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_RoutingProductSaleZoneRelationTypeEnum', 'WhS_BE_RoutingProductSupplierRelationTypeEnum',
        'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_RoutingProductSaleZoneRelationTypeEnum, WhS_BE_RoutingProductSupplierRelationTypeEnum,
        UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var routingProductId
        var routingProductEntity;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred;

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
            $scope.scopeModal = {}
            $scope.scopeModal.onSellingNumberPlansSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onSaleZoneSelectorReady = function (api) {
                saleZoneDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSaleZonesSelector = value; };
                var payload = {
                    sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader, saleZoneReadyPromiseDeferred);
               // UtilsService.safeApply($scope);
            };

            $scope.scopeModal.onCarrierAccountSelectorReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onSellingNumberPlanSelectionChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();

                if (selectedSellingNumberPlanId && saleZoneDirectiveAPI) {
                   // var setLoader = function (value) {
                        //$scope.scopeModal.isLoadingSaleZonesSelector = value;
                    //};

                    var payload = {
                        sellingNumberPlanId: selectedSellingNumberPlanId,
                    };
                    
                    saleZoneDirectiveAPI.load(payload);
                }
            }

            $scope.scopeModal.onSaleZoneRelationTypeSelectionChanged = function () {
                if ($scope.scopeModal.selectedSaleZones != undefined && $scope.scopeModal.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones) {
                    $scope.scopeModal.selectedSaleZones.length = 0;
                }

                $scope.scopeModal.showSaleZoneSelector = ($scope.scopeModal.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.SpecificZones );
            }

            $scope.scopeModal.onSupplierRelationTypeSelectionChanged = function () {
                if ($scope.scopeModal.selectedSuppliers != undefined && $scope.scopeModal.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.AllSuppliers) {
                    $scope.scopeModal.selectedSuppliers.length = 0;
                }

                $scope.scopeModal.showSupplierSelector = ($scope.scopeModal.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.SpecificSuppliers);
            }

            $scope.scopeModal.validateSellingNumberPlan = function () {
                return ($scope.scopeModal.selectedSellingNumberPlan != undefined) ? null : 'No selling number plan selected';

            }

            $scope.scopeModal.SaveRoutingProduct = function () {
                if (isEditMode) {
                    return updateRoutingProduct();
                }
                else {
                    return insertRoutingProduct();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getRoutingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routingProductEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSellingNumberPlanSelector, loadSaleZoneRelationTypes,
                loadSaleZoneSelector, loadSupplierRelationTypes, loadSupplierSelector, loadStaticSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(routingProductEntity ? routingProductEntity.Name : null, 'Routing Product') : UtilsService.buildTitleForAddEditor('Routing Product');
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
            $scope.scopeModal.saleZoneRelationType = UtilsService.getArrayEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.scopeModal.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', routingProductEntity.Settings.ZoneRelationType);
            else
                $scope.scopeModal.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones.value);
        }

        function loadSaleZoneSelector() {
            var saleZoneLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            if (routingProductEntity && routingProductEntity.Settings && routingProductEntity.Settings.Zones) {
                saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                saleZoneReadyPromiseDeferred.promise.then(function () {
                    saleZoneReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, getSaleZoneSelectorPayload(), saleZoneLoadPromiseDeferred);
                });
            }
            else {
                saleZoneLoadPromiseDeferred.resolve();
            }

            return saleZoneLoadPromiseDeferred.promise;

            function getSaleZoneSelectorPayload() {
                var zoneIds = [];

                for (var i = 0; i < routingProductEntity.Settings.Zones.length; i++) {
                    zoneIds.push(routingProductEntity.Settings.Zones[i].ZoneId);
                }

                return {
                    //sellingNumberPlanId: routingProductEntity.SellingNumberPlanId,
                    selectedIds: zoneIds
                };
            }
        }

        function loadSupplierRelationTypes() {
            $scope.scopeModal.supplierRelationType = UtilsService.getArrayEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.scopeModal.selectedSupplierRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum, 'value', routingProductEntity.Settings.SupplierRelationType);
            else
                $scope.scopeModal.selectedSupplierRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum, 'value', WhS_BE_RoutingProductSupplierRelationTypeEnum.AllSuppliers.value);
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

            $scope.scopeModal.routingProductName = routingProductEntity.Name;
        }

        function buildRoutingProductObjFromScope() {
            var routingProduct = {
                RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                Name: $scope.scopeModal.routingProductName,
                SellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                Settings: {
                    ZoneRelationType: $scope.scopeModal.selectedSaleZoneRelationType.value,
                    Zones: buildRoutingProductZoneObj(),
                    SupplierRelationType: $scope.scopeModal.selectedSupplierRelationType.value,
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
            $scope.scopeModal.isLoading = true;
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
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });

        }

        function updateRoutingProduct() {
            $scope.scopeModal.isLoading = true;
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
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }
    }

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
