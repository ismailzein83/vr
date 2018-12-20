(function (app) {
    'use strict';
    TaxruleOverall.$inject = ['UtilsService', 'VRUIUtilsService'];

    function TaxruleOverall(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var overallTaxRuleSettings = new OverallTaxRuleSettings(ctrl, $scope, $attrs);
                overallTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/MainExtensions/TaxRules/Voice/Templates/TaxRuleOverallTemplate.html"
        };

        function OverallTaxRuleSettings(ctrl, $scope, $attrs) {
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
                            $scope.scopeModel.taxPercentage = payload.settings.TaxPercentage;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.RA.Business.OverallVoiceTaxRuleSettings,Retail.RA.Business",
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

    app.directive('raRulesVoicetaxruleOverall', TaxruleOverall);
})(app);