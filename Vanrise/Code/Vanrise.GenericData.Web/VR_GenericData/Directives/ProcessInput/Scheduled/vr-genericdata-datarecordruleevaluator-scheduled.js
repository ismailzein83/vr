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

        var datarecordruleevaluatordefinitionSelectorAPI;
        var datarecordruleevaluatordefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var timePeriodSelectorAPI;
        var timePeriodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.datarecordruleevaluatordefinitionSelectorReady = function (api) {
                datarecordruleevaluatordefinitionSelectorAPI = api;
                datarecordruleevaluatordefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTimePeriodSelectorReady = function (api) {
                timePeriodSelectorAPI = api;
                timePeriodSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];

                promises.push(loadDataRecordRuleEvaluatorDefinitionSelector());

                function loadDataRecordRuleEvaluatorDefinitionSelector() {
                    var datarecordruleevaluatordefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    datarecordruleevaluatordefinitionSelectorReadyDeferred.promise.then(function () {
                        var dataRecordRuleEvaluatorPayload;
                        if (payload != undefined && payload.data != undefined)
                            dataRecordRuleEvaluatorPayload = {
                                selectedIds: payload.data.DataRecordRuleEvaluatorDefinitionId
                            };
                        VRUIUtilsService.callDirectiveLoad(datarecordruleevaluatordefinitionSelectorAPI, dataRecordRuleEvaluatorPayload, datarecordruleevaluatordefinitionSelectorLoadDeferred);
                    });

                    return datarecordruleevaluatordefinitionSelectorLoadDeferred.promise;
                }

                promises.push(loadTimePeriodSelector());

                function loadTimePeriodSelector() {
                    var timePeriodSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    timePeriodSelectorReadyDeferred.promise.then(function () {
                        var timePeriodPayload;
                        if (payload != undefined && payload.rawExpressions != undefined && payload.rawExpressions.VRTimePeriod != undefined)
                            timePeriodPayload = {
                                timePeriod : payload.rawExpressions.VRTimePeriod
                            };
                        VRUIUtilsService.callDirectiveLoad(timePeriodSelectorAPI, timePeriodPayload, timePeriodSelectorLoadDeferred);
                    });

                    return timePeriodSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getExpressionsData = function () {
                return { "ScheduleTime": "ScheduleTime", "VRTimePeriod": timePeriodSelectorAPI.getData() };
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.Notification.Arguments.DataRecordRuleEvaluatorProcessInput, Vanrise.GenericData.Notification.Arguments",
                    DataRecordRuleEvaluatorDefinitionId: datarecordruleevaluatordefinitionSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);
