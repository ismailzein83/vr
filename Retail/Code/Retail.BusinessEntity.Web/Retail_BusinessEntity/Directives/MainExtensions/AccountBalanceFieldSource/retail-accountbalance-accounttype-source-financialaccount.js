'use strict';

app.directive('retailAccountbalanceAccounttypeSourceFinancialaccount', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var financialAccountSourceSetting = new FinancialAccountSourceSetting($scope, ctrl, $attrs);
                financialAccountSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountBalanceFieldSource/Templates/FinancialAccountSourceSetting.html'
        };

        function FinancialAccountSourceSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: " Retail.BusinessEntity.MainExtensions.AccountBalanceFieldSource.FinancialAccountFieldSourceSetting,  Retail.BusinessEntity.MainExtensions",
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
