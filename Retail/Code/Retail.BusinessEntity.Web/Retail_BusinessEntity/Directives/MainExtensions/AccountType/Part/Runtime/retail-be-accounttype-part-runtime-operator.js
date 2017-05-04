'use strict';

app.directive('retailBeAccounttypePartRuntimeOperator', ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountTypeOperatorPartRuntime = new AccountTypeOperatorPartRuntime($scope, ctrl, $attrs);
                accountTypeOperatorPartRuntime.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeOperatorPartRuntimeTemplate.html'
        };

        function AccountTypeOperatorPartRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var partSettings;

                    if (payload != undefined) {
                        partSettings = payload.partSettings;
                    }

                    $scope.scopeModel.isMobileOperator = partSettings != undefined ? partSettings.IsMobileOperator : undefined;
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartOperatorSetting, Retail.BusinessEntity.MainExtensions',
                        IsMobileOperator: $scope.scopeModel.isMobileOperator
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

