'use strict';

app.directive('vrWhsDealSwapdealanalysisStaticeditor', ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_SwapDealAPIService', 'WhS_Deal_SwapDealAnalysisAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Deal_SwapDealAPIService, WhS_Deal_SwapDealAnalysisAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var swapDealAnalysis = new SwapDealAnalysis($scope, ctrl, $attrs);
                swapDealAnalysis.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisStaticeditorTemplate.html'
        };

        function SwapDealAnalysis($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var settingsAPI;
            var settingsReadyDeferred = UtilsService.createPromiseDeferred();
            var settingsLoadDeferred = UtilsService.createPromiseDeferred();

            var inboundAPI;
            var inboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

            var outboundAPI;
            var outboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

            var swapDealSettings;
            var swapDealSettingsLoadDeferred = UtilsService.createPromiseDeferred();

            var resultAPI;
            var resultReadyDeferred = UtilsService.createPromiseDeferred();

            var settings;
            var inbounds;
            var outbounds;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSettingsReady = function (api) {
                    settingsAPI = api;
                    settingsReadyDeferred.resolve();
                };

                $scope.scopeModel.onInboundManagementReady = function (api) {
                    inboundAPI = api;
                    inboundManagementReadyDeferred.resolve();
                };

                $scope.scopeModel.onOutboundManagementReady = function (api) {
                    outboundAPI = api;
                    outboundManagementReadyDeferred.resolve();
                };

                $scope.scopeModel.analyze = function () {

                    $scope.scopeModel.isLoading = true;
                    var promises = [];

                    var analyzeDealPromise = analyzeDeal();
                    promises.push(analyzeDealPromise);

                    var loadInboundManagementDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadInboundManagementDeferred.promise);

                    var loadOutboundManagementDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadOutboundManagementDeferred.promise);

                    var loadResultDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadResultDeferred.promise);

                    analyzeDealPromise.then(function (response) {
                        if (response == null) {
                            loadInboundManagementDeferred.resolve();
                            loadOutboundManagementDeferred.resolve();
                            return;
                        }
                        if (response == null) {
                            loadInboundManagementDeferred.resolve();
                            loadOutboundManagementDeferred.resolve();
                            return;
                        }
                        var inboundManagementData = {
                            Inbounds: response.Inbounds,
                            Summary: {
                                TotalSaleMargin: response.TotalSaleMargin,
                                TotalSaleRevenue: response.TotalSaleRevenue
                            }
                        };

                        loadInboundManagement(inboundManagementData).then(function () {
                            loadInboundManagementDeferred.resolve();
                        }).catch(function (error) {
                            loadInboundManagementDeferred.reject(error, $scope);
                        });

                        var outboundManagementData = {
                            Outbounds: response.Outbounds,
                            Summary: {
                                TotalCostMargin: response.TotalCostMargin,
                                TotalCostRevenue: response.TotalCostRevenue
                            }
                        };
                        loadOutboundManagement(outboundManagementData).then(function () {
                            loadOutboundManagementDeferred.resolve();
                        }).catch(function (error) {
                            loadOutboundManagementDeferred.reject(error, $scope);
                        });


                        var resultData = {
                            DealPeriodInDays: response.DealPeriodInDays,
                            TotalCostRevenue: response.TotalCostRevenue,
                            TotalSaleRevenue: response.TotalSaleRevenue,
                            TotalCostMargin: response.TotalCostMargin,
                            TotalSaleMargin: response.TotalSaleMargin,
                            OverallProfit: response.OverallProfit,
                            Margins: response.Margins,
                            OverallRevenue: response.OverallRevenue
                        };
                        loadResult(resultData).then(function () {
                            loadResultDeferred.resolve();
                        }).catch(function (error) {
                            loadResultDeferred.reject(error);
                        });
                    });

                    UtilsService.waitMultiplePromises(promises).finally(function () {

                    });
                    return analyzeDealPromise;
                };

                $scope.scopeModel.onResultReady = function (api) {
                    resultAPI = api;
                    resultReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            settings = {};
                            settings.CarrierAccountId = selectedValues.CarrierId;
                            settings.Name = selectedValues.Name;
                            settings.FromDate = selectedValues.DealBED;
                            settings.ToDate = selectedValues.DealEED;
                            inbounds = selectedValues.Inbounds.SwapDealAnalysisInbounds;
                            outbounds = selectedValues.Outbounds.SwapDealAnalysisOutbounds;
                        }
                    }
                    loadAllControls();
                };

                api.setData = function (swapDealAnalysisObject) {
                    var settingsData = settingsAPI.getData();
                    if (settingsData != undefined) {
                        swapDealAnalysisObject.Name = settingsData.Name;
                        swapDealAnalysisObject.CarrierId = settingsData.CarrierAccountId;
                        swapDealAnalysisObject.DealBED = settingsData.FromDate;
                        swapDealAnalysisObject.DealEED = settingsData.ToDate;
                        swapDealAnalysisObject.Inbounds =
                            {
                                $type: "TOne.WhS.Deal.Entities.SwapDealAnalysisInbound,TOne.WhS.Deal.Entities",
                                SwapDealAnalysisInbounds: inboundAPI.getData()
                            };
                        swapDealAnalysisObject.Outbounds =
                            {
                                $type: "TOne.WhS.Deal.Entities.SwapDealAnalysisOutbound,TOne.WhS.Deal.Entities",
                                SwapDealAnalysisOutbounds: outboundAPI.getData()
                            };
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadSwapDealSettings, loadSettings, loadInboundManagement, loadOutboundManagement]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
            function setTitle() {

                $scope.title = UtilsService.buildTitleForAddEditor('Swap Deal Analysis');
            }
            function loadSwapDealSettings() {
                return WhS_Deal_SwapDealAPIService.GetSwapDealSettingData().then(function (response) {
                    swapDealSettings = response;
                    swapDealSettingsLoadDeferred.resolve();
                });
            }
            function loadSettings() {
                settingsReadyDeferred.promise.then(function () {
                    var settingsPayload = {};
                    settingsPayload.context = {};
                    settingsPayload.context.setSellingNumberPlanId = setSellingNumberPlanId;
                    settingsPayload.context.clearAnalysis = clearAnalysis;
                    if (settings != undefined)
                        settingsPayload.Settings = settings;

                    VRUIUtilsService.callDirectiveLoad(settingsAPI, settingsPayload, settingsLoadDeferred);
                });

                return settingsLoadDeferred.promise;
            }
            function loadInboundManagement(data) {
                var inboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([swapDealSettingsLoadDeferred.promise, settingsLoadDeferred.promise, inboundManagementReadyDeferred.promise]).then(function () {
                    var inboundManagementPayload = {
                        context: {
                            settingsAPI: settingsAPI,
                            clearResult: clearResult
                        },
                        settings: {
                            defaultRateCalcMethodId: swapDealSettings.DefaultInboundRateCalcMethodId,
                            inboundRateCalcMethods: swapDealSettings.InboundCalculationMethods
                        }
                        , Inbounds: inbounds
                    };
                    if (data != undefined) {
                        inboundManagementPayload.Inbounds = data.Inbounds;
                        inboundManagementPayload.Summary = data.Summary;
                    }
                    VRUIUtilsService.callDirectiveLoad(inboundAPI, inboundManagementPayload, inboundManagementLoadDeferred);
                });

                return inboundManagementLoadDeferred.promise;
            }
            function loadOutboundManagement(data) {
                var outboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([swapDealSettingsLoadDeferred.promise, settingsLoadDeferred.promise, outboundManagementReadyDeferred.promise]).then(function () {
                    var outboundManagementPayload = {
                        context: {
                            settingsAPI: settingsAPI,
                            clearResult: clearResult
                        },
                        settings: {
                            defaultRateCalcMethodId: swapDealSettings.DefaultCalculationMethodId,
                            outboundRateCalcMethods: swapDealSettings.OutboundCalculationMethods
                        }
                        , Outbounds: outbounds
                    };
                    if (data != undefined) {
                        outboundManagementPayload.Outbounds = data.Outbounds;
                        outboundManagementPayload.Summary = data.Summary;
                    }
                    VRUIUtilsService.callDirectiveLoad(outboundAPI, outboundManagementPayload, outboundManagementLoadDeferred);
                });

                return outboundManagementLoadDeferred.promise;
            }
            function setSellingNumberPlanId(sellingNumberPlanId) {
                inboundAPI.setSellingNumberPlanId(sellingNumberPlanId);
            }
            function clearAnalysis() {
                inboundAPI.clear();
                outboundAPI.clear();
            }
            function clearResult() {
            }
            function analyzeDeal() {

                var settingsData = settingsAPI.getData();
                var inboundManagementData = inboundAPI.getData();
                var outboundManagementData = outboundAPI.getData();
                var analysisSettings = {};

                if (settingsData != undefined) {
                    analysisSettings.CarrierAccountId = settingsData.CarrierAccountId;
                    analysisSettings.FromDate = settingsData.FromDate;
                    analysisSettings.ToDate = settingsData.ToDate;
                }
                if (inboundManagementData != undefined) {
                    var inbounds = [];
                    for (var i = 0; i < inboundManagementData.$values.length; i++) {
                        var inbound =
                        {
                            GroupName: inboundManagementData.$values[i].GroupName,
                            CountryId: inboundManagementData.$values[i].CountryId,
                            SaleZoneIds: inboundManagementData.$values[i].SaleZoneIds,
                            Volume: inboundManagementData.$values[i].Volume,
                            DealRate: inboundManagementData.$values[i].DealRate,
                            ItemCalculationMethod: inboundManagementData.$values[i].ItemCalculationMethod,
                            CalculationMethodId: inboundManagementData.$values[i].CalculationMethodId
                        };
                        inbounds.push(inbound);
                    }
                    analysisSettings.Inbounds = inbounds;
                }
                if (outboundManagementData != undefined) {
                    var outbounds = [];
                    for (var j = 0; j < outboundManagementData.$values.length; j++) {
                        var outbound =
                        {
                            GroupName: outboundManagementData.$values[j].GroupName,
                            CountryId: outboundManagementData.$values[j].CountryId,
                            SupplierZoneIds: outboundManagementData.$values[j].SupplierZoneIds,
                            Volume: outboundManagementData.$values[j].Volume,
                            DealRate: outboundManagementData.$values[j].DealRate,
                            ItemCalculationMethod: outboundManagementData.$values[j].ItemCalculationMethod,
                            CalculationMethodId: outboundManagementData.$values[j].CalculationMethodId
                        };
                        outbounds.push(outbound);
                    }
                    analysisSettings.Outbounds = outbounds;
                }
                return WhS_Deal_SwapDealAnalysisAPIService.AnalyzeDeal(analysisSettings);
            }

            function loadResult(result) {
                var resultLoadDeferred = UtilsService.createPromiseDeferred();

                resultReadyDeferred.promise.then(function () {
                    var resultPayload = {
                        Result: result
                    };
                    VRUIUtilsService.callDirectiveLoad(resultAPI, resultPayload, resultLoadDeferred);
                });

                return resultLoadDeferred.promise;
            }
        }
    }]);