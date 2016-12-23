'use strict';

app.directive('retailBeAccountviewdefinitionsettingsFinancialtransactions', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FinancialTransactionsViewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/MainExtensions/AccountViewDefinitions/Templates/FinancialTransactionsViewSettingsTemplate.html'
        };

        function FinancialTransactionsViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountViews.FinancialTransactions, Retail.BusinessEntity.MainExtensions'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);