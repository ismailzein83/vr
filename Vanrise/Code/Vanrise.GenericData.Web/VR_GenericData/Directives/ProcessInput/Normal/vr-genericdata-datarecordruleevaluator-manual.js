"use strict";

app.directive("vrGenericdataDatarecordruleevaluatorManual", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var dataRecordRuleEvaluatorManualDirective = new DataRecordRuleEvaluatorManualDirective($scope, ctrl);
            dataRecordRuleEvaluatorManualDirective.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/ProcessInput/Normal/Templates/GenericDataDataRecordRuleEvaluatorManualTemplate.html"
    };

    function DataRecordRuleEvaluatorManualDirective($scope, ctrl) {
        this.initializeController = initializeController;

        var datarecordruleevaluatordefinitionSelectorAPI;
        var datarecordruleevaluatordefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            
            $scope.scopeModel.datarecordruleevaluatordefinitionSelectorReady = function (api) {
                datarecordruleevaluatordefinitionSelectorAPI = api;
                datarecordruleevaluatordefinitionSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    $scope.scopeModel.fromDate = payload.FromDate;
                    $scope.scopeModel.toDate = payload.ToDate;
                }

                promises.push(loadDataRecordRuleEvaluatorDefinitionSelector());

                function loadDataRecordRuleEvaluatorDefinitionSelector() {
                    var datarecordruleevaluatordefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    datarecordruleevaluatordefinitionSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(datarecordruleevaluatordefinitionSelectorAPI, undefined, datarecordruleevaluatordefinitionSelectorLoadDeferred);
                    });

                    return datarecordruleevaluatordefinitionSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "Vanrise.GenericData.Notification.Arguments.DataRecordRuleEvaluatorProcessInput, Vanrise.GenericData.Notification.Arguments",
                        DataRecordRuleEvaluatorDefinitionId: datarecordruleevaluatordefinitionSelectorAPI.getSelectedIds(),
                        FromDate: $scope.scopeModel.fromDate,
                        ToDate: $scope.scopeModel.toDate
                    }
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }

    return directiveDefinitionObject;
}]);
