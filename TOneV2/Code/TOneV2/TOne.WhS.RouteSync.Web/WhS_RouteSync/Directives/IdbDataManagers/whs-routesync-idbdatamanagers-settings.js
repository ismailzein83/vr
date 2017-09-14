(function (app) {

    'use strict';

    IdbDataManagersSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_IdbDataManagerSettingAPIService'];

    function IdbDataManagersSettingsDirective(UtilsService, VRUIUtilsService, WhS_RouteSync_IdbDataManagerSettingAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new IdbDataManagersSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSettings",
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/IdbDataManagers/Templates/IdbDataManagersTemplate.html'
        };

        function IdbDataManagersSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var idbDataManagersSettings;

                    if (payload != undefined) {
                        idbDataManagersSettings = payload.idbDataManagersSettings;
                    }

                    if (idbDataManagersSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var idbDataManagersSettingsTemplateConfigsPromise = getIdbDataManagersSettingsTemplateConfigs();
                    promises.push(idbDataManagersSettingsTemplateConfigsPromise);

                    function getIdbDataManagersSettingsTemplateConfigs() {
                        return WhS_RouteSync_IdbDataManagerSettingAPIService.GetIdbDataManagerExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (idbDataManagersSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, idbDataManagersSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { idbDataManagersSettings: idbDataManagersSettings };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }

                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncIdbdatamanagersSettings', IdbDataManagersSettingsDirective);

})(app);