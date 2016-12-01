(function (app) {

    'use strict';

    ProvisionerRuntimesettingsRadiusDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerRuntimesettingsRadiusDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusProvisionerRuntimeSetting = new RadiusProvisionerRuntimeSetting($scope, ctrl, $attrs);
                radiusProvisionerRuntimeSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Runtime/ProvisionerRuntime/Templates/RadiusSQLProvisionerRuntimeSettingsTemplate.html"

        };
        function RadiusProvisionerRuntimeSetting($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.MainExtensions.RadiusSQLProvisionerRuntimeSetting,Retail.BusinessEntity.MainExtensions"
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeProvisionerRuntimesettingsRadiussql', ProvisionerRuntimesettingsRadiusDirective);

})(app);