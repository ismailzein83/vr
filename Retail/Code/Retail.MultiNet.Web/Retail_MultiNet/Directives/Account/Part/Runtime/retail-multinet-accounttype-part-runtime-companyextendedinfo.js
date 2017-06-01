'use strict';

app.directive('retailMultinetAccounttypePartRuntimeCompanyextendedinfo', ["UtilsService", "VRUIUtilsService",  function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/MainExtensions/Account/Part/Runtime/Templates/AccountTypePartCompanyExtendedInfoRuntimeTemplate.html'
    };

    function AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs) {
       
        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            $scope.scopeModel = {};
            var api = {};
            api.load = function (payload) {
             

            };
            api.getData = function () {
                return {
                    $type: 'Retail.MultiNet.MainExtensions.MultiNetCompanyExtendedInfo, Retail.MultiNet.MainExtensions'
                }
                   
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
       
    }
}]);