"use strict";

app.directive("vrCdrFraudanalysisExecutestrategyManual", ["VRUIUtilsService", "UtilsService", "StrategyAPIService", "VRValidationService", "CDRAnalysis_FA_PeriodAPIService", function (VRUIUtilsService, UtilsService, StrategyAPIService, VRValidationService, CDRAnalysis_FA_PeriodAPIService) {

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
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/ExecuteStrategyManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;
        var strategySelectorAPI;
        var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.onStrategySelectorReady = function (api) {
                strategySelectorAPI = api;
                strategySelectorReadyDeferred.resolve();
            };

            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.createProcessInputObjects = [];

            $scope.periods = [];

            $scope.selectedPeriodChanged = function () {

                if ($scope.selectedPeriod != undefined) {
                    $scope.isLoadingData = true;
                    loadStrategySelector($scope.selectedPeriod.Id);
                }
            }

            defineAPI();
        }

        function defineAPI() {


            var api = {};
            api.getData = function () {

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
                var loadPeriods = CDRAnalysis_FA_PeriodAPIService.GetPeriods().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.periods.push(itm);
                    });
                })
                promises.push(loadPeriods);
                return UtilsService.waitMultiplePromises(promises);
            }



            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function createProcessInputObjects(fromDate, toDate) {
            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: strategySelectorAPI.getSelectedIds(),
                    PeriodId: $scope.selectedPeriod.Id,
                    FromDate: new Date(fromDate),
                    ToDate: new Date(toDate),
                    IncludeWhiteList: $scope.includeWhiteList
                }
            });
        }


        function loadStrategySelector(periodId) {
            var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            var payload = {
            };
            var filter = {};
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
