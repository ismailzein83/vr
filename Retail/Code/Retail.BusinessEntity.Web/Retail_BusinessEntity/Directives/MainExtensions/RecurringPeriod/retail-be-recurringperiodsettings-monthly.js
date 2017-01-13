'use strict';

app.directive('retailBeRecurringperiodsettingsMonthly', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recurringPeriod = new RecurringPeriodMonthly($scope, ctrl, $attrs);
                recurringPeriod.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/RecurringPeriod/Templates/MonthlyPeriodTemplate.html'
        };

        function RecurringPeriodMonthly($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.DayOfMonth = payload.DayOfMonth;
                    }
                };
                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.RecurringPeriods.MonthlyRecurringPeriodSettings, Retail.BusinessEntity.MainExtensions",
                        DayOfMonth: $scope.scopeModel.DayOfMonth
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
