"use strict";

app.directive("vrCdrFraudanalysisNumberprofilingManual", [ "StrategyAPIService", "HourEnum", "VRValidationService", function ( StrategyAPIService, HourEnum, VRValidationService) {
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
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/NumberProfilingManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;
        

        function initializeController() {

            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.createProcessInputObjects = [];

            $scope.periods = [];
            $scope.selectedPeriod;


            $scope.hours = [];
            angular.forEach(HourEnum, function (itm) {
                $scope.hours.push({ id: itm.id, name: itm.name })
            });

            $scope.gapBetweenConsecutiveCalls = 10;
            $scope.gapBetweenFailedConsecutiveCalls = 10;
            $scope.maxLowDurationCall = 8;
            $scope.minCountofCallsinActiveHour = 5;
            $scope.selectedPeakHours = [];

            angular.forEach($scope.hours, function (itm) {
                if (itm.id >= 12 && itm.id <= 17)
                    $scope.selectedPeakHours.push(itm);
            });


            defineAPI();
        }

        function defineAPI() {
          
            var api = {};
            api.getData = function () {

                $scope.PeakHoursIds = [];
                angular.forEach($scope.selectedPeakHours, function (itm) {
                    $scope.PeakHoursIds.push(itm.id)
                });

                var runningDate = new Date($scope.fromDate);

                $scope.createProcessInputObjects.length = 0;


                if ($scope.selectedPeriod.Id == 1)//Hourly
                {
                    while (runningDate < $scope.toDate) {
                        var fromDate = new Date(runningDate);
                        var toDate = new Date(runningDate.setHours(runningDate.getHours() + 1));



                        $scope.createProcessInputObjects.push({
                            InputArguments: {
                                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                                FromDate: new Date(fromDate),
                                ToDate: new Date(toDate),
                                PeriodId: $scope.selectedPeriod.Id,
                                IncludeWhiteList: $scope.includeWhiteList,
                                Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                            }
                        });
                        runningDate = new Date(toDate);
                    }

                }

                else if ($scope.selectedPeriod.Id == 2) //Daily
                {
                    while (runningDate < $scope.toDate) {
                        var fromDate = new Date(runningDate);
                        var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));

                        $scope.createProcessInputObjects.push({
                            InputArguments: {
                                $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                                FromDate: new Date(fromDate),
                                ToDate: new Date(toDate),
                                PeriodId: $scope.selectedPeriod.Id,
                                IncludeWhiteList: $scope.includeWhiteList,
                                Parameters: { GapBetweenConsecutiveCalls: $scope.gapBetweenConsecutiveCalls, GapBetweenFailedConsecutiveCalls: $scope.gapBetweenFailedConsecutiveCalls, MaxLowDurationCall: $scope.maxLowDurationCall, MinimumCountofCallsinActiveHour: $scope.minCountofCallsinActiveHour, PeakHoursIds: $scope.PeakHoursIds }
                            }
                        });

                        runningDate = new Date(toDate);
                    }


                }

                return $scope.createProcessInputObjects;

            };
            
            api.load = function (payload) {
               
                 return StrategyAPIService.GetPeriods().then(function (response) {
                    $scope.periods.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.periods.push(itm);
                    });  
                });

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
