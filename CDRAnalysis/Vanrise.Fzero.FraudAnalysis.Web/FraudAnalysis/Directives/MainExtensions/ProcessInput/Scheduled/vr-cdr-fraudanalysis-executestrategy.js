"use strict";

app.directive("vrCdrFraudanalysisExecutestrategy", ["UtilsService", "VRUIUtilsService", "StrategyAPIService", "CDRAnalysis_FA_PeriodAPIService", function (UtilsService, VRUIUtilsService, StrategyAPIService, CDRAnalysis_FA_PeriodAPIService) {
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
        var selectedStrategyIds;

        function initializeController() {

            $scope.onStrategySelectorReady = function (api) {
                strategySelectorAPI = api;
                strategySelectorReadyDeferred.resolve();
            };

            $scope.strategies = [];
            $scope.periods = [];

            $scope.selectedPeriodChanged = function () {
                if ($scope.selectedPeriod != undefined) {
                    loadStrategySelector($scope.selectedPeriod.Id);
                }
            }
            defineAPI();
        }

        function defineAPI() {
            
            var api = {};
            api.getData = function () {
                console.log($scope.selectedPeriod.Id)
                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: strategySelectorAPI.getSelectedIds(),
                    PeriodId: $scope.selectedPeriod.Id,
                    IncludeWhiteList: false
                };

            };

            api.getExpressionsData = function () {

                return { "ScheduleTime": "ScheduleTime" };

            };

            api.load = function (payload) {
                var promises = [];
                var data;
                if (payload != undefined && payload.data != undefined)
                    data = payload.data;
                var loadPeriods = CDRAnalysis_FA_PeriodAPIService.GetPeriods().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.periods.push(itm);
                    });

                    if (data != undefined && data.StrategyIds != undefined) {
                        selectedStrategyIds = data.StrategyIds;
                        $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, data.PeriodId, "Id");
                    }

                })
                promises.push(loadPeriods);
                return UtilsService.waitMultiplePromises(promises);

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }


        function loadStrategySelector(periodId) {
            $scope.isLoadingData = true;
            var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            var payload = {
            };

            if (selectedStrategyIds != undefined)
                payload.selectedIds = selectedStrategyIds;

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
