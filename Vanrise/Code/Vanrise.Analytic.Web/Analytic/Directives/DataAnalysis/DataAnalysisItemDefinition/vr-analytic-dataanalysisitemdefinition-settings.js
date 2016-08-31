
'use strict';

app.directive('vrAnalyticDataanalysisitemdefinitionSettings', ['Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargingPolicySettings = new ChargingPolicySettings($scope, ctrl, $attrs);
            chargingPolicySettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ChargingPolicy/Templates/ChargingPolicySettingsTemplate.html'
    };

    function ChargingPolicySettings($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var serviceTypeId;
                var chargingPolicy;
                var chargingPolicyDefinitionSettings;
                if (payload != undefined) {

                    serviceTypeId = payload.serviceTypeId;
                    chargingPolicy = payload.chargingPolicy;
                }

                var getChargingPolicyDefinitionSettingsPromise = getChargingPolicyDefinitionSettings();
                promises.push(getChargingPolicyDefinitionSettingsPromise);

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(directiveLoadDeferred.promise);

                UtilsService.waitMultiplePromises([getChargingPolicyDefinitionSettingsPromise, directiveReadyDeferred.promise]).then(function () {
                    var directivePayload = {
                        definitionSettings: chargingPolicyDefinitionSettings,
                        settings: (chargingPolicy != undefined) ? chargingPolicy.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                function getChargingPolicyDefinitionSettings() {
                    return Retail_BE_ServiceTypeAPIService.GetServiceTypeChargingPolicyDefinitionSettings(serviceTypeId).then(function (response) {
                        chargingPolicyDefinitionSettings = response;
                        if (response != null) {
                            $scope.scopeModel.directiveEditor = chargingPolicyDefinitionSettings.ChargingPolicyEditor;
                        }
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
