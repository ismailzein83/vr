'use strict';

app.directive('retailBeAccounttypePartDefinitionOperator', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeOperatorPartDefinition = new AccountTypeOperatorPartDefinition($scope, ctrl, $attrs);
            accountTypeOperatorPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeOperatorPartDefinitionTemplate.html'
    };

    function AccountTypeOperatorPartDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var partDefinitionSettings;

                if (payload != undefined) {
                    partDefinitionSettings = payload.partDefinitionSettings;
                }

                $scope.scopeModel.invisibleMobileOperator = partDefinitionSettings != undefined ? partDefinitionSettings.InvisibleMobileOperator : undefined;
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSettingDefinition, Retail.BusinessEntity.MainExtensions',
                    InvisibleMobileOperator: $scope.scopeModel.invisibleMobileOperator
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);