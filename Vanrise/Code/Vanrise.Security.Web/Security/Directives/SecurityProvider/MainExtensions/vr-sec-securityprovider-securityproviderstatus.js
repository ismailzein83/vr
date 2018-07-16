(function (app) {

    'use strict';

    SecurityProviderStatusActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function SecurityProviderStatusActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SecurityproviderStatus($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Security/Directives/SecurityProvider/MainExtensions/Templates/SecurityProviderStatusActionTemplate.html'
        };

        function SecurityproviderStatus($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.settings !=undefined) {
                        $scope.scopeModel.isEnabled = payload.settings.SetEnable;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.MainExtensions.GenericBEActions.SecurityProviderStatusAction, Vanrise.Security.MainExtensions",
                        SetEnable: $scope.scopeModel.isEnabled
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrSecSecurityproviderSecurityproviderstatus', SecurityProviderStatusActionDirective);

})(app);