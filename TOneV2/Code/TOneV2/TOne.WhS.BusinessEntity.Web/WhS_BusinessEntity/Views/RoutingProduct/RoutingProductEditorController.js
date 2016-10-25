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
            $scope.isEditMode = isEditMode;
        }
        function defineScope() {
            $scope.hasSaveRoutingProductPermission = function () {
                if (isEditMode)
                    return WhS_BE_RoutingProductAPIService.HasUpdateRoutingProductPermission();
                else
                    return WhS_BE_RoutingProductAPIService.HasAddRoutingProductPermission();
            }
            $scope.scopeModel = {}
            $scope.scopeModel.onSellingNumberPlansSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();

            };

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onSellingNumberPlanSelectionChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();

                if (selectedSellingNumberPlanId && saleZoneDirectiveAPI) {
                    var payload = {};
                    payload.selectedIds = routingProductEntity && routingProductEntity.Settings && routingProductEntity.Settings.Zones ? getSaleZoneSelectorPayload() : undefined;
                    payload.sellingNumberPlanId = selectedSellingNumberPlanId;
                    saleZoneDirectiveAPI.load(payload);
                }
            }

            $scope.scopeModel.onSaleZoneRelationTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedSaleZones != undefined && $scope.scopeModel.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones) {
                    $scope.scopeModel.selectedSaleZones.length = 0;
                }

                $scope.scopeModel.showSaleZoneSelector = ($scope.scopeModel.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.SpecificZones);
            }

            $scope.scopeModel.onSupplierRelationTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedSuppliers != undefined && $scope.scopeModel.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.AllSuppliers) {
                    $scope.scopeModel.selectedSuppliers.length = 0;
                }

                $scope.scopeModel.showSupplierSelector = ($scope.scopeModel.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.SpecificSuppliers);
            }

            $scope.scopeModel.validateSellingNumberPlan = function () {
                return ($scope.scopeModel.selectedSellingNumberPlan != undefined) ? null : 'No selling number plan selected';

            }

            $scope.scopeModel.SaveRoutingProduct = function () {
                if (isEditMode) {
                    return updateRoutingProduct();
                }
                else {
                    return insertRoutingProduct();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            function getSaleZoneSelectorPayload() {
                var zoneIds = [];

                for (var i = 0; i < routingProductEntity.Settings.Zones.length; i++) {
                    zoneIds.push(routingProductEntity.Settings.Zones[i].ZoneId);
                }

                return zoneIds;
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getRoutingProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            routingProductEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
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
                        loadSupplierRelationTypes, loadSupplierSelector, loadStaticSection])
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        })
                        .finally(function () {
                            $scope.scopeModel.isLoading = false;
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
            $scope.scopeModel.saleZoneRelationType = UtilsService.getArrayEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.scopeModel.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', routingProductEntity.Settings.ZoneRelationType);
            else
                $scope.scopeModel.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones.value);
        }
        function loadSupplierRelationTypes() {
            $scope.scopeModel.supplierRelationType = UtilsService.getArrayEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.scopeModel.selectedSupplierRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum, 'value', routingProductEntity.Settings.SupplierRelationType);
            else
                $scope.scopeModel.selectedSupplierRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSupplierRelationTypeEnum, 'value', WhS_BE_RoutingProductSupplierRelationTypeEnum.AllSuppliers.value);
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

            $scope.scopeModel.routingProductName = routingProductEntity.Name;
        }

        function insertRoutingProduct() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_RoutingProductAPIService.AddRoutingProduct(buildRoutingProductObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Routing Product", response, "Name")) {
                    if ($scope.onRoutingProductAdded != undefined)
                        $scope.onRoutingProductAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateRoutingProduct() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_RoutingProductAPIService.UpdateRoutingProduct(buildRoutingProductObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Routing Product", response, "Name")) {
                    if ($scope.onRoutingProductUpdated != undefined)
                        $scope.onRoutingProductUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildRoutingProductObjFromScope() {
            var obj = {
                RoutingProductId: (routingProductId != null) ? routingProductId : 0,
                Name: $scope.scopeModel.routingProductName,
                Settings: {
                    ZoneRelationType: $scope.scopeModel.selectedSaleZoneRelationType.value,
                    Zones: buildRoutingProductZoneObj(),
                    SupplierRelationType: $scope.scopeModel.selectedSupplierRelationType.value,
                    Suppliers: buildRoutingProductSupplierObj()
                }
            };

            if (!isEditMode)
                obj.SellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();

            return obj;
        }
        function buildRoutingProductZoneObj() {
            var routingProductZones = undefined;
            var zoneIds = saleZoneDirectiveAPI.getSelectedIds();

            if (zoneIds != undefined && zoneIds.length > 0) {
                routingProductZones = [];

                for (var i = 0; i < zoneIds.length; i++) {
                    routingProductZones.push({
                        ZoneId: zoneIds[i]
                    });
                }
            }

            return routingProductZones;
        }
        function buildRoutingProductSupplierObj() {
            var routingProductSuppliers = undefined;
            var supplierIds = carrierAccountDirectiveAPI.getSelectedIds();

            if (supplierIds != undefined && supplierIds.length > 0) {
                routingProductSuppliers = [];

                for (var i = 0; i < supplierIds.length; i++) {
                    routingProductSuppliers.push({
                        SupplierId: supplierIds[i]
                    });
                }
            }

            return routingProductSuppliers;
        }
    }

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
