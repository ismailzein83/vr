(function (app) {

    'use strict';

    DAProfCalcAnalysisPeriod.$inject = ['VR_Analytic_DAProfCalcTimeUnitEnum', 'UtilsService'];

    function DAProfCalcAnalysisPeriod(VR_Analytic_DAProfCalcTimeUnitEnum, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                label: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                if (ctrl.label == undefined)
                    $scope.label = 'Analysis Period';
                else
                    $scope.label = ctrl.label;

                var ctor = new Ctor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcAnalysisPeriod.html'
        };

        function Ctor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.timeUnits = UtilsService.getArrayEnum(VR_Analytic_DAProfCalcTimeUnitEnum);

                    if (payload != undefined) {
                        if (payload.DAProfCalcAnalysisPeriod != undefined) {
                            $scope.analysisPeriodTimeBack = payload.DAProfCalcAnalysisPeriod.AnalysisPeriodTimeBack;
                            $scope.analysisPeriodTimeUnit = UtilsService.getItemByVal($scope.timeUnits, payload.DAProfCalcAnalysisPeriod.AnalysisPeriodTimeUnit, 'value');
                        }
                        else if (payload.selectDefault) {
                            $scope.analysisPeriodTimeUnit = VR_Analytic_DAProfCalcTimeUnitEnum.Minutes;
                        }
                    }
                };

                api.getData = function () {
                    if ($scope.analysisPeriodTimeBack != undefined && $scope.analysisPeriodTimeUnit != undefined) {
                        return {
                            AnalysisPeriodTimeBack: $scope.analysisPeriodTimeBack,
                            AnalysisPeriodTimeUnit: $scope.analysisPeriodTimeUnit.value
                        };
                    }
                    else {
                        return undefined;
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcAnalysisperiod', DAProfCalcAnalysisPeriod);

})(app);