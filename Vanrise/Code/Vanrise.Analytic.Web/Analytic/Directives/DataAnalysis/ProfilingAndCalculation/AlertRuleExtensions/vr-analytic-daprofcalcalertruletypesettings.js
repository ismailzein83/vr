(function (app) {

    'use strict';

    DAProfCalcAlertRuleTypeSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function DAProfCalcAlertRuleTypeSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrDAProfCalcAlertRuleTypeSettings = new VRDAProfCalcAlertRuleTypeSettings($scope, ctrl, $attrs);
                vrDAProfCalcAlertRuleTypeSettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/AlertRuleExtensions/Templates/DAProfCalcAlertRuleTypeSettingsTemplate.html"

        };
        function VRDAProfCalcAlertRuleTypeSettings($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Analytic.Entities.DAProfCalcAlertRuleTypeSettings, Vanrise.Analytic.Entities"
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcalertruletypesettings', DAProfCalcAlertRuleTypeSettings);

})(app);