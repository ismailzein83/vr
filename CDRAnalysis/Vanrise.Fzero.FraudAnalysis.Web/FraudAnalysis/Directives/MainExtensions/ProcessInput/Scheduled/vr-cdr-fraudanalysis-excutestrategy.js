"use strict";

app.directive("vrCdrFraudanalysisExcutestrategy", ["UtilsService", "VRUIUtilsService", "StrategyAPIService", function (UtilsService, VRUIUtilsService, StrategyAPIService) {
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
        templateUrl: function (element, attrs) {

            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Scheduled/Templates/ExcuteStrategyTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {

            $scope.strategies = [];
            $scope.selectedStrategies = [];
            $scope.selectedStrategyIds = [];
            $scope.periods = [];
            $scope.selectedPeriod;
           
            var firstTimeToload = false;
            $scope.selectedPeriodChanged = function () {
                

                if ($scope.selectedPeriod != undefined && firstTimeToload == true) {
                    $scope.strategies.length = 0;
                    $scope.selectedStrategies.length = 0;
                    loadStrategies($scope.selectedPeriod.Id);
                }
               
            }
            var api = {};
            api.getData = function () {
                angular.forEach($scope.selectedStrategies, function (itm) {
                    $scope.selectedStrategyIds.push(itm.id);
                });

                return {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: $scope.selectedStrategyIds,
                    OverridePrevious: false,
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
                var loadPeriods =   StrategyAPIService.GetPeriods().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.periods.push(itm);
                    });
                })
                promises.push(loadPeriods);
                var strategy;
                if (data != undefined && data.StrategyIds!= undefined) {
                    var firstStrategyLoad = loadStrategies(0).then(function () {
                        var promises = [];
                        strategy = UtilsService.getItemByVal($scope.strategies, data.StrategyIds[0], "id");

                        if (strategy != null) {
                            $scope.selectedPeriod = UtilsService.getItemByVal($scope.periods, strategy.periodId, "Id");
                           
                        }
                        if ($scope.selectedPeriod != undefined) {
                            loadStrategies($scope.selectedPeriod.Id).then(function () {
                                firstTimeToload = true;
                                angular.forEach(data.StrategyIds, function (strategyId) {
                                    $scope.selectedStrategies.push(UtilsService.getItemByVal($scope.strategies, strategyId, "id"));
                                });
                            })
                        }
                        else {
                            firstTimeToload = true;

                        }
                       
                       
                    })
                    promises.push(firstStrategyLoad)
                }
                else {
                    firstTimeToload = true;
                }
                
                return UtilsService.waitMultiplePromises(promises);

            }

            

            if (ctrl.onReady != null)
                ctrl.onReady(api);


            function loadStrategies(periodId ) {            

                return StrategyAPIService.GetStrategies(periodId, true).then(function (response) {
                    $scope.strategies.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });
                    });
                });
            }
        }
    }

    return directiveDefinitionObject;
}]);
