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

            var dimensions;
            var reportDefintionFilterGroup;
            var reportDefintionFilterDirectiveAPI;
            var reportDefintionFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var amountTypeSelectorAPI;
            var amountTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            var amountTypeSelectedPromise;
            var analyticTableId = "795440c9-69e4-442e-a067-896bc969c73f";
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

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    reportDefintionFilterDirectiveAPI = api;
                    reportDefintionFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.validateRegions = function () {
                    if ($scope.scopeModel.regions.length == 0 )
                        return 'You Should At Least Add One Region ';
                    if (validatePercentages($scope.scopeModel.regions))
                        return 'Percentages Sum Must Be Equal To 100';
                    return null;
                };

                $scope.scopeModel.validateMarkets = function () {
                    if ($scope.scopeModel.markets.length == 0)
                        return 'You Should At Least Add One Market ';
                    if (validatePercentages($scope.scopeModel.markets))
                        return 'Percentages Sum Must Be Equal To 100';
                    return null;
                };
                $scope.scopeModel.showSplitRateValue = function () {
                    if (amountTypeSelectorAPI != undefined && amountTypeSelectorAPI.getData() != undefined)
                        return true;
                    return false;
                };
                $scope.scopeModel.onRateCalculationTypeSelectorReady = function (api) {
                    amountTypeSelectorAPI = api;
                    amountTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAmountTypeSelectionChanged = function (value) {
                    if (amountTypeSelectorAPI) {
                        if (value) {
                            if (amountTypeSelectedPromise != undefined) {
                                amountTypeSelectedPromise = undefined;
                            }
                            else {

                                currencySelectorAPI.load();
                                $scope.scopeModel.splitRateValue = undefined;
                            }
                        }
                    }
                };
                defineAPI();
            }

            function validatePercentages(items) {
                var percentagesSum=0;
                for (var i = 0; i < items.length; i++) {
                    percentagesSum = Number(percentagesSum) +Number(items[i].Percentage);
                }
                if (percentagesSum == 100)
                    return false;
                return true;
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
                        selectedIds: regionObject.payload.RegionId
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
                        selectedIds: marketObject.payload.MarketId
                    };
                    VRUIUtilsService.callDirectiveLoad(entity.marketDirectiveAPI, payload, marketObject.marketLoadPromiseDeferred);
                });
                marketObject.customerTypeReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        selectedIds: marketObject.payload.CustomerTypeId
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
            function loadAmountTypeSelector(payload) {
                var amountTypeSelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                amountTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(amountTypeSelectorAPI, payload, amountTypeSelectorLoadromiseDeferred);
                    amountTypeSelectedPromise = UtilsService.createPromiseDeferred();
                });
                return amountTypeSelectorLoadromiseDeferred.promise;
            }
            function loadCurrencySelector(payload) {
                var currencySelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                currencySelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, currencySelectorLoadPromiseDeferred);
                });
                return currencySelectorLoadPromiseDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedValues;
                    var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                    loadDimensions().then(function () {
                        if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.Settings != undefined) {
                            selectedValues = payload.selectedValues;
                            var settings = selectedValues.Settings;
                          
                            reportDefintionFilterGroup = settings.ReportFilter;
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
                        $scope.scopeModel.splitRateValue = selectedValues.SplitRateValue;
                        promises.push(loadAmountTypeSelector({ selectedIds: selectedValues.AmountType }));
                        promises.push(loadCurrencySelector({ selectedIds: selectedValues.CurrencyId }));
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
                                RegionId: item.regionDirectiveAPI.getSelectedIds(),
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
                                MarketId: item.marketDirectiveAPI.getSelectedIds(),
                                CustomerTypeId: item.customerTypeDirectiveAPI.getSelectedIds(),
                                Percentage: item.Percentage
                            });
                        }
                    }
                    payload.AmountType = amountTypeSelectorAPI.getData();
                    payload.SplitRateValue = $scope.scopeModel.splitRateValue;
                    payload.CurrencyId = currencySelectorAPI.getSelectedIds();
                    payload.Settings = {
                        $type: "TOne.WhS.Jazz.Entities.JazzReportDefinitionSettings,TOne.WhS.Jazz.Entities",
                        MarketSettings: {
                            MarketOptions: marketOptions
                        },

                        RegionSettings: {
                            RegionOptions: regionOptions,

                        },
                      
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
