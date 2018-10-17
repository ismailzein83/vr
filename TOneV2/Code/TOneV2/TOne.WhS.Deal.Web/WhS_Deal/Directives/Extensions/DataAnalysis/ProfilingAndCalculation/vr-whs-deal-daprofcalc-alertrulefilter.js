(function (app) {

    'use strict';

    DealDAProfCalcAlertRuleFilterDirective.$inject = ['VRUIUtilsService', 'UtilsService', 'WhS_DealDAProfCalcAlertRuleFilterType'];

    function DealDAProfCalcAlertRuleFilterDirective(VRUIUtilsService, UtilsService, WhS_DealDAProfCalcAlertRuleFilterType) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DealDAProfCalcAlertRuleFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/DataAnalysis/ProfilingAndCalculation/Templates/DealDAProfCalcAlertRuleFilterTemplate.html'
        };

        function DealDAProfCalcAlertRuleFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dealAlertRuleFilterTypeSelectorAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = UtilsService.getArrayEnum(WhS_DealDAProfCalcAlertRuleFilterType);

                $scope.scopeModel.onDealAlertRuleFilterTypeSelectorReady = function (api) {
                    dealAlertRuleFilterTypeSelectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var dealDAProfCalcAlertRuleFilter;

                    if (payload != undefined) {
                        dealDAProfCalcAlertRuleFilter = payload.daProfCalcAlertRuleFilter;
                    }

                    if (dealDAProfCalcAlertRuleFilter != undefined && dealDAProfCalcAlertRuleFilter.DealDAProfCalcAlertRuleFilterType != undefined) {
                        $scope.scopeModel.selectedvalues = UtilsService.getItemByVal($scope.scopeModel.datasource, dealDAProfCalcAlertRuleFilter.DealDAProfCalcAlertRuleFilterType, "value");
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.DataAnalysis.ProfilingAndCalculation.DealDAProfCalcAlertRuleFilter, TOne.WhS.Deal.MainExtensions",
                        DealDAProfCalcAlertRuleFilterType: $scope.scopeModel.selectedvalues.value
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrWhsDealDaprofcalcAlertrulefilter', DealDAProfCalcAlertRuleFilterDirective);
})(app);