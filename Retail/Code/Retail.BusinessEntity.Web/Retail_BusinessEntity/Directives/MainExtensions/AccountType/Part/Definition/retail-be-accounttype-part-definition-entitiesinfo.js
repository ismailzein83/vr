'use strict';

app.directive('retailBeAccounttypePartDefinitionEntitiesinfo', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeEntitiesInfoPartDefinition = new AccountTypeEntitiesInfoPartDefinition($scope, ctrl, $attrs);
            accountTypeEntitiesInfoPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeEntitiesInfoPartDefinitionTemplate.html'
    };

    function AccountTypeEntitiesInfoPartDefinition($scope, ctrl, $attrs) {
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
                    $type: 'Retail.BusinessEntity.RingoExtensions.AccountParts.AccountPartEntitiesInfo,Retail.BusinessEntity.RingoExtensions'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);