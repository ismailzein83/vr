//(function (app) {

//    'use strict';

//    ProvisionerDefinitionsettingsTelesSwitchDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

//    function ProvisionerDefinitionsettingsTelesSwitchDirective(UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var radiusProvisionerDefinitionSetting = new TelesSwitchProvisionerDefinitionSetting($scope, ctrl, $attrs);
//                radiusProvisionerDefinitionSetting.initializeController();
//            },
//            controllerAs: "Ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Definition/ProvisionerDefinition/Templates/BlockTelesSwitchProvisionerDefinitionSettingsTemplate.html"

//        };
//        function TelesSwitchProvisionerDefinitionSetting($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;
//            var mainPayload;
//            function initializeController() {
//                $scope.scopeModel = {};
//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        mainPayload = payload;
//                        if (payload.provisionerDefinitionSettings != undefined) {
//                        }
//                    }

//                };

//                api.getData = getData;

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }

//                function getData() {
//                    var data = {
//                        $type: "Retail.BusinessEntity.Extensions.TelesSwitch.BlockTelesSwitchUserProvisionerDefinitionSettings,Retail.BusinessEntity.Extensions.TelesSwitch",
//                    };
//                    return data;
//                }
//            }
//        }
//    }

//    app.directive('retailBeProvisionerDefinitionsettingsBlocktelesswitch', ProvisionerDefinitionsettingsTelesSwitchDirective);

//})(app);