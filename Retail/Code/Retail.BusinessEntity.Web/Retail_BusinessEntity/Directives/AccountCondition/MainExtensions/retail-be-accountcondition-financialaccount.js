(function (app) {

    'use strict';

    FinancialAccountConditionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function FinancialAccountConditionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FinancialAccountConditionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/FinancialAccountConditionTemplate.html"
        };

        function FinancialAccountConditionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountConditions.FinancialAccountCondition, Retail.BusinessEntity.MainExtensions"
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountconditionFinancialaccount', FinancialAccountConditionDirective);

})(app);