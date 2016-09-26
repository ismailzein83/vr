'use strict';

app.directive('retailBeAccounttypePartDefinitionOtherinfo', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeOtherInfoPartDefinition = new AccountTypeOtherInfoPartDefinition($scope, ctrl, $attrs);
            accountTypeOtherInfoPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeOtherInfoPartDefinitionTemplate.html'
    };

    function AccountTypeOtherInfoPartDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.RingoExtensions.AccountParts.AccountPartOtherInfoDefinition,Retail.BusinessEntity.RingoExtensions'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);