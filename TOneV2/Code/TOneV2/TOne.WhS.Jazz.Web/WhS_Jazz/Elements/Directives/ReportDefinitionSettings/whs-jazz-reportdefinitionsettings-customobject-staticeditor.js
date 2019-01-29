(function (app) {

    'use strict';

    whsJazzReportDefinitionSettingsCustomObjectStaticEditor.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_Analytic_AnalyticTypeEnum','VR_Analytic_AnalyticItemConfigAPIService'];

    function whsJazzReportDefinitionSettingsCustomObjectStaticEditor(UtilsService, VRUIUtilsService, VRNotificationService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/ReportDefinitionSettings/Templates/ReportDefinitionSettingsCustomObjectStaticEditor.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var rateCalculationTypeSelectorAPI;
            var rateCalculationTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var taxFilterGroup;
            var taxFilterDirectiveAPI;
            var taxFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var dimensions;
            var reportDefintionFilterGroup;
            var reportDefintionFilterDirectiveAPI;
            var reportDefintionFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            var analyticTableId = " 4c1aaa1b-675b-420f-8e60-26b0747ca79b";
            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.markets = [];

                $scope.scopeModel.regions = [];

                $scope.scopeModel.addMarket = function (item) {
                    var entity = {};

                    entity.onMarketSelectorReady = function (api) {
                        entity.marketDirectiveAPI = api;
                        var setLoader = function (value) { $scope.scopeModel.isMarketDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                    };
                    entity.onCustomerTypeSelectorReady = function (api) {
                        entity.customerTypeDirectiveAPI = api;
                        var payload = {};
                        var setLoader = function (value) { $scope.scopeModel.isCustomerTypeDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                    };
                    $scope.scopeModel.markets.push(entity);
                };

                $scope.scopeModel.onDeleteMarket = function (dataItem) {
                    var index = $scope.scopeModel.markets.indexOf(dataItem);
                    $scope.scopeModel.markets.splice(index, 1);
                };

                $scope.scopeModel.addRegion = function (item) {
                    var entity = {};

                    entity.onRegionSelectorReady = function (api) {
                        entity.regionDirectiveAPI = api;
                        var setLoader = function (value) { $scope.scopeModel.isRegionDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                    };

                    $scope.scopeModel.regions.push(entity);
                };

                $scope.scopeModel.onDeleteRegion = function (dataItem) {
                    var index = $scope.scopeModel.regions.indexOf(dataItem);
                    $scope.scopeModel.regions.splice(index, 1);
                };

                $scope.scopeModel.onRateCalculationTypeSelectorReady = function (api) {
                    rateCalculationTypeSelectorAPI = api;
                    rateCalculationTypeSelectorReadyPromiseDeferred.resolve();
                };
         
                $scope.scopeModel.onCreateTaxChanged = function () {
                    $scope.scopeModel.taxPercentage = undefined;
                    taxFilterGroup = undefined;
                        var setLoader = function (value) { $scope.scopeModel.isTaxFilterDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, taxFilterDirectiveAPI,undefined , setLoader);
                };

                $scope.scopeModel.onTaxFilterDirectiveReady = function (api) {
                    taxFilterDirectiveAPI = api;
                    taxFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    reportDefintionFilterDirectiveAPI = api;
                    reportDefintionFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.validateRegions = function () {
                    if ($scope.scopeModel.regions.length == 0)
                        return 'You Should At Least Add One Region ';
                    return null;
                };

                $scope.scopeModel.validateMarkets = function () {
                    if ($scope.scopeModel.markets.length == 0)
                        return 'You Should At Least Add One Market ';
                    return null;
                };
                defineAPI();
            }
            function loadTaxRecordFilterDirective() {
                var taxRecordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                taxFilterDirectiveReadyDeferred.promise.then(function () {
                    var taxFilterDirectivePayload = {
                        context: buildContext(),
                        FilterGroup: taxFilterGroup
                    }; 
                    VRUIUtilsService.callDirectiveLoad(taxFilterDirectiveAPI, taxFilterDirectivePayload, taxRecordFilterDirectiveLoadDeferred);
                });

                return taxRecordFilterDirectiveLoadDeferred.promise;
            }

            function loadRateCalculationTypeSelector(rateCalculationPayload) {
                var rateCalculationTypeSelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                rateCalculationTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(rateCalculationTypeSelectorAPI, rateCalculationPayload, rateCalculationTypeSelectorLoadromiseDeferred);
                });
                return rateCalculationTypeSelectorLoadromiseDeferred.promise;
            }

            function prepareRegionObject(regionObject) {

                var entity = {
                    Percentage: regionObject.payload.Percentage
                };
                entity.onRegionSelectorReady = function (api) {
                    entity.regionDirectiveAPI = api;
                    regionObject.regionReadyPromiseDeferred.resolve();
                };

                regionObject.regionReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: regionObject.payload.RegionCodeId
                    };
                    VRUIUtilsService.callDirectiveLoad(entity.regionDirectiveAPI, payload, regionObject.regionLoadPromiseDeferred);
                });

                $scope.scopeModel.regions.push(entity);
            }

            function prepareMarketObject(marketObject) {

                var entity = {
                    Percentage: marketObject.payload.Percentage
                };
                entity.onMarketSelectorReady = function (api) {

                    entity.marketDirectiveAPI = api;
                    marketObject.marketReadyPromiseDeferred.resolve();
                };
                entity.onCustomerTypeSelectorReady = function (api) {
                    entity.customerTypeDirectiveAPI = api;
                    marketObject.customerTypeReadyPromiseDeferred.resolve();
                };
                marketObject.marketReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: marketObject.payload.MarketCodeId
                    };
                    VRUIUtilsService.callDirectiveLoad(entity.marketDirectiveAPI, payload, marketObject.marketLoadPromiseDeferred);
                });
                marketObject.customerTypeReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: marketObject.payload.CustomerTypeCodeId
                    }; 
                    VRUIUtilsService.callDirectiveLoad(entity.customerTypeDirectiveAPI, payload, marketObject.customerTypeLoadPromiseDeferred);
                });
                $scope.scopeModel.markets.push(entity);
            }

            function loadReportDefinitionFilterDirective() {
                var reportDefintionFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                reportDefintionFilterDirectiveReadyDeferred.promise.then(function () {
                    var reportDefintionDirectivePayload = {
                        context: buildContext(),
                        FilterGroup: reportDefintionFilterGroup
                    };
                    VRUIUtilsService.callDirectiveLoad(reportDefintionFilterDirectiveAPI, reportDefintionDirectivePayload, reportDefintionFilterDirectiveLoadDeferred);
                });

                return reportDefintionFilterDirectiveLoadDeferred.promise;
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rateCalculationPayload;
                    var promises = [];
                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    loadDimensions().then(function () {
                        if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.Settings != undefined) {
                            var settings = payload.selectedValues.Settings;
                            rateCalculationPayload = { AmountCalculation: settings.AmountCalculation }; 
                            $scope.scopeModel.createTax = settings.CreateTax;
                            $scope.scopeModel.taxPercentage = settings.TaxPercentage;
                            $scope.scopeModel.divideByRegion = settings.DivideByRegion;
                            $scope.scopeModel.divideByMarket = settings.DivideByMarket;
                            reportDefintionFilterGroup = settings.ReportFilter;
                            taxFilterGroup = settings.TaxFilter;
                            if (settings.MarketSettings != undefined) {
                                var marketOptions = settings.MarketSettings.MarketOptions;
                                for (var j = 0; j < marketOptions.length; j++) {
                                    var marketOption = marketOptions[j];
                                    var marketObject = {
                                        payload: marketOption,
                                        marketReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        marketLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        customerTypeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        customerTypeLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    promises.push(marketObject.marketLoadPromiseDeferred.promise);
                                    promises.push(marketObject.customerTypeLoadPromiseDeferred.promise);
                                    prepareMarketObject(marketObject);
                                }
                            }
                            if (settings.RegionSettings != undefined) {
                                var regionOptions = settings.RegionSettings.RegionOptions;
                                for (var j = 0; j < regionOptions.length; j++) {
                                    var regionOption = regionOptions[j];
                                    var regionObject = {
                                        payload: regionOption,
                                        regionReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        regionLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(regionObject.regionLoadPromiseDeferred.promise);
                                    prepareRegionObject(regionObject);
                                }
                            }
                        }
                        
                        promises.push(loadRateCalculationTypeSelector(rateCalculationPayload));
                        promises.push(loadTaxRecordFilterDirective());
                        promises.push(loadReportDefinitionFilterDirective());
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            loadPromiseDeferred.resolve();
                        });
                    });
                    return loadPromiseDeferred.promise;
                };

                api.setData = function (payload) {
                    var regionOptions;
                    if ($scope.scopeModel.regions != undefined) {
                        regionOptions = [];
                        for (var i = 0; i < $scope.scopeModel.regions.length; i++) {
                            var item = $scope.scopeModel.regions[i];
                            regionOptions.push({
                                RegionCodeId: item.regionDirectiveAPI.getSelectedIds(),
                                Percentage: item.Percentage
                            });
                        }
                    } 
                    var marketOptions;
                    if ($scope.scopeModel.markets != undefined) {
                        marketOptions = [];
                        for (var i = 0; i < $scope.scopeModel.markets.length; i++) {
                            var item = $scope.scopeModel.markets[i];
                            marketOptions.push({
                                MarketCodeId: item.marketDirectiveAPI.getSelectedIds(),
                                CustomerTypeCodeId: item.customerTypeDirectiveAPI.getSelectedIds(),
                                Percentage: item.Percentage
                            });
                        }
                    }
                    payload.Settings = {
                        $type: "TOne.WhS.Jazz.Entities.JazzReportDefinitionSettings,TOne.WhS.Jazz.Entities",
                        AmountCalculation: rateCalculationTypeSelectorAPI.getSelectedIds(),
                      
                        DivideByMarket: $scope.scopeModel.divideByMarket,
                        MarketSettings: {
                            MarketOptions: marketOptions
                        },

                        DivideByRegion: $scope.scopeModel.divideByRegion,
                        RegionSettings: {
                            RegionOptions: regionOptions,

                        },
                        CreateTax: $scope.scopeModel.createTax,
                        TaxPercentage:$scope.scopeModel.taxPercentage,
                        TaxFilter: taxFilterDirectiveAPI.getData() != undefined ? taxFilterDirectiveAPI.getData().filterObj :undefined,
                        ReportFilter: reportDefintionFilterDirectiveAPI.getData() != undefined ? reportDefintionFilterDirectiveAPI.getData().filterObj : undefined
                    };
       
                    return payload;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
               return context = {
                    getFields: function () {
                        var fields = [];
                        if (dimensions != undefined) {
                            for (var i = 0; i < dimensions.length; i++) {
                                var dimension = dimensions[i];
                                fields.push({
                                    FieldName: dimension.Name,
                                    FieldTitle: dimension.Title,
                                    Type: dimension.Config.FieldType
                                });
                            }
                        }
                        return fields;
                    }
                };
            }

            function loadDimensions() {
                var input = {
                    TableIds: [analyticTableId],
                    ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                };
                return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                    dimensions = response;
                });
            }
        }
    }

    app.directive('whsJazzReportdefinitionsettingsCustomobjectStaticeditor', whsJazzReportDefinitionSettingsCustomObjectStaticEditor);

})(app);
