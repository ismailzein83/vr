(function (app) {

    'use strict';

    CountAggregateDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function CountAggregateDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var countAggregate = new CountAggregate($scope, ctrl, $attrs);
                countAggregate.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/DARecordAggregates/Templates/CountAggregateTemplate.html"

        };
        function CountAggregate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.DARecordAggregates.CountAggregate, Vanrise.Analytic.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticCountaggregate', CountAggregateDirective);

})(app);