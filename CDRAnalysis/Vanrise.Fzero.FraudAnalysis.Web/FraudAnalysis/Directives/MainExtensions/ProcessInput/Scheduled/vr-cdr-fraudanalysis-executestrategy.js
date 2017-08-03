"use strict";

app.directive("vrCdrFraudanalysisExecutestrategy", ["UtilsService", "VRUIUtilsService", "StrategyAPIService",
    function (UtilsService, VRUIUtilsService, StrategyAPIService) {
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
            }
        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Scheduled/Templates/ExecuteStrategyTemplate.html"
    };


    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        var strategySelectorAPI;
        var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var execStrategyEntity;

        function initializeController() {

            $scope.onStrategySelectorReady = function (api) {
                strategySelectorAPI = api;
                strategySelectorReadyDeferred.resolve();
            };

            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };



            $scope.onPeriodSelectionChanged = function (selectedPeriod) {
                if (selectedPeriod != undefined) {
                    $scope.isLoadingData = true;
                    loadStrategySelector(selectedPeriod.Id);
                }
            };
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {

                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: strategySelectorAPI.getSelectedIds(),
                    PeriodId: periodSelectorAPI.getSelectedIds(),
                    IncludeWhiteList: false
                };

            };

            api.getExpressionsData = function () {

                return { "ScheduleTime": "ScheduleTime" };

            };

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined && payload.data != undefined) {
                    execStrategyEntity = payload.data;
                }

                promises.push(loadPeriodSelector());
                return UtilsService.waitMultiplePromises(promises);

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }


        function loadPeriodSelector() {

            var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            periodSelectorReadyDeferred.promise.then(function () {

                var payload = {
                };
                if (execStrategyEntity != undefined && execStrategyEntity.PeriodId != undefined)
                    payload.selectedIds = execStrategyEntity.PeriodId;

                VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, payload, periodSelectorLoadDeferred);
            });

            return periodSelectorLoadDeferred.promise;
        }


        function loadStrategySelector(periodId) {
            $scope.isLoadingData = true;
            var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            var payload = {
            };

            if (execStrategyEntity != undefined && execStrategyEntity.StrategyIds != undefined)
                payload.selectedIds = execStrategyEntity.StrategyIds;

            var filter = {};
            if (periodId != undefined)
                filter.PeriodId = periodId;

            payload.filter = filter;

            strategySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, payload, strategySelectorLoadDeferred);
            }).finally(function () { $scope.isLoadingData = false; });

            return strategySelectorLoadDeferred.promise;
        }

    }

    return directiveDefinitionObject;
}]);
