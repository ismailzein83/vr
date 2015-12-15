"use strict";

app.directive("vrQmClitesterProfilesettings", ['ProfileTypeEnum', 'UtilsService', function (ProfileTypeEnum, UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/QM_CLITester/Directives/Profile/Templates/ProfileSettngsDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.profileTypes = UtilsService.getArrayEnum(ProfileTypeEnum);
        $scope.selectedProfileType = [];

        $scope.type = undefined;
        $scope.gatewayIP = undefined;
        $scope.gatewayPort = undefined;
        $scope.sourceNumber = undefined;
        $scope.callTime = undefined;
        $scope.ringTime = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "QM.CLITester.iTestIntegration.ProfileExtensionSettings, QM.CLITester.iTestIntegration",
                    Type: $scope.selectedProfileType.value,
                    GatewayIP: $scope.gatewayIP,
                    GatewayPort: $scope.gatewayPort,
                    SourceNumber: $scope.sourceNumber,
                    CallTime: $scope.callTime,
                    ringTime: $scope.ringTime
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {

                    $scope.selectedProfileType = UtilsService.getItemByVal($scope.profileTypes, payload.Type, "value");
                    $scope.gatewayIP = payload.GatewayIP;
                    $scope.gatewayPort = payload.GatewayPort;
                    $scope.sourceNumber = payload.SourceNumber;
                    $scope.callTime = payload.CallTime;
                    $scope.ringTime = payload.RingTime;
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
