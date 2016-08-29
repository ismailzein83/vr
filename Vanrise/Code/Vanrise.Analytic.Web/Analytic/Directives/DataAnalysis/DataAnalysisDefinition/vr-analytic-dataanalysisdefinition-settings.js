(function (app) {

    'use strict';

    DataAnalysisDefinitionSettingsDirective.$inject = ['VR_Analytic_DataAnalysisDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DataAnalysisDefinitionSettingsDirective(VR_Analytic_DataAnalysisDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                var dataAnalysisDefinitionSettings = new DataAnalysisDefinitionSettings($scope, ctrl, $attrs);
                dataAnalysisDefinitionSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/DataAnalysisDefinition/Templates/DataAnalysisDefinitionSettingsTemplate.html'
        };

        function DataAnalysisDefinitionSettings($scope, ctrl, $attrs) {
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

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var dataAnalysisDefinitionSettings;

                    if (payload != undefined) {
                        dataAnalysisDefinitionSettings = payload.dataAnalysisDefinitionSettings;
                    }

                    var getDataAnalysisDefinitionSettingsTemplateConfigsPromise = getDataAnalysisDefinitionSettingsTemplateConfigs();
                    promises.push(getDataAnalysisDefinitionSettingsTemplateConfigsPromise);

                    if (dataAnalysisDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getDataAnalysisDefinitionSettingsTemplateConfigs() {
                        return VR_Analytic_DataAnalysisDefinitionAPIService.GetStyleFormatingExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (dataAnalysisDefinitionSettings != undefined && dataAnalysisDefinitionSettings.StyleFormatingSettings != null) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, dataAnalysisDefinitionSettings.StyleFormatingSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { styleFormatingSettings: dataAnalysisDefinitionSettings.StyleFormatingSettings };
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
                        StyleFormatingSettings: data
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDataanalysisdefinitionSettings', DataAnalysisDefinitionSettingsDirective);

})(app);