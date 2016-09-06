(function (app) {

    'use strict';

    PreviousPeriodFilterDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function PreviousPeriodFilterDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var previousPeriodFilter = new PreviousPeriodFilter($scope, ctrl, $attrs);
                previousPeriodFilter.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/TimeRangeFilters/Templates/PreviousPeriodFilterTemplate.html"

        };
        function PreviousPeriodFilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.validateTimeOffset = function (value) {
                    return UtilsService.validateTimeOffset(value);
                }

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.periodLength = payload.timeRangeFilter.PeriodLength;
                        $scope.scopeModel.periodDistanceFromNow = payload.timeRangeFilter.PeriodDistanceFromNow;
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.TimeRangeFilters.PreviousPeriodFilter, Vanrise.Analytic.MainExtensions",
                        PeriodLength: $scope.scopeModel.periodLength,
                        PeriodDistanceFromNow: $scope.scopeModel.periodDistanceFromNow
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticPreviousperiodfilter', PreviousPeriodFilterDirective);

})(app);