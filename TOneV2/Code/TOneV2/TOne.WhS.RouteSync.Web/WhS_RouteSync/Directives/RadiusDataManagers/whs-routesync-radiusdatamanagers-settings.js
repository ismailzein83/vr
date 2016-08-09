(function (app) {

    'use strict';

    RaduisDataManagersSettingsDirective.$inject = ['WhS_RouteSync_RadiusDataManagerSettingAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RaduisDataManagersSettingsDirective(WhS_RouteSync_RadiusDataManagerSettingAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusDataManagersSettings = new RadiusDataManagersSettings($scope, ctrl, $attrs);
                radiusDataManagersSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/RadiusDataManagers/Templates/RadiusDataManagersTemplate.html'
        };

        function RadiusDataManagersSettings($scope, ctrl, $attrs) {
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
                    var radiusDataManagersSettings;

                    if (payload != undefined) {
                        radiusDataManagersSettings = payload.radiusDataManagersSettings;
                    }

                    if (radiusDataManagersSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getRadiusDataManagersSettingsTemplateConfigsPromise = getRadiusDataManagersSettingsTemplateConfigs();
                    promises.push(getRadiusDataManagersSettingsTemplateConfigsPromise);

                    function getRadiusDataManagersSettingsTemplateConfigs() {
                        return WhS_RouteSync_RadiusDataManagerSettingAPIService.GetRadiusDataManagerExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (radiusDataManagersSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, radiusDataManagersSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { radiusDataManagersSettings: radiusDataManagersSettings };
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
                    return {
                        DataManager: data
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncRadiusdatamanagersSettings', RaduisDataManagersSettingsDirective);

})(app);