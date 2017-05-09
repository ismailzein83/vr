"use strict";

app.directive("retailBeServicetypeSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_ServiceTypeAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_ServiceTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ServiceTypeSettings($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ServiceType/Templates/ServiceTypeSettings.html'
    };

    function ServiceTypeSettings($scope, ctrl) {
        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();
        var extendedSettingsDirectiveAPI;
        var extendedSettingsDirectiveReadyDeferred;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedExtendedSettingsTemplateConfig;
            $scope.scopeModel.extendedSettingsTemplateConfigs = [];
            $scope.scopeModel.onChargingPolicyReady = function (api) {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            };
            $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                extendedSettingsDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isExtendedSettingsDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, extendedSettingsDirectiveAPI, undefined, setLoader, extendedSettingsDirectiveReadyDeferred);
            };
           
            defineAPI();
        }

        function defineAPI() {
            var serviceTypeSettings;
            var api = {};
            var accountTypeSettings;
            var accountBEDefinitionId;
            api.load = function (payload) {
                if (payload != undefined) {
                    serviceTypeSettings = payload.serviceTypeSettings;
                   
                    Retail_BE_ServiceTypeAPIService.GetServiceTypeExtendedSettingsTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.extendedSettingsTemplateConfigs.push(response[i]);
                            }

                            var extendedSettings;
                            if (serviceTypeSettings != undefined)
                                extendedSettings = serviceTypeSettings.ExtendedSettings;

                            if (extendedSettings != undefined && extendedSettings.ConfigId != null) {
                                $scope.scopeModel.selectedExtendedSettingsTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.extendedSettingsTemplateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                            }
                        }
                    });
                }
                function loadChargingPolicy() {
                    var chargingPolicyLoadDeferred = UtilsService.createPromiseDeferred();

                    chargingPolicyReadyDeferred.promise.then(function () {
                        var chargingPolicyPayload;

                        if (serviceTypeSettings!= undefined) {
                            chargingPolicyPayload = { chargingPolicy: serviceTypeSettings.ChargingPolicyDefinitionSettings }
                        }

                        VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyPayload, chargingPolicyLoadDeferred);
                    });

                    return chargingPolicyLoadDeferred.promise;
                }

                function loadExtendedSettingsDirectiveWrapper() {
                    if (serviceTypeSettings == undefined ||
                        serviceTypeSettings.ExtendedSettings == undefined || serviceTypeSettings.ExtendedSettings.ConfigId == undefined)
                        return;

                    extendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var extendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    extendedSettingsDirectiveReadyDeferred.promise.then(function () {
                        extendedSettingsDirectiveReadyDeferred = undefined;

                        var extendedSettingsDirectivePayload;
                        if (serviceTypeSettings != undefined && serviceTypeSettings.ExtendedSettings) {

                            extendedSettingsDirectivePayload = {
                                extendedSettings: serviceTypeSettings.ExtendedSettings
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(extendedSettingsDirectiveAPI, extendedSettingsDirectivePayload, extendedSettingsDirectiveLoadDeferred);
                    });

                    return extendedSettingsDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadChargingPolicy,loadExtendedSettingsDirectiveWrapper])
                  .catch(function (error) {
                      VRNotificationService.notifyExceptionWithClose(error, $scope);
                  }).finally(function () {

                  });

            };
            api.getData = function () {
               
                var data = {
                    ChargingPolicyDefinitionSettings: chargingPolicyAPI.getData(),
                    ExtendedSettings: extendedSettingsDirectiveAPI.getData()

                };
                return data;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
        
    }




    return directiveDefinitionObject;

}]);