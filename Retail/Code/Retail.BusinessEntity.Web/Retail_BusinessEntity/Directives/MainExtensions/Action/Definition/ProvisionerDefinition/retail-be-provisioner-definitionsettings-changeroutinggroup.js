(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsChangeRoutingGroupDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsChangeRoutingGroupDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var changeRoutingGroupProvisionerDefinitionSetting = new ChangeRoutingGroupProvisionerDefinitionSetting($scope, ctrl, $attrs);
                changeRoutingGroupProvisionerDefinitionSetting.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Definition/ProvisionerDefinition/Templates/ChangeRoutingGroupProvisionerDefinitionSettingsTemplate.html"

        };
        function ChangeRoutingGroupProvisionerDefinitionSetting($scope, ctrl, $attrs) {
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
                        if(payload.provisionerDefinitionSettings !=undefined)
                        {
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ChangeRoutingGroupProvisionerDefinitionSetting,Retail.BusinessEntity.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeProvisionerDefinitionsettingsChangeroutinggroup', ProvisionerDefinitionsettingsChangeRoutingGroupDirective);

})(app);