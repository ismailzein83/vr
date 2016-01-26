"use strict";

app.directive("vrCdrFraudanalysisExcutestrategyManual", ["UtilsService", "StrategyAPIService", "VRValidationService", function (UtilsService, StrategyAPIService, VRValidationService ) {
  
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
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/ExcuteStrategyManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        var firstTimeToload = false;
        function initializeController() {

            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.createProcessInputObjects = [];

            $scope.strategies = [];
            $scope.selectedStrategies = [];
            $scope.selectedStrategyIds = [];
            $scope.periods = [];
          
            $scope.selectedPeriodChanged = function () {
              
                if ($scope.selectedPeriod != undefined && firstTimeToload == true) {
                    $scope.isLoadingData = true;
                    $scope.strategies.length = 0;
                    $scope.selectedStrategies.length = 0;
                    loadStrategies($scope.selectedPeriod.Id);
                }
            }

            defineAPI();
        }

        function defineAPI() {

           
            var api = {};
            api.getData = function () {

                angular.forEach($scope.selectedStrategies, function (itm) {
                    $scope.selectedStrategyIds.push(itm.id);
                });


                var runningDate = new Date($scope.fromDate);

                $scope.createProcessInputObjects.length = 0;


                if ($scope.selectedPeriod.Id == 1)//Hourly
                {
                    while (runningDate < $scope.toDate) {
                        var fromDate = new Date(runningDate);
                        var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));
                        createProcessInputObjects(fromDate, toDate);
                        runningDate = new Date(toDate);
                    }

                }

                else if ($scope.selectedPeriod.Id == 2) //Daily
                {
                    while (runningDate < $scope.toDate) {
                        var fromDate = new Date(runningDate);
                        var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));
                        createProcessInputObjects(fromDate, toDate);
                        runningDate = new Date(toDate);
                    }


                }

                return $scope.createProcessInputObjects;

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
                firstTimeToload = true;
                return UtilsService.waitMultiplePromises(promises);

            }

            

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function createProcessInputObjects(fromDate, toDate) {
            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: $scope.selectedStrategyIds,
                    FromDate: new Date(fromDate),
                    ToDate: new Date(toDate),
                    IncludeWhiteList: $scope.includeWhiteList
                }
            });
        }

        function loadStrategies(periodId) {
            return StrategyAPIService.GetStrategies(periodId, true).then(function (response) {
                $scope.strategies.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.strategies.push({ id: itm.Id, name: itm.Name, periodId: itm.PeriodId });
                });
            }).finally(function () {
                $scope.isLoadingData = false;
            });
        }
    }

    return directiveDefinitionObject;
}]);
