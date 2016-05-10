(function (app) {

    'use strict';

    HistoryAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Analytic_AnalyticService'];

    function HistoryAnalyticReportDirective(UtilsService, VRUIUtilsService, Analytic_AnalyticService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var historyAnalyticReport = new HistoryAnalyticReport($scope, ctrl, $attrs);
                historyAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/History/Templates/HistoryAnalyticReportRuntimeTemplates.html"

        };
        function HistoryAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            function initializeController() {
                $scope.scopeModel = {};

               

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   

                };

              
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }
        }
    }
    app.directive('vrAnalyticAnalyticreportHistoryRuntime', HistoryAnalyticReportDirective);
})(app);