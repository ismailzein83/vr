'use strict';

app.directive('retailVoiceChargingpolicySettings', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var voiceChargingPolicySettings = new VoiceChargingPolicySettings($scope, ctrl, $attrs);
            voiceChargingPolicySettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Voice/Directives/MainExtensions/ChargingPolicyDefinition/Templates/VoiceChargingPolicySettingsTemplate.html'
    };

    function VoiceChargingPolicySettings($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        var partsDirectiveAPI;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onPartsDirectiveReady = function (api) {
                partsDirectiveAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var partDefinitions;
                var parts;

                if (payload != undefined) {
                    partDefinitions = payload.definitionSettings.PartDefinitions;

                    if (payload.settings != undefined) {
                        parts = payload.settings.Parts;
                    }
                }

                var partsDirectivePayload = {
                    partDefinitions: partDefinitions,
                    parts: parts
                };

                return partsDirectiveAPI.load(partsDirectivePayload);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.Voice.Entities.VoiceChargingPolicySettings, Retail.Voice.Entities',
                    Parts: partsDirectiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
