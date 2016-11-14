(function (app) {

    'use strict';

    MeasurestyleStyleruleconditionRangeDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VR_Analytic_CompareOperatorEnum'];

    function MeasurestyleStyleruleconditionRangeDirective(UtilsService, VRUIUtilsService, VR_Analytic_CompareOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var measurestyleStyleruleconditionRange = new MeasurestyleStyleruleconditionRange($scope, ctrl, $attrs);
                measurestyleStyleruleconditionRange.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/StyleRuleConditions/Templates/RangeConditionTemplate.html"

        };
        function MeasurestyleStyleruleconditionRange($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.rangeStart = payload.RangeStart;
                        $scope.scopeModel.rangeEnd = payload.RangeEnd;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.StyleRuleConditions.RangeCondition, Vanrise.Analytic.MainExtensions ",
                        RangeStart: $scope.scopeModel.rangeStart,
                        RangeEnd: $scope.scopeModel.rangeEnd
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticMeasurestyleStyleruleconditionRange', MeasurestyleStyleruleconditionRangeDirective);

})(app);