'use strict';

app.directive('retailBeAccounttypePartRuntimeOtherinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeOtherInfoPartRuntime = new AccountTypeOtherInfoPartRuntime($scope, ctrl, $attrs);
            accountTypeOtherInfoPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeOtherInfoPartRuntimeTemplate.html'
    };

    function AccountTypeOtherInfoPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};


            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.partSettings != undefined) {

                    $scope.scopeModel.cnic = payload.partSettings.CNIC;
                    $scope.scopeModel.taxCode = payload.partSettings.TaxCode;
                    $scope.scopeModel.isTheft = payload.partSettings.IsTheft;
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.RingoExtensions.AccountParts.AccountPartOtherInfo,Retail.BusinessEntity.RingoExtensions',
                    CNIC: $scope.scopeModel.cnic,
                    TaxCode: $scope.scopeModel.taxCode,
                    IsTheft: $scope.scopeModel.isTheft
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

    }
}]);