(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsRadiusDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsRadiusDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusProvisionerDefinitionSetting = new RadiusProvisionerDefinitionSetting($scope, ctrl, $attrs);
                radiusProvisionerDefinitionSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ActionDefinition/ProvisionerDefinition/Templates/RadiusProvisionerDefinitionSettingsTemplate.html"

        };
        function RadiusProvisionerDefinitionSetting($scope, ctrl, $attrs) {
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
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.TestProvisionerDefinitionSetting,Retail.BusinessEntity.MainExtensions",
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeProvisionerDefinitionsettingsRadius', ProvisionerDefinitionsettingsRadiusDirective);

})(app);