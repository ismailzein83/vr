'use strict';

app.directive('retailBeAccounttypePartRuntimeActivation', ["Retail_BE_AccountStatusEnum",function (Retail_BE_AccountStatusEnum) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeActivationPartRuntime = new AccountTypeActivationPartRuntime($scope, ctrl, $attrs);
            accountTypeActivationPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeActivationPartRuntimeTemplate.html'
    };

    function AccountTypeActivationPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.accountStatus = UtilsService.getArrayEnum(Retail_BE_AccountStatusEnum);
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if(payload != undefined)
                {
                    $scope.scopeModel.selectedAccountStatus = UtilsService.getItemByVal($scope.scopeModel.accountStatus, payload.Status,"value");
                } else {
                    $scope.scopeModel.selectedAccountStatus = $scope.scopeModel.accountStatus[0];
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartActivation,Retail.BusinessEntity.MainExtensions',
                    Status: $scope.scopeModel.selectedAccountStatus.value,
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);