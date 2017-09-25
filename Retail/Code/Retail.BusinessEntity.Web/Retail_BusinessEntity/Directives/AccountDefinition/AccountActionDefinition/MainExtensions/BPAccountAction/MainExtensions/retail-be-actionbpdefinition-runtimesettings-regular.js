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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/Templates/RegularActionBPSettingsTemplate.html"

        };
        function RegularActionbpdefinitionRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var provisionerRuntimeAPI;
            var provisionerReadyDeferred = UtilsService.createPromiseDeferred();

            var provisionerPostActionRuntimeAPI;
            var provisionerPostActionRuntimeReadyDeferred = UtilsService.createPromiseDeferred();
            var accountId;
            var accountBEDefinitionId;

            var provisionerDefinitionSettings;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.onProvisionerRuntimeSettingReady = function (api) {
                    provisionerRuntimeAPI = api;
                    provisionerReadyDeferred.resolve();
                };
                $scope.scopeModel.onProvisionerPostActionRuntimeReady = function (api) {
                    provisionerPostActionRuntimeAPI = api;
                    provisionerPostActionRuntimeReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        accountId = payload.accountId;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        mainPayload = payload;
                    }

                    var promises = [];
                    var provisionerDefinitionSettings;
                    var provisionPostAction;
                    var actionBPSettings;
                    if (payload != undefined )
                        if( payload.bpDefinitionSettings !=undefined) {
                            provisionerDefinitionSettings = payload.bpDefinitionSettings.ProvisionerDefinitionSettings;
                            provisionPostAction = payload.bpDefinitionSettings.ProvisionDefinitionPostAction;
                            if(provisionPostAction != undefined)
                            {
                                $scope.scopeModel.postActionRuntimeDirective = provisionPostAction.RuntimeDirective;
                            }
                            if(payload.vrActionEntity !=undefined)
                            {
                                actionBPSettings = payload.vrActionEntity.ActionBPSettings;
                            }
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

                            var settingPayload = {
                                provisionerDefinitionSettings: provisionerDefinitionSettings,
                                accountId: accountId,
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (actionBPSettings != undefined)
                            {
                                settingPayload.provisionerRuntimeEntity = actionBPSettings.AccountProvisioner;
                            }
                            VRUIUtilsService.callDirectiveLoad(provisionerRuntimeAPI, settingPayload, provisionerSettingLoadDeferred);
                        });
                        return provisionerSettingLoadDeferred.promise;
                    }

                    if ($scope.scopeModel.postActionRuntimeDirective != undefined) {
                        promises.push(loadPostActionDirective());
                    }

                    function loadPostActionDirective() {
                        if ($scope.scopeModel.postActionRuntimeDirective != undefined)
                        {
                            var provisionerPostActionRuntimeLoadDeferred = UtilsService.createPromiseDeferred();
                            provisionerPostActionRuntimeReadyDeferred.promise.then(function () {

                                var postActionPayload = {
                                    accountProvisionDefinitionPostAction: provisionPostAction,
                                    accountId: accountId,
                                    accountBEDefinitionId: accountBEDefinitionId
                                };
                                if (actionBPSettings != undefined) {
                                    postActionPayload.provisionPostAction = actionBPSettings.ProvisionPostAction;
                                }
                                VRUIUtilsService.callDirectiveLoad(provisionerPostActionRuntimeAPI, postActionPayload, provisionerPostActionRuntimeLoadDeferred);
                            });
                            return provisionerPostActionRuntimeLoadDeferred.promise;
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainActionBPs.Entities.RegularActionBPSettings, Retail.BusinessEntity.MainActionBPs.Entities",
                        AccountProvisioner: provisionerRuntimeAPI.getData(),
                        ProvisionPostAction: provisionerPostActionRuntimeAPI != undefined?provisionerPostActionRuntimeAPI.getData():undefined
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionRuntimesettingsRegular', RegularActionbpdefinitionRuntimesettingsDirective);

})(app);