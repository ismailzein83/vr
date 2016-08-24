'use strict';

app.directive('retailBeRecurringperiodWeekly', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recurringPeriod = new RecurringPeriodWeekly($scope, ctrl, $attrs);
                recurringPeriod.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringPeriod/Templates/WeeklyPeriodTemplate .html'
        };

        function RecurringPeriodWeekly($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.DayOfWeek = payload.DayOfWeek;
                    }
                };
                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.RecurringPeriods.WeeklyRecurringPeriodSettings, Retail.BusinessEntity.MainExtensions",
                        DayOfWeek: $scope.scopeModel.DayOfWeek
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
