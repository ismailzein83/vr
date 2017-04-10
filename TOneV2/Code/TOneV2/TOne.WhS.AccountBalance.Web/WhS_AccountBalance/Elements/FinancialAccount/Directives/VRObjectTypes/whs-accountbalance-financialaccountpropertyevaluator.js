(function (app) {

    'use strict';

    FinancialAccountPropertyEvaluator.$inject = ['WhS_AccountBalance_FinancialAccountPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function FinancialAccountPropertyEvaluator(WhS_AccountBalance_FinancialAccountPropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new FinancialAccountSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/VRObjectTypes/Templates/FinancialAccountPropertyEvaluatorTemplate.html'
        };

        function FinancialAccountSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.financialAccountFields = UtilsService.getArrayEnum(WhS_AccountBalance_FinancialAccountPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedFinancialAccountField = UtilsService.getItemByVal($scope.scopeModel.financialAccountFields, payload.objectPropertyEvaluator.FinancialAccountField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountPropertyEvaluator, TOne.WhS.AccountBalance.MainExtensions",
                        FinancialAccountField: $scope.scopeModel.selectedFinancialAccountField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('whsAccountbalanceFinancialaccountpropertyevaluator', FinancialAccountPropertyEvaluator);

})(app);
