(function (app) {

    'use strict';

    AnalyticDataProviderSettingsDirective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticDataProviderSettingsDirective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
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
                var analyticDataProviderSettings = new AnalyticDataProviderSettings($scope, ctrl, $attrs);
                analyticDataProviderSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/AnalyticDataProvider/Templates/AnalyticDataProviderSettings.html'
        };

        function AnalyticDataProviderSettings($scope, ctrl, $attrs) {
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
                    var analyticDataProviderSettings;

                    if (payload != undefined) {
                        analyticDataProviderSettings = payload.AnalyticDataProvider;
                    }

                    if (analyticDataProviderSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getAnalyticDataProviderSettingsTemplateConfigsPromise = getAnalyticDataProviderSettingsTemplateConfigs();
                    promises.push(getAnalyticDataProviderSettingsTemplateConfigsPromise);

                    function getAnalyticDataProviderSettingsTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetAnalyticDataProviderConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (analyticDataProviderSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, analyticDataProviderSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { analyticDataProviderSettings: analyticDataProviderSettings };
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
                        console.log(data);
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

    app.directive('vrAnalyticDataproviderSettings', AnalyticDataProviderSettingsDirective);

})(app);