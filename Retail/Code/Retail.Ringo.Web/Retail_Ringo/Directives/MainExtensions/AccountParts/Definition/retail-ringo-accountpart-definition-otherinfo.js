'use strict';

app.directive('retailRingoAccountpartDefinitionOtherinfo', [function () {
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
        templateUrl: '/Client/Modules/Retail_Ringo/Directives/MainExtensions/AccountParts/Definition/Templates/AccountTypeOtherInfoPartDefinitionTemplate.html'
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
                    $type: 'Retail.Ringo.MainExtensions.AccountParts.AccountPartOtherInfoDefinition,Retail.Ringo.MainExtensions'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);