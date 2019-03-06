(function (app) {

    'use strict';

    recurringChargePeriodType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function recurringChargePeriodType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecurringChargePeriodCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RecurringChargePeriod/Templates/RecurringChargePeriodCustomObjectSettings.html"

        };
        function RecurringChargePeriodCtor($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.Business.FinancialRecurringChargePeriodObjectTypeSettings, Retail.BusinessEntity.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRecurringchargeperiodCustomobjectsettings', recurringChargePeriodType);

})(app);
