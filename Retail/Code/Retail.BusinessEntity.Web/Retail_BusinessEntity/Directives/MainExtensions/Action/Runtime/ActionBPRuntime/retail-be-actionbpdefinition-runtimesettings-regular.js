﻿(function (app) {

    'use strict';

    RegularActionbpdefinitionRuntimesettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService','Retail_BE_ActionDefinitionAPIService'];

    function RegularActionbpdefinitionRuntimesettingsDirective(UtilsService, VRUIUtilsService, Retail_BE_ActionDefinitionAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var regularActionbpdefinitionRuntimesettings = new RegularActionbpdefinitionRuntimesettings($scope, ctrl, $attrs);
                regularActionbpdefinitionRuntimesettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Action/Runtime/ActionBPRuntime/Templates/RegularActionBPSettingsTemplate.html"

        };
        function RegularActionbpdefinitionRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var provisionerRuntimeAPI;
            var provisionerReadyDeferred = UtilsService.createPromiseDeferred();

            var provisionerDefinitionSettings;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.onProvisionerRuntimeSettingReady = function (api) {
                    provisionerRuntimeAPI = api;
                    provisionerReadyDeferred.resolve();
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                    }

                    var promises = [];
                    var provisionerDefinitionSettings;

                    if (payload != undefined && payload.bpDefinitionSettings !=undefined) {
                        provisionerDefinitionSettings = payload.bpDefinitionSettings.ProvisionerDefinitionSettings;
                    }


                    var loadProvisionerDefinitionExtensionConfigsPromise = loadProvisionerDefinitionExtensionConfigs();
                    promises.push(loadProvisionerDefinitionExtensionConfigsPromise);

                    if (provisionerDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadProvisionerDefinitionExtensionConfigs() {
                        return Retail_BE_ActionDefinitionAPIService.GetProvisionerDefinitionExtensionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (provisionerDefinitionSettings != undefined)
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, provisionerDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                            }
                        });
                    }

                    function loadDirective() {
                        var provisionerSettingLoadDeferred = UtilsService.createPromiseDeferred();

                        provisionerReadyDeferred.promise.then(function () {
                            var settingPayload = provisionerDefinitionSettings != undefined ? { provisionerRuntimeSettings: provisionerDefinitionSettings } : undefined
                            VRUIUtilsService.callDirectiveLoad(provisionerRuntimeAPI, settingPayload, provisionerSettingLoadDeferred);
                        });
                        return provisionerSettingLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainActionBPs.Entities.RegularActionBSettings, Retail.BusinessEntity.MainActionBPs.Entities",
                        NewStatusDefinitionId: "8a359658-75d4-47fb-8b3b-1f940c3faa58",
                        ActionProvisioner: provisionerRuntimeAPI.getData()
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionRuntimesettingsRegular', RegularActionbpdefinitionRuntimesettingsDirective);

})(app);