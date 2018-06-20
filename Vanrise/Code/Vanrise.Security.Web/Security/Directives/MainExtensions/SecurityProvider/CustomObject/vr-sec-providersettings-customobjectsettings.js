(function (app) {

    'use strict';

    SecurityProviderType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function SecurityProviderType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SecurityProviderCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: ""

        };
        function SecurityProviderCtor($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Security.Business.SecurityProviderCustomObjectTypeSettings, Vanrise.Security.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrSecProvidersettingsCustomobjectsettings', SecurityProviderType);

})(app);