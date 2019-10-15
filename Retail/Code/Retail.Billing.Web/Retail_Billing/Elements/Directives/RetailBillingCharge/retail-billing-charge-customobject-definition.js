(function (app) {

    'use strict';

    RetailbillingChargeCustomObjectDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RetailbillingChargeCustomObjectDefinition(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBEChargeCustomObjectDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeCustomObjectDefinitionTemplate.html"
        };

        function RetailBEChargeCustomObjectDefinitionCtor($scope, ctrl, $attrs) {
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
                        $type: "Retail.Billing.MainExtensions.RetailBillingCharge.RetailBillingChargeCustomObjectTypeSettings, Retail.Billing.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('retailBillingChargeCustomobjectDefinition', RetailbillingChargeCustomObjectDefinition);
})(app);