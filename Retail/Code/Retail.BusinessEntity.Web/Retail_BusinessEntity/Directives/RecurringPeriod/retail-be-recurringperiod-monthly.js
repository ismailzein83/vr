'use strict';

app.directive('retailBeRecurringperiodMonthly', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recurringPeriod = new RecurringPeriodMonthly($scope, ctrl, $attrs);
                statusDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringPeriod/Templates/MonthlyPeriodTemplate.html'
        };

        function RecurringPeriodMonthly($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusDefinition = [];

            }
        }
    }]);
