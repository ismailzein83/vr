(function (app) {

    'use strict';

    LastPeriodFilterDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function LastPeriodFilterDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var lastPeriodFilter = new LastPeriodFilter($scope, ctrl, $attrs);
                lastPeriodFilter.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/TimeRangeFilters/Templates/LastPeriodFilterTemplate.html"

        };
        function LastPeriodFilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateTimeOffset = function (value) {
                    return UtilsService.validateTimeOffset(value);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.periodLength = payload.timeRangeFilter.PeriodLength;
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.TimeRangeFilters.LastPeriodFilter, Vanrise.Analytic.MainExtensions",
                        PeriodLength: $scope.scopeModel.periodLength
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticLastperiodfilter', LastPeriodFilterDirective);

})(app);