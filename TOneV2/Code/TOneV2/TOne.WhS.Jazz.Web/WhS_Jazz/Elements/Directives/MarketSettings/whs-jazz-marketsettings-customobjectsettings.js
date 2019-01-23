(function (app) {

    'use strict';

    whsJazzMarketSettingCustomObjectSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function whsJazzMarketSettingCustomObjectSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/MarketSettings/Templates/MarketSettingsCustomObjectSettings.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.Jazz.Business.WhSJazzMarketSettingsCustomObject,TOne.WhS.Jazz.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzMarketsettingsCustomobjectsettings', whsJazzMarketSettingCustomObjectSettings);

})(app);
