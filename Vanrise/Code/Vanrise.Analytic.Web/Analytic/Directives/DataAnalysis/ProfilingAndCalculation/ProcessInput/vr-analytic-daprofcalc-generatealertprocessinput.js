"use strict";

app.directive("vrAnalyticDaprofcalcGeneratealertprocessinput", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'DAProfCalcChunkTimeEnum', 'VR_Notification_VRAlertRuleTypeAPIService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VRNotificationService', 'VR_Analytic_DAProfCalcTimeUnitEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, DAProfCalcChunkTimeEnum, VR_Notification_VRAlertRuleTypeAPIService, VR_Analytic_DataAnalysisDefinitionAPIService, VRNotificationService, VR_Analytic_DAProfCalcTimeUnitEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/ProcessInput/Templates/DAProfCalcGenerateAlertProcessInputTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            $scope.scopeModel = {};

            $scope.showChunkTimeSelector = false;
            $scope.isLoading = false;

            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var minDataAnalysisPeriodAPI;
            var minDataAnalysisPeriodReadyDeferred = UtilsService.createPromiseDeferred();

            var maxDataAnalysisPeriodAPI;
            var maxDataAnalysisPeriodReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onAlertRuleTypeSelectorReady = function (api) {
                alertRuleTypeSelectorAPI = api;
                alertRuleTypeSelectorReadyDeferred.resolve();
            };

            $scope.onMinDataAnalysisPeriodDirectiveReady = function (api) {
                minDataAnalysisPeriodAPI = api;
                minDataAnalysisPeriodReadyDeferred.resolve();
            };

            $scope.onMaxDataAnalysisPeriodDirectiveReady = function (api) {
                maxDataAnalysisPeriodAPI = api;
                maxDataAnalysisPeriodReadyDeferred.resolve();
            };

            $scope.onAlertRuleTypeSelectionChanged = function (selectedItem) {
                if (selectedItem == undefined) {
                    $scope.showChunkTimeSelector = false;
                }
                else {
                    $scope.isLoading = true;
                    VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(selectedItem.VRAlertRuleTypeId).then(function (alertRuleType) {
                        VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(alertRuleType.Settings.DataAnalysisDefinitionId).then(function (dataAnalysisDefintion) {
                            $scope.showChunkTimeSelector = dataAnalysisDefintion.Settings.UseChunkTime;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.isLoading = false;
                        });

                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                }
            };

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            $scope.validateDataAnalysisPeriod = function () {
                $scope.timeUnits = UtilsService.getArrayEnum(VR_Analytic_DAProfCalcTimeUnitEnum);

                var maxAnalysisPeriod = maxDataAnalysisPeriodAPI.getData();
                var minAnalysisPeriod = minDataAnalysisPeriodAPI.getData();

                if (maxAnalysisPeriod == undefined || minAnalysisPeriod == undefined)
                    return null;

                var maxAnalysisPeriodTimeBack = maxAnalysisPeriod.AnalysisPeriodTimeBack;
                var maxAnalysisPeriodTimeUnit = UtilsService.getItemByVal($scope.timeUnits, maxAnalysisPeriod.AnalysisPeriodTimeUnit, 'value');
                var maxPeriodInMinutes = maxAnalysisPeriodTimeBack * maxAnalysisPeriodTimeUnit.ValueInMinutes;

                var minAnalysisPeriodTimeBack = minAnalysisPeriod.AnalysisPeriodTimeBack;
                var minAnalysisPeriodTimeUnit = UtilsService.getItemByVal($scope.timeUnits, minAnalysisPeriod.AnalysisPeriodTimeUnit, 'value');
                var minPeriodInMinutes = minAnalysisPeriodTimeBack * minAnalysisPeriodTimeUnit.ValueInMinutes;

                if (maxPeriodInMinutes > minPeriodInMinutes)
                    return null;

                return 'Min Analysis Period should be less than Max Analysis Period';
            };

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput, Vanrise.Analytic.BP.Arguments",
                            AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                            MinDAProfCalcAnalysisPeriod: minDataAnalysisPeriodAPI.getData(),
                            MaxDAProfCalcAnalysisPeriod: maxDataAnalysisPeriodAPI.getData(),
                            ChunkTime: $scope.scopeModel.selectedChunkTime != undefined ? $scope.scopeModel.selectedChunkTime.value : undefined
                        }
                    };
                };

                api.load = function (payload) {
                    $scope.chunkTimes = UtilsService.getArrayEnum(DAProfCalcChunkTimeEnum);

                    var promises = [];

                    var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    alertRuleTypeSelectorReadyDeferred.promise.then(function () {
                        var payload = { filter: { Filters: [] } };
                        payload.filter.Filters.push({ $type: "Vanrise.Analytic.Business.DAProfCalcVRAlertRuleTypeFilter, Vanrise.Analytic.Business" });
                        VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, payload, alertRuleTypeSelectorLoadDeferred);
                    });

                    promises.push(alertRuleTypeSelectorLoadDeferred.promise);

                    var minDataAnalysisPeriodLoadPromise = getMinDataAnalysisPeriodLoadPromise();
                    promises.push(minDataAnalysisPeriodLoadPromise);

                    var maxDataAnalysisPeriodLoadPromise = getMaxDataAnalysisPeriodLoadPromise();
                    promises.push(maxDataAnalysisPeriodLoadPromise);

                    function getMinDataAnalysisPeriodLoadPromise() {
                        var dataAnalysisPeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        minDataAnalysisPeriodReadyDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(minDataAnalysisPeriodAPI, undefined, dataAnalysisPeriodDirectiveLoadDeferred);
                        });

                        return dataAnalysisPeriodDirectiveLoadDeferred.promise;
                    };

                    function getMaxDataAnalysisPeriodLoadPromise() {
                        var dataAnalysisPeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        maxDataAnalysisPeriodReadyDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(maxDataAnalysisPeriodAPI, undefined, dataAnalysisPeriodDirectiveLoadDeferred);
                        });

                        return dataAnalysisPeriodDirectiveLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);