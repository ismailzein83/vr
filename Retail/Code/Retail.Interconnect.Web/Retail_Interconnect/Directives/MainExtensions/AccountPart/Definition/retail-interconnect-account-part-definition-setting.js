'use strict';

app.directive('retailInterconnectAccountPartDefinitionSetting', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeSettingtDefinition = new AccountTypeSettingtDefinition($scope, ctrl, $attrs);
            accountTypeSettingtDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Interconnect/Directives/MainExtensions/AccountPart/Definition/Templates/AccountTypeSettingPartDefinitionTemplate.html'
    };

    function AccountTypeSettingtDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

            };

            api.getData = function () {
                return {
                    $type: 'Retail.Interconnect.Business.AccountPartInterconnectSettingDefinition,Retail.Interconnect.Business'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);