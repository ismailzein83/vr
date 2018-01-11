(function (appControllers) {

    "use strict";

    routingProductEditorController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_RoutingProductSaleZoneRelationTypeEnum', 'WhS_BE_RoutingProductSupplierRelationTypeEnum',
        'WhS_BE_RoutingProductZoneServiceService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routingProductEditorController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_RoutingProductSaleZoneRelationTypeEnum, WhS_BE_RoutingProductSupplierRelationTypeEnum,
        WhS_BE_RoutingProductZoneServiceService, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var routingProductId;
        var routingProductEntity;
        var zoneNamesDict;
        var serviceNamesDict;

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var sellingNumberPlanSelectionChangedPromiseDeferred;

        var zoneServiceAPI;
        var zoneServiceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneRelationTypeSelectionChangedPromiseDeferred;

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var routingProductZoneServiceGridAPI;
        var routingProductZoneServiceGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel = {};
            $scope.scopeModel.routingProductZoneServices = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onSellingNumberPlansSelectorReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onZoneServiceSelectorReady = function (api) {
                zoneServiceAPI = api;
                zoneServiceSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRoutingProductZoneServiceGridReady = function (api) {
                routingProductZoneServiceGridAPI = api;
                routingProductZoneServiceGridReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSellingNumberPlanSelectionChanged = function () {
                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();

                if (selectedSellingNumberPlanId && saleZoneDirectiveAPI) {

                    var payload = {};
                    payload.selectedIds = routingProductEntity && routingProductEntity.Settings && routingProductEntity.Settings.Zones ? getSaleZoneSelectorPayload() : undefined;
                    payload.sellingNumberPlanId = selectedSellingNumberPlanId;
                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, sellingNumberPlanSelectionChangedPromiseDeferred);
                }
            };
            $scope.scopeModel.onSaleZoneRelationTypeSelectionChanged = function () {
                $scope.scopeModel.showSaleZoneSelector = ($scope.scopeModel.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.SpecificZones);

                if (saleZoneRelationTypeSelectionChangedPromiseDeferred != undefined)
                    saleZoneRelationTypeSelectionChangedPromiseDeferred.resolve();
                else {
                    if ($scope.scopeModel.selectedSaleZones != undefined && $scope.scopeModel.selectedSaleZoneRelationType == WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones) {
                        $scope.scopeModel.selectedSaleZones.length = 0;
                    }

                    $scope.scopeModel.routingProductZoneServices = [];
                }
            };
            $scope.scopeModel.onSupplierRelationTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedSuppliers != undefined && $scope.scopeModel.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.AllSuppliers) {
                    $scope.scopeModel.selectedSuppliers.length = 0;
                }

                $scope.scopeModel.showSupplierSelector = ($scope.scopeModel.selectedSupplierRelationType == WhS_BE_RoutingProductSupplierRelationTypeEnum.SpecificSuppliers);
            };

            $scope.scopeModel.onAddRoutingProductZoneService = function () {

                var onRoutingProductZoneServiceAdded = function (addedRoutingProductZoneService) {
                    $scope.scopeModel.routingProductZoneServices.push(addedRoutingProductZoneService);
                };

                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                var availableZonesIds = getAvailableZoneIds();
                var excludedZoneIds = getExcludedZoneIds();

                WhS_BE_RoutingProductZoneServiceService.addRoutingProductZoneService(selectedSellingNumberPlanId, availableZonesIds, excludedZoneIds, onRoutingProductZoneServiceAdded);
            };
            $scope.scopeModel.onRemoveRoutingProductZoneService = function (zoneService) {
                if ($scope.scopeModel.isRoutingProductHasRelatedSaleEntities == true)
                    return;
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = UtilsService.getItemIndexByVal($scope.scopeModel.routingProductZoneServices, zoneService.ZoneNames, 'ZoneNames');
                        $scope.scopeModel.routingProductZoneServices.splice(index, 1);
                    }
                });
            };

            $scope.scopeModel.onSelectSaleZone = function (selectedSaleZone) {
                if (zoneNamesDict == undefined)
                    zoneNamesDict = {};

                zoneNamesDict[selectedSaleZone.SaleZoneId] = selectedSaleZone.Name;
            };
            $scope.scopeModel.onDeselectSaleZone = function (deselectedSaleZone) {
                editRoutingProductZoneServiceObj(deselectedSaleZone.SaleZoneId);
            };

            $scope.scopeModel.validateSellingNumberPlan = function () {
                return ($scope.scopeModel.selectedSellingNumberPlan != undefined) ? null : 'No selling number plan selected';

            };

            $scope.scopeModel.SaveRoutingProduct = function () {
                if (isEditMode) {
                    return updateRoutingProduct();
                }
                else {
                    return insertRoutingProduct();
                }
            };

            $scope.hasSaveRoutingProductPermission = function () {
                if (isEditMode)
                    return WhS_BE_RoutingProductAPIService.HasUpdateRoutingProductPermission();
                else
                    return WhS_BE_RoutingProductAPIService.HasAddRoutingProductPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
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
                    isRoutingProductHasRelatedSaleEntities()
                        .then(function () {
                            loadAllControls()
                                .finally(function () {
                                    routingProductEntity = undefined;
                                });
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
            return WhS_BE_RoutingProductAPIService.GetRoutingProductEditorRuntime(routingProductId).then(function (routingProductEditorRuntime) {
                // this.routingProductEditorRuntime = routingProductEditorRuntime;

                routingProductEntity = routingProductEditorRuntime.Entity;
                zoneNamesDict = routingProductEditorRuntime.ZoneNames,
                serviceNamesDict = routingProductEditorRuntime.ServiceNames;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadSellingNumberPlanSelector, loadServiceZoneConfigSelector, loadSaleZoneRelationTypes, loadSaleZoneSelector,
                         loadRoutingProductZoneServicesGrid, loadSupplierRelationTypes, loadSupplierSelector])
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        })
                        .finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
        }

        function isRoutingProductHasRelatedSaleEntities() {
            return WhS_BE_RoutingProductAPIService.CheckIfRoutingProductHasRelatedSaleEntities(routingProductEntity.RoutingProductId).then(function (response) {
                $scope.scopeModel.isRoutingProductHasRelatedSaleEntities = response;
            });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(routingProductEntity ? routingProductEntity.Name : null, 'Routing Product', $scope) : UtilsService.buildTitleForAddEditor('Routing Product');
        }
        function loadStaticSection() {
            if (routingProductEntity == undefined)
                return;

            $scope.scopeModel.routingProductName = routingProductEntity.Name;
        }
        function loadSellingNumberPlanSelector() {
            var sellingNumberPlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sellingNumberPlanReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: routingProductEntity != undefined ? routingProductEntity.SellingNumberPlanId : undefined,
                        selectifsingleitem:true
                    };

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, directivePayload, sellingNumberPlanLoadPromiseDeferred);
                });
            return sellingNumberPlanLoadPromiseDeferred.promise;
        }
        function loadServiceZoneConfigSelector() {
            var serviceZoneConfigLoadDeferred = UtilsService.createPromiseDeferred();

            zoneServiceSelectorReadyPromiseDeferred.promise.then(function () {

                var payload;
                if (routingProductEntity != undefined && routingProductEntity.Settings != undefined) {
                    payload = { selectedIds: routingProductEntity.Settings.DefaultServiceIds };
                }

                VRUIUtilsService.callDirectiveLoad(zoneServiceAPI, payload, serviceZoneConfigLoadDeferred);
            });

            return serviceZoneConfigLoadDeferred.promise;
        }
        function loadSaleZoneRelationTypes() {
            $scope.scopeModel.saleZoneRelationType = UtilsService.getArrayEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum);

            if (routingProductEntity != undefined && routingProductEntity.Settings != null)
                $scope.scopeModel.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', routingProductEntity.Settings.ZoneRelationType);
            else
                $scope.scopeModel.selectedSaleZoneRelationType = UtilsService.getEnum(WhS_BE_RoutingProductSaleZoneRelationTypeEnum, 'value', WhS_BE_RoutingProductSaleZoneRelationTypeEnum.AllZones.value);
        }
        function loadSaleZoneSelector() {
            if (!isEditMode)
                return;

            if (sellingNumberPlanSelectionChangedPromiseDeferred == undefined)
                sellingNumberPlanSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

            return sellingNumberPlanSelectionChangedPromiseDeferred.promise;
        }
        function loadRoutingProductZoneServicesGrid() {
            if (!isEditMode || !routingProductEntity || !routingProductEntity.Settings || !routingProductEntity.Settings.ZoneServices)
                return;

            if (!$scope.scopeModel.isRoutingProductHasRelatedSaleEntities)
                defineMenuActions();

            saleZoneRelationTypeSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

            return UtilsService.waitMultiplePromises([saleZoneRelationTypeSelectionChangedPromiseDeferred.promise, routingProductZoneServiceGridReadyPromiseDeferred.promise]).then(function () {

                saleZoneRelationTypeSelectionChangedPromiseDeferred = undefined;

                var zoneServices = routingProductEntity.Settings.ZoneServices;
                for (var i = 0; i < zoneServices.length; i++) {
                    var currentZoneService = zoneServices[i];
                    extendRoutingProductZoneServiceObj(currentZoneService);
                    $scope.scopeModel.routingProductZoneServices.push(currentZoneService);
                }
            });

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: "Edit",
                    clicked: editZoneService,
                });
            }

            function editZoneService(zoneService) {

                var onRoutingProductZoneServiceUpdated = function (updatedRoutingProductZoneService) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.routingProductZoneServices, zoneService.ZoneNames, 'ZoneNames');
                    $scope.scopeModel.routingProductZoneServices[index] = updatedRoutingProductZoneService;
                };

                var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                var availableZonesIds = getAvailableZoneIds();
                var excludedZoneIds = getExcludedZoneIds();

                WhS_BE_RoutingProductZoneServiceService.editRoutingProductZoneService(zoneService, selectedSellingNumberPlanId, availableZonesIds, excludedZoneIds, onRoutingProductZoneServiceUpdated);
            }
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

        function extendRoutingProductZoneServiceObj(zoneService) {

            zoneService.ZoneNames = buildNamesFromIds(zoneService.ZoneIds, zoneNamesDict);
            zoneService.ServiceNames = buildNamesFromIds(zoneService.ServiceIds, serviceNamesDict);
        }
        function editRoutingProductZoneServiceObj(saleZoneId) {
            var zoneServices = $scope.scopeModel.routingProductZoneServices;

            for (var i = 0; i < zoneServices.length; i++) {
                var currentZoneService = zoneServices[i];
                for (var index = 0; index < currentZoneService.ZoneIds.length; index++) {
                    var zoneId = currentZoneService.ZoneIds[index];
                    if (zoneId == saleZoneId) {
                        currentZoneService.ZoneIds.splice(index, 1);

                        //Removing item/GridRow
                        if (currentZoneService.ZoneIds.length == 0) {
                            var indexToBeRemoved = i;
                            $scope.scopeModel.routingProductZoneServices.splice(indexToBeRemoved, 1);
                            return;
                        }

                        currentZoneService.ZoneNames = buildNamesFromIds(currentZoneService.ZoneIds, zoneNamesDict);
                        var indexToBeRemoved = i;
                        $scope.scopeModel.routingProductZoneServices[indexToBeRemoved] = currentZoneService;
                        return;
                    }
                }
            }
        }
        function buildNamesFromIds(listOfIds, namesDict) {

            var names = [];
            for (var i = 0; i < listOfIds.length; i++) {
                names.push(namesDict[listOfIds[i]]);
            }
            return names.join(", ");
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
                    DefaultServiceIds: zoneServiceAPI.getSelectedIds().sort(),
                    ZoneRelationType: $scope.scopeModel.selectedSaleZoneRelationType.value,
                    Zones: buildRoutingProductZoneObj(),
                    ZoneServices: buildRoutingProductZoneServiceObj(),
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
        function buildRoutingProductZoneServiceObj() {
            var routingProductZoneServices;

            if ($scope.scopeModel.routingProductZoneServices.length > 0) {
                routingProductZoneServices = [];
                for (var i = 0; i < $scope.scopeModel.routingProductZoneServices.length; i++) {
                    var currentItem = $scope.scopeModel.routingProductZoneServices[i];
                    routingProductZoneServices.push({
                        ZoneIds: currentItem.ZoneIds,
                        ServiceIds: currentItem.ServiceIds
                    });
                }
            }
            return routingProductZoneServices;
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

        function getAvailableZoneIds() {
            var availableZoneIds;
            if ($scope.scopeModel.selectedSaleZones != undefined && $scope.scopeModel.selectedSaleZones.length > 0) {
                availableZoneIds = [];
                for (var i = 0; i < $scope.scopeModel.selectedSaleZones.length; i++) {
                    availableZoneIds.push($scope.scopeModel.selectedSaleZones[i].SaleZoneId);
                }
            }
            return availableZoneIds;
        }
        function getExcludedZoneIds() {
            var zoneServices = $scope.scopeModel.routingProductZoneServices;
            if (zoneServices.length == 0)
                return;

            var excludedZoneIds = [];
            for (var i = 0; i < zoneServices.length; i++) {
                var currentZoneService = zoneServices[i];
                for (var index = 0; index < currentZoneService.ZoneIds.length; index++) {
                    excludedZoneIds.push(currentZoneService.ZoneIds[index]);
                }
            }
            return excludedZoneIds;
        }
    }

    appControllers.controller('WhS_BE_RoutingProductEditorController', routingProductEditorController);
})(appControllers);
