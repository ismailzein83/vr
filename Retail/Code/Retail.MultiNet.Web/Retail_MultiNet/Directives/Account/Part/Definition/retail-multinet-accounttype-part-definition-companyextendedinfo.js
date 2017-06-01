﻿'use strict';

app.directive('retailZajilAccounttypePartDefinitionCompanyextendedinfo', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeActivationPartDefinition = new AccountTypeActivationPartDefinition($scope, ctrl, $attrs);
            accountTypeActivationPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/MainExtensions/Account/Part/Definition/Templates/AccountTypePartCompanyExtendedInfoDefinitionTemplate.html'
    };

    function AccountTypeActivationPartDefinition($scope, ctrl, $attrs)
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
                
            };

            api.getData = function () {
                return {
                    $type: 'Retail.MultiNet.MainExtensions.MultiNetCompanyExtendedInfoDefinition, Retail.MultiNet.MainExtensions'
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);