(function (app) {

    'use strict';

    ChargingpolicypartDurationtarrifsSingleDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ChargingpolicypartDurationtarrifsSingleDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var chargingpolicypartDurationtarrifsSingle = new ChargingpolicypartDurationtarrifsSingle($scope, ctrl, $attrs);
                chargingpolicypartDurationtarrifsSingle.initializeController();
            },
            controllerAs: "singletarrifCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/ChargingPolicyParts/DurationTariffs/Templates/SingleDurationTariffsTemplate.html"

        };
       
        function ChargingpolicypartDurationtarrifsSingle($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
          
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.DurationTariffs.SingleDurationTariffDefinition,Retail.BusinessEntity.MainExtensions"
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeChargingpolicypartDurationtarrifsSingle', ChargingpolicypartDurationtarrifsSingleDirective);

})(app);