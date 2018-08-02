(function (app) {

    'use strict';

    VrreportgenerationFilterStandardruntimeDirective.$inject = ['PeriodEnum'];

    function VrreportgenerationFilterStandardruntimeDirective(PeriodEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrreportgenerationFilterStandardruntime = new VrreportgenerationFilterStandardruntime(ctrl, $scope, $attrs);
                vrreportgenerationFilterStandardruntime.initializeController();
            },
            controllerAs: 'Ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function VrreportgenerationFilterStandardruntime(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.selectedPeriod = PeriodEnum.CurrentMonth;
                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    if (ctrl.onReady != undefined) {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};
                api.load = function (payload) {
                    return directiveAPI.load(payload);
                };

                api.getData = function () {
                    return {
                            $type: 'Vanrise.Analytic.MainExtensions.StandardReportGenerationRuntimeFilter,Vanrise.Analytic.MainExtensions',
                            FromTime: $scope.scopeModal.fromDate,
                            ToTime: $scope.scopeModal.toDate
                        };
                   
                };
                return api;
            }
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-timerange on-ready="Ctrl.onDirectiveReady" width="1/3row" type="dateTime" from="scopeModal.fromDate" to="scopeModal.toDate" period="scopeModal.selectedPeriod" isrequired="true"></vr-timerange>';
        }
    }

    app.directive('vrAnalyticReportgenerationFilterStandardruntime', VrreportgenerationFilterStandardruntimeDirective);

})(app);