"use strict";

app.directive("vrAnalyticDaprofcalcGeneratealertprocesstask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_Notification_VRAlertRuleTypeAPIService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VRNotificationService', 'VR_Analytic_DAProfCalcTimeUnitEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, VR_Notification_VRAlertRuleTypeAPIService, VR_Analytic_DataAnalysisDefinitionAPIService, VRNotificationService, VR_Analytic_DAProfCalcTimeUnitEnum) {
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
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/ScheduleTask/Templates/GenerateAlertProcessTaskTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onalertRuleTypeSelectorSelectionChangedDeferred;

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var minDataAnalysisPeriodAPI;
            var minDataAnalysisPeriodReadyDeferred = UtilsService.createPromiseDeferred();

            var maxDataAnalysisPeriodAPI;
            var maxDataAnalysisPeriodReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.showChunkTimeSelector = false;

                $scope.onAlertRuleTypeSelectorReady = function (api) {
                    alertRuleTypeSelectorAPI = api;
                    alertRuleTypeSelectorReadyDeferred.resolve();
                };

                $scope.onAlertRuleTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        if (onalertRuleTypeSelectorSelectionChangedDeferred != undefined) {
                            onalertRuleTypeSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            $scope.isLoading = true;
                            VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(selectedItem.VRAlertRuleTypeId).then(function (alertRuleType) {
                                VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(alertRuleType.Settings.DataAnalysisDefinitionId).then(function (dataAnalysisDefintion) {
                                    $scope.showChunkTimeSelector = dataAnalysisDefintion.Settings.UseChunkTime;
                                    if ($scope.showChunkTimeSelector) {
                                        chunkTimeSelectorReadyDeferred.promise.then(function () {
                                            VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, undefined, undefined);
                                        });
                                    }

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
                    }
                };

                $scope.onChunkTimeSelectorReady = function (api) {
                    chunkTimeSelectorAPI = api;
                    chunkTimeSelectorReadyDeferred.resolve();
                };

                $scope.onMinDataAnalysisPeriodDirectiveReady = function (api) {
                    minDataAnalysisPeriodAPI = api;
                    minDataAnalysisPeriodReadyDeferred.resolve();
                };

                $scope.onMaxDataAnalysisPeriodDirectiveReady = function (api) {
                    maxDataAnalysisPeriodAPI = api;
                    maxDataAnalysisPeriodReadyDeferred.resolve();
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

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var alertRuleTypeId;
                    var chunkTime;

                    if (payload != undefined) {

                        if (payload.data != undefined) {
                            alertRuleTypeId = payload.data.AlertRuleTypeId;
                            chunkTime = payload.data.ChunkTime;
                        }
                    }

                    var promises = [];
                    if (chunkTime != undefined) {

                        ruRuleTypeSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        $scope.showChunkTimeSelector = true;
                        var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([onalertRuleTypeSelectorSelectionChangedDeferred.promise, chunkTimeSelectorReadyDeferred.promise]).then(function () {
                            onalertRuleTypeSelectorSelectionChangedDeferred = undefined;

                            var chunkTimeSelectorPayload = {
                                selectedIds: chunkTime
                            };
                            VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                        });
                        promises.push(chunkTimeSelectorLoadDeferred.promise);
                    }

                    var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    alertRuleTypeSelectorReadyDeferred.promise.then(function () {
                        var alertRuleTypePayload = { filter: { Filters: [] }, selectIfSingleItem: true };
                        alertRuleTypePayload.filter.Filters.push({ $type: "Vanrise.Analytic.Business.DAProfCalcVRAlertRuleTypeFilter, Vanrise.Analytic.Business" });
                        if (alertRuleTypeId != undefined) {
                            alertRuleTypePayload.selectedIds = alertRuleTypeId;
                        }
                        VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypePayload, alertRuleTypeSelectorLoadDeferred);
                    });
                    promises.push(alertRuleTypeSelectorLoadDeferred.promise);


                    var minDataAnalysisPeriodLoadPromise = getMinDataAnalysisPeriodLoadPromise();
                    promises.push(minDataAnalysisPeriodLoadPromise);

                    var maxDataAnalysisPeriodLoadPromise = getMaxDataAnalysisPeriodLoadPromise();
                    promises.push(maxDataAnalysisPeriodLoadPromise);

                    function getMinDataAnalysisPeriodLoadPromise() {
                        var dataAnalysisPeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        minDataAnalysisPeriodReadyDeferred.promise.then(function () {
                            var minDataAnalysisPeriodPayload;
                            if (payload != undefined && payload.data != undefined)
                                minDataAnalysisPeriodPayload = { DAProfCalcAnalysisPeriod: payload.data.MinDAProfCalcAnalysisPeriod };

                            VRUIUtilsService.callDirectiveLoad(minDataAnalysisPeriodAPI, minDataAnalysisPeriodPayload, dataAnalysisPeriodDirectiveLoadDeferred);
                        });

                        return dataAnalysisPeriodDirectiveLoadDeferred.promise;
                    }

                    function getMaxDataAnalysisPeriodLoadPromise() {
                        var dataAnalysisPeriodDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        maxDataAnalysisPeriodReadyDeferred.promise.then(function () {
                            var maxDataAnalysisPeriodPayload;
                            if (payload != undefined && payload.data != undefined)
                                maxDataAnalysisPeriodPayload = { DAProfCalcAnalysisPeriod: payload.data.MaxDAProfCalcAnalysisPeriod };

                            VRUIUtilsService.callDirectiveLoad(maxDataAnalysisPeriodAPI, maxDataAnalysisPeriodPayload, dataAnalysisPeriodDirectiveLoadDeferred);
                        });

                        return dataAnalysisPeriodDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getExpressionsData = function () {
                    return null;
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput, Vanrise.Analytic.BP.Arguments",
                        AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                        MinDAProfCalcAnalysisPeriod: minDataAnalysisPeriodAPI.getData(),
                        MaxDAProfCalcAnalysisPeriod: maxDataAnalysisPeriodAPI.getData(),
                        ChunkTime: chunkTimeSelectorAPI != undefined ? chunkTimeSelectorAPI.getSelectedIds() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
