(function (app) {

    'use strict';

    FinancialAccountObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function FinancialAccountObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customerObjectType = new FinancialAccountDierctiveObjectType($scope, ctrl, $attrs);
                customerObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/VRObjectTypes/Templates/FinancialAccountObjectTypeTemplate.html'


        };
        function FinancialAccountDierctiveObjectType($scope, ctrl, $attrs) {
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
                    var data = {
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountObjectType, TOne.WhS.AccountBalance.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsAccountbalanceFinancialaccountobjecttype', FinancialAccountObjectType);

})(app);