'use strict';

app.directive('retailZajilAccounttypePartRuntimeOrderdetails', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeOrderDetailsRuntime = new AccountTypeOrderDetailsRuntime($scope, ctrl, $attrs);
            accountTypeOrderDetailsRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Zajil/Directives/MainExtensions/Account/Part/Runtime/Templates/AccountTypePartOrderDetailRuntimeTemplate.html'
    };

    function AccountTypeOrderDetailsRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
       
        var mainPayload;
        function initializeController() {
            $scope.scopeModel = {}


            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                mainPayload = payload;
            };

            api.getData = function () {
                return {
                    $type: 'Retail.Zajil.MainExtensions.AccountPartOrderDetail, Retail.Zajil.MainExtensions',
                    OrderDetailItems: []
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);