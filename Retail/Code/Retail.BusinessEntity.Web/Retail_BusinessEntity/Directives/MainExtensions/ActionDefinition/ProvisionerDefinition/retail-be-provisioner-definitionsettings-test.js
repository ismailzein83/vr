(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsTestDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsTestDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var testProvisionerDefinitionSetting = new TestProvisionerDefinitionSetting($scope, ctrl, $attrs);
                testProvisionerDefinitionSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ActionDefinition/ProvisionerDefinition/Templates/TestProvisionerDefinitionSettingsTemplate.html"

        };
        function TestProvisionerDefinitionSetting($scope, ctrl, $attrs) {
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

    app.directive('retailBeProvisionerDefinitionsettingsTest', ProvisionerDefinitionsettingsTestDirective);

})(app);