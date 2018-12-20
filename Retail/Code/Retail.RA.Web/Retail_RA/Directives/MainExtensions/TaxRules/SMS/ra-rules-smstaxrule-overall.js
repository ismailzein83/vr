(function (app) {
    'use strict';
    OverallSMSTaxRule.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OverallSMSTaxRule(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var overallSMSTaxRuleSettings = new OverallSMSTaxRuleSettings(ctrl, $scope, $attrs);
                overallSMSTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/Rules/SMS/Templates/SMSTaxRuleOverallTemplate.html"
        };

        function OverallSMSTaxRuleSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        if (payload.settings != undefined) {
                            $scope.scopeModel.taxPercentage = settings.TaxPercentage;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.RA.Business.OverallSMSTaxRuleSettings,Retail.RA.Business",
                        TaxPercentage: $scope.scopeModel.taxPercentage
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesSmstaxruleOverall', OverallSMSTaxRule);
})(app);