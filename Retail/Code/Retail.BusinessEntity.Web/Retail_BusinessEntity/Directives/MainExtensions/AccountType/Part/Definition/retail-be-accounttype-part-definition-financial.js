'use strict';

app.directive('retailBeAccounttypePartDefinitionFinancial', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeFinancialPartDefinition = new AccountTypeFinancialPartDefinition($scope, ctrl, $attrs);
            accountTypeFinancialPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeFinancialPartDefinitionTemplate.html'
    };

    function AccountTypeFinancialPartDefinition($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.scopeModel.hideProductSelector = payload.partDefinitionSettings != undefined && payload.partDefinitionSettings.HideProductSelector || false;
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancialDefinition,Retail.BusinessEntity.MainExtensions',
                    HideProductSelector: $scope.scopeModel.hideProductSelector
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);