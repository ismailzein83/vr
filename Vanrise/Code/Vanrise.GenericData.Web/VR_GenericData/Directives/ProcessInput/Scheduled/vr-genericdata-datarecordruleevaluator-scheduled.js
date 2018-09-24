"use strict";

app.directive("vrGenericdataDatarecordruleevaluatorSchedualed", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var dataRecordRuleEvaluatorScheduledDirective = new DataRecordRuleEvaluatorScheduledDirective($scope, ctrl);
            dataRecordRuleEvaluatorScheduledDirective.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/ProcessInput/Scheduled/Templates/GenericDataDataRecordRuleEvaluatorScheduledTemplate.html"
    };

    function DataRecordRuleEvaluatorScheduledDirective($scope, ctrl) {
        this.initializeController = initializeController;

        var dataRecordRuleEvaluatorDefinitionSelectorAPI;
        var dataRecordRuleEvaluatorDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred;

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.dataRecordRuleEvaluatorDefinitionSelectorReady = function (api) {
                dataRecordRuleEvaluatorDefinitionSelectorAPI = api;
                dataRecordRuleEvaluatorDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;

                var setLoader = function (value) {
                    $scope.scopeModel.isTimePeriodLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, timePeriodSelectorAPI, undefined, setLoader, timePeriodSelectorReadyDeferred);
            };

            $scope.scopeModel.onDataRecordRuleEvaluatorSelectionChanged = function (value) {
                if (value != undefined) {
                    if (dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred != undefined) {
                        dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred.resolve();
                    }
                }
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined && payload.rawExpressions != undefined && payload.rawExpressions.VRTimePeriod != undefined) {
                    timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                    dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadTimePeriodSelector());
                }

                promises.push(loadDataRecordRuleEvaluatorDefinitionSelector());

                function loadDataRecordRuleEvaluatorDefinitionSelector() {
                    var dataRecordRuleEvaluatorDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordRuleEvaluatorDefinitionSelectorReadyDeferred.promise.then(function () {
                        var dataRecordRuleEvaluatorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Vanrise.GenericData.Notification.DataRecordRuleEvaluatorDefinitionStartInstanceFilter ,Vanrise.GenericData.Notification"
                                }]
                            }
                        };
                        if (payload != undefined && payload.data != undefined)
                            dataRecordRuleEvaluatorPayload.selectedIds = payload.data.DataRecordRuleEvaluatorDefinitionId;
                        VRUIUtilsService.callDirectiveLoad(dataRecordRuleEvaluatorDefinitionSelectorAPI, dataRecordRuleEvaluatorPayload, dataRecordRuleEvaluatorDefinitionSelectorLoadDeferred);
                    });

                    return dataRecordRuleEvaluatorDefinitionSelectorLoadDeferred.promise;
                }

                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([timePeriodSelectorReadyDeferred.promise, dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred.promise]).then(function () {
                        timePeriodSelectorReadyDeferred = undefined;
                        var timePeriodPayload = { timePeriod: payload.rawExpressions.VRTimePeriod };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodPayload, timePeriodSelectorLoadDeferred);
                    });

                    return timePeriodSelectorLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    dataRecordRuleEvaluatorDefinitionSelectorSelectDeferred = undefined;
                });
            };

            api.getExpressionsData = function () {
                var obj = { "ScheduleTime": "ScheduleTime" };

                if (timePeriodSelectorAPI != undefined && !$scope.scopeModel.selectedEvaluator.AreDatesHardCoded)
                    obj.VRTimePeriod = timePeriodSelectorAPI.getData();

                return obj;
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.Notification.Arguments.DataRecordRuleEvaluatorProcessInput, Vanrise.GenericData.Notification.Arguments",
                    DataRecordRuleEvaluatorDefinitionId: dataRecordRuleEvaluatorDefinitionSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);
