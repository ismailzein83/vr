'use strict';

app.directive('retailMultinetAccounttypePartDefinitionCompanyextendedinfo', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountTypeExtendedInfoDefinition($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/Account/Part/Definition/Templates/MultiNetCompanyExtendedInfoDefinitionTemplate.html'
    };

    function AccountTypeExtendedInfoDefinition($scope, ctrl, $attrs)
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