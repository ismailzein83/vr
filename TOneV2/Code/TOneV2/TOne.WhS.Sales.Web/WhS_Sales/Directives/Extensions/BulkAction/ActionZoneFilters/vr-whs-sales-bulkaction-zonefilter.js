'use strict';

app.directive('vrWhsSalesBulkactionZonefilter', ['WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_Sales_SpecificApplicableZoneEntityTypeEnum', 'VRCommon_EntityFilterEffectiveModeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService',
    function (WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_SpecificApplicableZoneEntityTypeEnum, VRCommon_EntityFilterEffectiveModeEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var bulkActionZoneFilter = new BulkActionZoneFilter($scope, ctrl, $attrs);
                bulkActionZoneFilter.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionZoneFilters/Templates/BulkActionZoneFilterTemplate.html'
        };

        function BulkActionZoneFilter($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var zoneFilter;
            var bulkActionContext;

            var entityTypeSelectorAPI;
            var entityTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var countrySelectorAPI;
            var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var multipleCountrySelectorAPI;
            var multipleCountrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.gridDataSource = [];
                $scope.scopeModel.entityTypes = UtilsService.getArrayEnum(WhS_Sales_SpecificApplicableZoneEntityTypeEnum);
                $scope.scopeModel.selectedEntityType = UtilsService.getItemByVal($scope.scopeModel.entityTypes, WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value, 'value');

                $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                    entityTypeSelectorAPI = api;
                    entityTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onEntityTypeSelectionChanged = function (selectedEntityType) {
                    if (selectedEntityType == undefined)
                        return;
                    $scope.scopeModel.selectedCountry = undefined;
                    $scope.scopeModel.selectedCountries.length = 0;
                    $scope.scopeModel.isSaleZoneSelectorVisible = (selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value);
                    $scope.scopeModel.isLoading = true;
                    loadSaleZoneSelector().then(function () {
                        $scope.scopeModel.isLoading = false;
                    });

                    switch (selectedEntityType.value) {
                        case WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value:
                            $scope.scopeModel.showCountrySelector = false;
                            $scope.scopeModel.showMultipleCountrySelector = true;
                            break;
                        case WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value:
                            $scope.scopeModel.showCountrySelector = true;
                            $scope.scopeModel.showMultipleCountrySelector = false;
                            break;
                    }
                };

                $scope.scopeModel.onCountrySelectorReady = function (api) {
                    countrySelectorAPI = api;
                    countrySelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onCountrySelectionChanged = function (selectedCountry) {
                    if ($scope.scopeModel.selectedEntityType != undefined && $scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value) {
                        $scope.scopeModel.isLoading = true;
                        loadSaleZoneSelector().then(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
                };

                $scope.scopeModel.onMultipleCountrySelectorReady = function (api) {
                    multipleCountrySelectorAPI = api;
                    multipleCountrySelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onMultipleCountrySelectionChanged = function (selectedCountry) {
                    //if ($scope.scopeModel.selectedEntityType != undefined && $scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
                    //    loadSaleZoneSelector();
                };

                $scope.scopeModel.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };
                $scope.scopeModel.isGridDataValid = function () {
                    return ($scope.scopeModel.gridDataSource.length == 0) ? 'No filters exist' : null;
                };

                $scope.scopeModel.add = function () {
                    add();
                };
                $scope.scopeModel.isAddButtonDisabled = function () {
                    if ($scope.scopeModel.selectedEntityType == undefined) {
                        return true;
                    }
                    else if ($scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value) {
                        return ($scope.scopeModel.selectedCountries == undefined || $scope.scopeModel.selectedCountries.length == 0);
                    }
                    else {
                        return ($scope.scopeModel.selectedSaleZones == undefined || $scope.scopeModel.selectedSaleZones.length == 0);
                    }
                };
                $scope.scopeModel.remove = function (dataRow) {
                    var entities = UtilsService.getPropValuesFromArray($scope.scopeModel.gridDataSource, 'Entity');
                    if (entities == undefined)
                        return;
                    var index = UtilsService.getItemIndexByVal(entities, dataRow.Entity.id, 'id');
                    $scope.scopeModel.gridDataSource.splice(index, 1);

                    $scope.scopeModel.isLoading = true;
                    UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadMultipleCountrySelector, loadSaleZoneSelector]).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                UtilsService.waitMultiplePromises([entityTypeSelectorReadyDeferred.promise, countrySelectorReadyDeferred.promise, saleZoneSelectorReadyDeferred.promise, gridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {
                        zoneFilter = payload.zoneFilter;
                        bulkActionContext = payload.bulkActionContext;
                    }

                    extendBulkActionContext();

                    var loadCountrySelectorPromise = loadCountrySelector();
                    promises.push(loadCountrySelectorPromise);

                    var loadMultipleCountrySelectorPromise = loadMultipleCountrySelector();
                    promises.push(loadMultipleCountrySelectorPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        CountryZonesByCountry: {},
                        IncludedZoneIds: []
                    };
                    for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
                        var entity = $scope.scopeModel.gridDataSource[i].Entity;
                        if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
                            data.IncludedZoneIds.push(entity.entityId);
                        else {
                            data.CountryZonesByCountry[entity.entityId] = {
                                CountryId: entity.entityId,
                                ExcludedZoneIds: entity.saleZoneSelectorAPI.getSelectedIds()
                            };
                        }
                    }
                    return data;
                };

                api.getSummary = function () {
                    var numberOfCountries = 0;
                    var numberOfZones = 0;

                    for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
                        var entity = $scope.scopeModel.gridDataSource[i].Entity;

                        if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value)
                            numberOfCountries++;
                        else
                            numberOfZones++;
                    }

                    return 'Countries: ' + numberOfCountries + ' | Zones: ' + numberOfZones;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function extendBulkActionContext() {
                if (bulkActionContext == undefined)
                    return;
                bulkActionContext.onBulkActionChanged = function () {
                    $scope.scopeModel.gridDataSource.length = 0;
                    $scope.scopeModel.isLoading = true;
                    return UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadMultipleCountrySelector, loadSaleZoneSelector]).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };
            }

            function loadCountrySelector() {
                var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                var countrySelectorPayload = {
                    filter: {
                        ExcludedCountryIds: getExcludedCountryIds(),
                        Filters: getCountrySelectorFilters()
                    }
                };
                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);

                return countrySelectorLoadDeferred.promise;
            }
            function loadMultipleCountrySelector() {
                var multipleCountrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                var multipleCountrySelectorPayload = {
                    filter: {
                        ExcludedCountryIds: getExcludedCountryIds(),
                        Filters: getCountrySelectorFilters()
                    }
                };
                VRUIUtilsService.callDirectiveLoad(multipleCountrySelectorAPI, multipleCountrySelectorPayload, multipleCountrySelectorLoadDeferred);

                return multipleCountrySelectorLoadDeferred.promise;
            }
            function getExcludedCountryIds() {
                var excludedCountryIds = [];
                for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
                    var entity = $scope.scopeModel.gridDataSource[i].Entity;
                    if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value)
                        excludedCountryIds.push(entity.entityId);
                }
                return excludedCountryIds;
            }
            function getCountrySelectorFilters() {
                var countrySelectorFilters = [];

                var ownerType;
                var ownerId;
                var bulkAction;

                if (bulkActionContext != undefined) {
                    ownerType = bulkActionContext.ownerType;
                    ownerId = bulkActionContext.ownerId;
                    if (bulkActionContext.getSelectedBulkAction != undefined)
                        bulkAction = bulkActionContext.getSelectedBulkAction();
                }

                //The CountrySoldToCustomerFilter must be executed first because the others depend on it
                if (ownerType === WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                    var countrySoldToCustomerFilter = {
                        $type: 'TOne.WhS.Sales.Business.RPCountrySoldToCustomerFilter, TOne.WhS.Sales.Business',
                        CustomerId: bulkActionContext.ownerId,
                        EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                        IsEffectiveInFuture: true
                    };
                    countrySelectorFilters.push(countrySoldToCustomerFilter);
                }

                var bulkActionApplicableToCountryFilter = {
                    $type: 'TOne.WhS.Sales.Business.BulkActionApplicableToCountryFilter, TOne.WhS.Sales.Business',
                    OwnerType: ownerType,
                    OwnerId: ownerId,
                    BulkAction: bulkAction
                };
                countrySelectorFilters.push(bulkActionApplicableToCountryFilter);

                return countrySelectorFilters;
            }

            function loadSaleZoneSelector() {
                var saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                var saleZoneSelectorPayload = getSaleZoneSelectorPayload();
                VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, saleZoneSelectorLoadDeferred);

                return saleZoneSelectorLoadDeferred.promise;
            }
            function getSaleZoneSelectorPayload() {
                var saleZoneSelectorPayload = {
                    filter: {
                        EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.CurrentAndFuture.value
                    }
                };

                if (bulkActionContext != undefined) {
                    saleZoneSelectorPayload.sellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
                }

                var countryId = countrySelectorAPI.getSelectedIds();
                if (countryId != undefined) {
                    saleZoneSelectorPayload.filter.CountryIds = [];
                    saleZoneSelectorPayload.filter.CountryIds.push(countryId);
                }

                saleZoneSelectorPayload.filter.ExcludedZoneIds = getExcludedSaleZoneIds();
                saleZoneSelectorPayload.filter.Filters = getSaleZoneSelectorFilters();

                return saleZoneSelectorPayload;
            }
            function getExcludedSaleZoneIds() {
                var excludedSaleZoneIds = [];
                for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
                    var entity = $scope.scopeModel.gridDataSource[i].Entity;
                    if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
                        excludedSaleZoneIds.push(entity.entityId);
                    else if (entity.saleZoneSelectorAPI != undefined){
                        var selectedSaleZoneIds = entity.saleZoneSelectorAPI.getSelectedIds();
                        if (selectedSaleZoneIds != undefined) {
                            for (var j = 0; j < selectedSaleZoneIds.length; j++)
                                excludedSaleZoneIds.push(selectedSaleZoneIds[j]);
                        }
                    }
                }
                return excludedSaleZoneIds;
            }
            function getSaleZoneSelectorFilters() {
                var saleZoneSelectorFilters = [];

                var ownerType;
                var ownerId;
                var bulkAction;

                if (bulkActionContext != undefined) {
                    ownerType = bulkActionContext.ownerType;
                    ownerId = bulkActionContext.ownerId;
                    if (bulkActionContext.getSelectedBulkAction != undefined)
                        bulkAction = bulkActionContext.getSelectedBulkAction();
                }
                //The SaleZoneCountrySoldToCustomerFilter must be executed first because the others depend on it
                if (ownerType != undefined && ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
                    var countrySoldToCustomerFilter = {
                        $type: 'TOne.WhS.Sales.Business.SaleZoneCountrySoldToCustomerFilter, TOne.WhS.Sales.Business',
                        CustomerId: ownerId,
                        EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime()),
                        IsEffectiveInFuture: true
                    };
                    saleZoneSelectorFilters.push(countrySoldToCustomerFilter);
                }
                  var applicableSaleZoneFilter = {
                    $type: 'TOne.WhS.Sales.Business.ApplicableSaleZoneFilter, TOne.WhS.Sales.Business',
                        OwnerType: ownerType,
                        OwnerId: ownerId,
                        ActionType: bulkAction
                };
                saleZoneSelectorFilters.push(applicableSaleZoneFilter);

                return saleZoneSelectorFilters;
            }

            function add() {
                if ($scope.scopeModel.selectedEntityType == undefined)
                    return;

                var entityTypeValue = $scope.scopeModel.selectedEntityType.value;
                var entityTypeDescription = $scope.scopeModel.selectedEntityType.description;

                if ($scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value) {
                    addCountries(entityTypeValue, entityTypeDescription);
                }
                else if ($scope.scopeModel.selectedSaleZones != undefined) {
                    for (var i = 0; i < $scope.scopeModel.selectedSaleZones.length; i++) {
                        var saleZone = $scope.scopeModel.selectedSaleZones[i];
                        var entity = {
                            id: ($scope.scopeModel.gridDataSource.length + 1),
                            entityTypeValue: entityTypeValue,
                            entityTypeDescription: entityTypeDescription,
                            entityId: saleZone.SaleZoneId,
                            entityName: saleZone.Name
                        };
                        $scope.scopeModel.gridDataSource.push({ Entity: entity });
                    }
                    $scope.scopeModel.isLoading = true;
                    loadSaleZoneSelector().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            }
            function addCountries(entityTypeValue, entityTypeDescription) {
                if ($scope.scopeModel.selectedCountries == undefined || $scope.scopeModel.selectedCountries.length == 0)
                    return;

                $scope.scopeModel.isLoading = true;

                var directiveLoadPromises = [];


                for (var i = 0; i < $scope.scopeModel.selectedCountries.length; i++) {
                    var commonDirectivePayload = getCommonSaleZoneSelectorPayload();
                    var country = $scope.scopeModel.selectedCountries[i];
                    addCountry(country, entityTypeValue, entityTypeDescription, commonDirectivePayload, directiveLoadPromises);
                }

                UtilsService.waitMultiplePromises(directiveLoadPromises).then(function () {
                    UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadMultipleCountrySelector]).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });

                function getCommonSaleZoneSelectorPayload() {
                    var selectorPayload = {
                        filter: {
                            EffectiveMode: VRCommon_EntityFilterEffectiveModeEnum.CurrentAndFuture.value,
                            Filters: getSaleZoneSelectorFilters(),
                            ExcludedZoneIds: getExcludedSaleZoneIds()
                        }
                    };
                    if (bulkActionContext != undefined) {
                        selectorPayload.sellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
                    }
                    return selectorPayload;
                }
            }
            function addCountry(country, entityTypeValue, entityTypeDescription, commonDirectivePayload, directiveLoadPromises) {

                var countryEntity = {
                    id: ($scope.scopeModel.gridDataSource.length + 1),
                    entityId: country.CountryId,
                    entityName: country.Name,
                    entityTypeValue: entityTypeValue,
                    entityTypeDescription: entityTypeDescription,
                    isCountryEntityType: true
                };

                countryEntity.saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                directiveLoadPromises.push(countryEntity.saleZoneSelectorLoadDeferred.promise);

                countryEntity.onSaleZoneSelectorReady = function (api) {
                    countryEntity.saleZoneSelectorAPI = api;
                    onDirectiveReady(api, commonDirectivePayload, countryEntity.entityId, countryEntity.saleZoneSelectorLoadDeferred);
                };

                $scope.scopeModel.gridDataSource.push({ Entity: countryEntity });
            }
            function onDirectiveReady(directiveAPI, directivePayload, countryId, directiveLoadDeferred) {
                directivePayload.filter.CountryIds = [countryId];
                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
            }
        }
    }]);