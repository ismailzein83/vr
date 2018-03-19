'use strict';

app.directive('vrWhsRoutingQcdefinitionTrafficstatisticSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QCDefinitionTrafficStatisticSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationDefinition/Extensions/Templates/QCDefinitionTrafficStatisticSettingsTemplate.html'
        };

        function QCDefinitionTrafficStatisticSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedAnalyticTableId;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableSelectorSelectionChangedDeferred;

            var measureSelectorAPI;
            var measureSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var saleZoneDimensionSelectorAPI;
            var saleZoneDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierDimensionSelectorAPI;
            var supplierDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneDimensionSelectorAPI;
            var supplierZoneDimensionReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onMeasureSelectorDirectiveReady = function (api) {
                    measureSelectorAPI = api;
                    measureSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onSaleZoneDimensionSelectorReady = function (api) {
                    saleZoneDimensionSelectorAPI = api;
                    saleZoneDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onSupplierDimensionSelectorReady = function (api) {
                    supplierDimensionSelectorAPI = api;
                    supplierDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onSupplierZoneDimensionSelectorReady = function (api) {
                    supplierZoneDimensionSelectorAPI = api;
                    supplierZoneDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onTableSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        selectedAnalyticTableId = selectedItem.AnalyticTableId;

                        if (tableSelectorSelectionChangedDeferred != undefined) {
                            tableSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadMeasureSelector();
                            loadSaleZoneDimensionSelector();
                            loadSupplierDimensionSelector();
                            loadSupplierZoneDimensionSelector();
                        }
                    }

                    function loadMeasureSelector() {
                        measureSelectorReadyDeferred.promise.then(function () {
                            var tableSelectorPayLoad = { filter: { TableIds: [selectedAnalyticTableId] } };
                            var setLoader = function (value) {
                                $scope.scopeModel.isMeasureSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, measureSelectorAPI, tableSelectorPayLoad, setLoader);
                        });
                    }
                    function loadSaleZoneDimensionSelector() {
                        saleZoneDimensionReadyDeferred.promise.then(function () {
                            var saleZoneDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            var setLoader = function (value) {
                                $scope.scopeModel.isSaleZoneDimensionSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDimensionSelectorAPI, saleZoneDimensionSelectorPayload, setLoader);
                        });
                    }
                    function loadSupplierDimensionSelector() {
                        supplierDimensionReadyDeferred.promise.then(function () {
                            var supplierDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            var setLoader = function (value) {
                                $scope.scopeModel.isSupplierDimensionSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierDimensionSelectorAPI, supplierDimensionSelectorPayload, setLoader);
                        });
                    }
                    function loadSupplierZoneDimensionSelector() {
                        supplierZoneDimensionReadyDeferred.promise.then(function () {
                            var supplierZoneDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            var setLoader = function (value) {
                                $scope.scopeModel.isSupplierZoneDimensionSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDimensionSelectorAPI, supplierZoneDimensionSelectorPayload, setLoader);
                        });
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var trafficStatisticQCDefinitionExtendedSettings;

                    if (payload != undefined) {
                        trafficStatisticQCDefinitionExtendedSettings = payload.qualityConfigurationDefinitionExtendedSettings;

                        if (trafficStatisticQCDefinitionExtendedSettings != undefined) {
                            selectedAnalyticTableId = trafficStatisticQCDefinitionExtendedSettings.AnalyticTableId;
                        }
                    }

                    var tableSelectorLoadPromise = getTableSelectorLoadPromise();
                    promises.push(tableSelectorLoadPromise);

                    if (selectedAnalyticTableId != undefined) {
                        tableSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var measureSelectorLoadPromise = getMeasureSelectorLoadPromise();
                        promises.push(measureSelectorLoadPromise);

                        var saleZoneDimensionSelectorLoadPromise = getSaleZoneDimensionSelectorLoadPromise();
                        promises.push(saleZoneDimensionSelectorLoadPromise);

                        var supplierDimensionSelectorLoadPromise = getSupplierDimensionSelectorLoadPromise();
                        promises.push(supplierDimensionSelectorLoadPromise);

                        var supplierZoneDimensionSelectorLoadPromise = getSupplierZoneDimensionSelectorLoadPromise();
                        promises.push(supplierZoneDimensionSelectorLoadPromise);
                    }


                    function getTableSelectorLoadPromise() {
                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        tableSelectorReadyDeferred.promise.then(function () {

                            var tableSelectorPayLoad;
                            if (selectedAnalyticTableId != undefined) {
                                tableSelectorPayLoad = { selectedIds: selectedAnalyticTableId };
                            }
                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, tableSelectorPayLoad, loadTableSelectorPromiseDeferred);
                        });

                        return loadTableSelectorPromiseDeferred.promise;
                    }
                    function getMeasureSelectorLoadPromise() {
                        var loadMeasureSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([tableSelectorSelectionChangedDeferred.promise, measureSelectorReadyDeferred.promise]).then(function () {

                            var measureSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            if (trafficStatisticQCDefinitionExtendedSettings && trafficStatisticQCDefinitionExtendedSettings.IncludedMeasures) {
                                measureSelectorPayload.selectedIds = trafficStatisticQCDefinitionExtendedSettings.IncludedMeasures;
                            }
                            VRUIUtilsService.callDirectiveLoad(measureSelectorAPI, measureSelectorPayload, loadMeasureSelectorPromiseDeferred);
                        });

                        return loadMeasureSelectorPromiseDeferred.promise;
                    }
                    function getSaleZoneDimensionSelectorLoadPromise() {
                        var loadSaleZoneDimensionPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([tableSelectorSelectionChangedDeferred.promise, saleZoneDimensionReadyDeferred.promise]).then(function () {

                            var saleZoneDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            if (trafficStatisticQCDefinitionExtendedSettings && trafficStatisticQCDefinitionExtendedSettings.SaleZoneFieldName) {
                                saleZoneDimensionSelectorPayload.selectedIds = trafficStatisticQCDefinitionExtendedSettings.SaleZoneFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(saleZoneDimensionSelectorAPI, saleZoneDimensionSelectorPayload, loadSaleZoneDimensionPromiseDeferred);
                        });

                        return loadSaleZoneDimensionPromiseDeferred.promise;
                    }
                    function getSupplierDimensionSelectorLoadPromise() {
                        var loadSupplierDimensionPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([tableSelectorSelectionChangedDeferred.promise, supplierDimensionReadyDeferred.promise]).then(function () {

                            var supplierDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            if (trafficStatisticQCDefinitionExtendedSettings && trafficStatisticQCDefinitionExtendedSettings.SupplierFieldName) {
                                supplierDimensionSelectorPayload.selectedIds = trafficStatisticQCDefinitionExtendedSettings.SupplierFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(supplierDimensionSelectorAPI, supplierDimensionSelectorPayload, loadSupplierDimensionPromiseDeferred);
                        });

                        return loadSupplierDimensionPromiseDeferred.promise;
                    }
                    function getSupplierZoneDimensionSelectorLoadPromise() {
                        var loadSupplierZoneDimensionPromiseDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([tableSelectorSelectionChangedDeferred.promise, supplierZoneDimensionReadyDeferred.promise]).then(function () {

                            var supplierZoneDimensionSelectorPayload = { filter: { TableIds: [selectedAnalyticTableId] } };
                            if (trafficStatisticQCDefinitionExtendedSettings && trafficStatisticQCDefinitionExtendedSettings.SupplierZoneFieldName) {
                                supplierZoneDimensionSelectorPayload.selectedIds = trafficStatisticQCDefinitionExtendedSettings.SupplierZoneFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(supplierZoneDimensionSelectorAPI, supplierZoneDimensionSelectorPayload, loadSupplierZoneDimensionPromiseDeferred);
                        });

                        return loadSupplierZoneDimensionPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        tableSelectorSelectionChangedDeferred = undefined;
                    });
                };

                api.getData = function () {

                    return {
                        $type: 'TOne.WhS.Routing.Business.TrafficStatisticQCDefinitionSettings, TOne.WhS.Routing.Business',
                        AnalyticTableId: tableSelectorAPI.getSelectedIds(),
                        IncludedMeasures: measureSelectorAPI.getSelectedIds(),
                        SaleZoneFieldName: saleZoneDimensionSelectorAPI.getSelectedIds(),
                        SupplierFieldName: supplierDimensionSelectorAPI.getSelectedIds(),
                        SupplierZoneFieldName: supplierZoneDimensionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);