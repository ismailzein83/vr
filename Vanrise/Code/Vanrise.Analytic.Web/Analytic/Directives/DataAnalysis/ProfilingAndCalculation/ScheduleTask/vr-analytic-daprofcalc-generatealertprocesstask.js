"use strict";

app.directive("vrAnalyticDaprofcalcGeneratealertprocesstask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VR_Notification_VRAlertRuleTypeAPIService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRValidationService, VR_Notification_VRAlertRuleTypeAPIService, VR_Analytic_DataAnalysisDefinitionAPIService, VRNotificationService) {
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
                                    console.log(1);
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

                $scope.isValid = function () {
                    if ($scope.daysBack != undefined && $scope.numberOfDays != undefined) {
                        if (parseFloat($scope.numberOfDays) > parseFloat($scope.daysBack)) {
                            return 'Number Of Days should be less than or equal to Days Back.';
                        }
                    }
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var alertRuleTypeId;
                    var chunkTime;

                    if (payload != undefined) {
                        if (payload.rawExpressions != undefined) {
                            $scope.daysBack = payload.rawExpressions.DaysBack;
                            $scope.numberOfDays = payload.rawExpressions.NumberOfDays;
                        }

                        if (payload.data != undefined) {
                            alertRuleTypeId = payload.data.AlertRuleTypeId;
                            chunkTime = payload.data.ChunkTime;
                        }
                    }

                    var promises = [];
                    console.log(chunkTime);
                    if (chunkTime != undefined) {

                        onalertRuleTypeSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

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
                        var alertRuleTypePayload = { filter: { Filters: [] } };
                        alertRuleTypePayload.filter.Filters.push({ $type: "Vanrise.Analytic.Business.DAProfCalcVRAlertRuleTypeFilter, Vanrise.Analytic.Business" });
                        if (alertRuleTypeId != undefined) {
                            alertRuleTypePayload.selectedIds = alertRuleTypeId;
                        }
                        VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypePayload, alertRuleTypeSelectorLoadDeferred);
                    });
                    promises.push(alertRuleTypeSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime", "DaysBack": $scope.daysBack, "NumberOfDays": $scope.numberOfDays };
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput, Vanrise.Analytic.BP.Arguments",
                        AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                        ChunkTime: chunkTimeSelectorAPI != undefined ? chunkTimeSelectorAPI.getSelectedIds() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
