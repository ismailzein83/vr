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
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/WHSFinancialAccountObjectTypeTemplate.html'


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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.WHSFinancialAccountObjectType, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsBeFinancialaccountobjecttype', FinancialAccountObjectType);

})(app);