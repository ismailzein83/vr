'use strict';

app.directive('retailDataChargingpolicySettings', [function () {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataChargingPolicySettings = new DataChargingPolicySettings($scope, ctrl, $attrs);
            dataChargingPolicySettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Data/Directives/MainExtensions/Templates/DataChargingPolicySettingsTemplate.html'
    };

    function DataChargingPolicySettings($scope, ctrl, $attrs) {
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
                    $type: 'Retail.Data.Entities.DataChargingPolicySettings, Retail.Data.Entities',
                    Parts: partsDirectiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
