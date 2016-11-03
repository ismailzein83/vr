(function (app) {

    'use strict';

    ProvisionerRuntimesettingsTelesSwitchDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerRuntimesettingsTelesSwitchDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusProvisionerRuntimeSetting = new TelestSwitchProvisionerRuntimeSetting($scope, ctrl, $attrs);
                radiusProvisionerRuntimeSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Runtime/ProvisionerRuntime/Templates/ActivateTelesSwitchProvisionerRuntimeSettingsTemplate.html"

        };
        function TelestSwitchProvisionerRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.provisionerRuntimeSettings != undefined) {
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ActivateTelesSwitchUserProvisionerRuntimeSettings,Retail.BusinessEntity.MainExtensions",
                        Domain:$scope.scopeModel.domain,
                        GateWay: $scope.scopeModel.gateWay,
                        LoginName: $scope.scopeModel.loginName,
                        Password: $scope.scopeModel.password,
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeProvisionerRuntimesettingsActivatetelesswitch', ProvisionerRuntimesettingsTelesSwitchDirective);

})(app);