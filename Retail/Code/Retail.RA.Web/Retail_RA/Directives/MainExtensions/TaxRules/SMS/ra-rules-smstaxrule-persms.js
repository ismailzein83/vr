(function (app) {
    'use strict';
    SmsTaxrulePerSMS.$inject = ['UtilsService', 'VRUIUtilsService'];

    function SmsTaxrulePerSMS(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var perSMSTaxRuleSettings = new PerSMSTaxRuleSettings(ctrl, $scope, $attrs);
                perSMSTaxRuleSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_RA/Directives/MainExtensions/TaxRules/SMS/Templates/SMSTaxRulePerSMSTemplate.html"
        };

        function PerSMSTaxRuleSettings(ctrl, $scope, $attrs) {
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
                            $scope.scopeModel.taxPerSMS = payload.settings.TaxPerSMS;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.RA.Business.PerSMSTaxRuleSettings,Retail.RA.Business",
                        TaxPerSMS: $scope.scopeModel.taxPerSMS
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('raRulesSmstaxrulePersms', SmsTaxrulePerSMS);
})(app);