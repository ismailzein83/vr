'use strict';

app.directive('retailBeAccountactiondefinitionsettingsBpaccount', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BPAccountActionDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/Templates/BPAccountActionSettingsTemplate.html'
        };

        function BPAccountActionDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var bPDefinitionSettingsApi;
            var bPDefinitionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

            var viewLogPermissionAPI;
            var viewLogPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var startActionPermissionAPI;
            var startActionPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};


                $scope.scopeModel.onBPAccountActionSelectiveReady = function (api) {
                    bPDefinitionSettingsApi = api;
                    bPDefinitionSettingsPromiseDeferred.resolve();
                };
                $scope.scopeModel.onViewLogRequiredPermissionReady = function (api) {
                    viewLogPermissionAPI = api;
                    viewLogPermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onStartActionRequiredPermissionReady = function (api) {
                    startActionPermissionAPI = api;
                    startActionPermissionReadyDeferred.resolve();
                };


                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountActionDefinitionSettings;
                    if (payload != undefined)
                    {
                        accountActionDefinitionSettings = payload.accountActionDefinitionSettings;
                    }
                    var promises = [];
                    promises.push(loadBusinessEntityDefinitionSelector());
                    promises.push(loadViewLogRequiredPermission());
                    promises.push(loadStartActionRequiredPermission());

                    function loadBusinessEntityDefinitionSelector() {
                        var bPDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        bPDefinitionSettingsPromiseDeferred.promise.then(function () {
                            var payloadBPDefinition = accountActionDefinitionSettings != undefined ? { bpDefinitionSettings: accountActionDefinitionSettings.BPDefinitionSettings } : undefined;
                            VRUIUtilsService.callDirectiveLoad(bPDefinitionSettingsApi, payloadBPDefinition, bPDefinitionSettingsLoadDeferred);
                        });

                        return bPDefinitionSettingsLoadDeferred.promise;
                    }
                    function loadViewLogRequiredPermission() {
                        var viewLogSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewLogPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: accountActionDefinitionSettings && accountActionDefinitionSettings.Security && accountActionDefinitionSettings.Security.ViewLogPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewLogPermissionAPI, dataPayload, viewLogSettingPermissionLoadDeferred);
                        });
                        return viewLogSettingPermissionLoadDeferred.promise;
                    }


                    function loadStartActionRequiredPermission() {
                        var startActionPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        startActionPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: accountActionDefinitionSettings && accountActionDefinitionSettings.Security && accountActionDefinitionSettings.Security.StartActionPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(startActionPermissionAPI, dataPayload, startActionPermissionLoadDeferred);
                        });
                        return startActionPermissionLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBEActionTypes.BPAccountActionSettings, Retail.BusinessEntity.MainExtensions',
                        BPDefinitionSettings: bPDefinitionSettingsApi.getData(),
                        Security: {
                            ViewLogPermission: viewLogPermissionAPI.getData(),
                            StartActionPermission: startActionPermissionAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);