(function (app) {

    'use strict';

    DealDAProfCalcAlertRuleFilterDefinitionDirective.$inject = ['VRUIUtilsService', 'UtilsService'];

    function DealDAProfCalcAlertRuleFilterDefinitionDirective(VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealDAProfCalcAlertRuleFilterDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/DataAnalysis/ProfilingAndCalculation/Templates/DealDAProfCalcAlertRuleFilterDefinitionTemplate.html'
        };

        function DealDAProfCalcAlertRuleFilterDefinitionCtor($scope, ctrl, $attrs) {
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
                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.DataAnalysis.ProfilingAndCalculation.DealDAProfCalcAlertRuleFilterDefinition, TOne.WhS.Deal.MainExtensions"
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrWhsDealDaprofcalcAlertrulefilterdefinition', DealDAProfCalcAlertRuleFilterDefinitionDirective);
})(app);