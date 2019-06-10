'use strict';

app.directive('vrWhsDealSwapdealanalysis', ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_SwapDealAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Deal_SwapDealAPIService) {
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
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisTemplate.html'
        };

        function SwapDealAnalysis($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var settingsAPI;
            var settingsReadyDeferred = UtilsService.createPromiseDeferred();
            var settingsLoadDeferred = UtilsService.createPromiseDeferred();

            var inboundManagementAPI;
            var inboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

            var outboundManagementAPI;
            var outboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

            var swapDealSettings;
            var swapDealSettingsLoadDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSettingsReady = function (api) {
                    settingsAPI = api;
                    settingsReadyDeferred.resolve();
                };

                $scope.scopeModel.onInboundManagementReady = function (api) {
                    inboundManagementAPI = api;
                    inboundManagementReadyDeferred.resolve();
                };

                $scope.scopeModel.onOutboundManagementReady = function (api) {
                    outboundManagementAPI = api;
                    outboundManagementReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {

                    }
                    loadAllControls();
                };

                api.getData = function () {

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
                    };
                    VRUIUtilsService.callDirectiveLoad(inboundManagementAPI, inboundManagementPayload, inboundManagementLoadDeferred);
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
                    };
                    VRUIUtilsService.callDirectiveLoad(outboundManagementAPI, outboundManagementPayload, outboundManagementLoadDeferred);
                });

                return outboundManagementLoadDeferred.promise;
            }
            function setSellingNumberPlanId(sellingNumberPlanId) {
                inboundManagementAPI.setSellingNumberPlanId(sellingNumberPlanId);
            }
            function clearAnalysis() {
                inboundManagementAPI.clear();
                outboundManagementAPI.clear();
            }
            function clearResult() {
            }
        }
    }]);