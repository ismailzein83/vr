"use strict";

app.directive("vrAnalyticDaprofcalcGeneratealertprocesstask", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, ReprocessChunkTimeEnum) {
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

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onAlertRuleTypeSelectorReady = function (api) {
                    alertRuleTypeSelectorAPI = api;
                    alertRuleTypeSelectorReadyDeferred.resolve();
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

                    var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    chunkTimeSelectorReadyDeferred.promise.then(function () {
                        var chunkTimeSelectorPayload;
                        if (chunkTime != undefined) {
                            chunkTimeSelectorPayload = {
                                selectedIds: chunkTime
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                    });
                    promises.push(chunkTimeSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime", "DaysBack": $scope.daysBack, "NumberOfDays": $scope.numberOfDays };
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput, Vanrise.Analytic.BP.Arguments",
                        AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                        ChunkTime: chunkTimeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
