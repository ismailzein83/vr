(function (app) {

    'use strict';
    poolBasedCLIGroupCustomObjectSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];
    function poolBasedCLIGroupCustomObjectSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomObjectCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/TranslationRule/Templates/PoolBasedCLIGroupCustomObjectSettingsTemplate.html'

        };
        function CustomObjectCtor($scope, ctrl, $attrs) {
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
                        $type: "NP.IVSwitch.Business.PoolBasedCLIGroupCustomObjectTypeSettings, NP.IVSwitch.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('npIvswitchTranslationrulePoolbasedcligroupCustomobjectsettings', poolBasedCLIGroupCustomObjectSettings);

})(app);
