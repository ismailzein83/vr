(function (app) {

    'use strict';

    ChargingpolicypartRatevaluesSingleDirective.$inject = ['UtilsService'];

    function ChargingpolicypartRatevaluesSingleDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var chargingpolicypartRatevaluesSingle = new ChargingpolicypartRatevaluesSingle($scope, ctrl, $attrs);
                chargingpolicypartRatevaluesSingle.initializeController();
            },
            controllerAs: "singleratevalueCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/ChargingPolicyParts/RateValues/Templates/SingleRateValuesTemplate.html"

        };
       
        function ChargingpolicypartRatevaluesSingle($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues.SingleRateValueDefinition,Retail.BusinessEntity.MainExtensions"
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeChargingpolicypartRatevaluesSingle', ChargingpolicypartRatevaluesSingleDirective);

})(app);