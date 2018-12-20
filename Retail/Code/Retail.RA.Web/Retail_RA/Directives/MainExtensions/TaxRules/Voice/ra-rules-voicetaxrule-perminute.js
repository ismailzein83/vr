(function (app) {
    'use strict';
    TaxrulePerminute.$inject = ['UtilsService', 'VRUIUtilsService'];

    function TaxrulePerminute(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var perMinuteTaxRuleSettings = new PerMinuteTaxRuleSettings(ctrl, $scope, $attrs);
                perMinuteTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/MainExtensions/TaxRules/Voice/Templates/TaxRulePerMinuteTemplate.html"
        };

        function PerMinuteTaxRuleSettings(ctrl, $scope, $attrs) {
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
                            $scope.scopeModel.taxPerMinute = payload.settings.TaxPerMinute;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.RA.Business.PerMinuterVoiceTaxRuleSettings,Retail.RA.Business",
                        TaxPerMinute: $scope.scopeModel.taxPerMinute
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesVoicetaxrulePerminute', TaxrulePerminute);
})(app);