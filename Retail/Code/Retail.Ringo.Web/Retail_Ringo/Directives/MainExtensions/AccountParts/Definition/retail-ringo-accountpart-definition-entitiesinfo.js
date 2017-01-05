'use strict';

app.directive('retailRingoAccountpartDefinitionEntitiesinfo', [function () {
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
        templateUrl: '/Client/Modules/Retail_Ringo/Directives/MainExtensions/AccountParts/Definition/Templates/AccountTypeEntitiesInfoPartDefinitionTemplate.html'
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
                    $type: 'Retail.Ringo.MainExtensions.AccountParts.AccountPartEntitiesInfo, Retail.Ringo.MainExtensions'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);