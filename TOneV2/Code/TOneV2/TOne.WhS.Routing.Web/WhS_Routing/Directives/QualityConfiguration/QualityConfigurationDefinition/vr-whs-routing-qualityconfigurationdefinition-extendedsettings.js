(function (app) {

    'use strict';

    QualityConfigurationDefinitionExtendedSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationDefinitionAPIService'];

    function QualityConfigurationDefinitionExtendedSettingsDirective(UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationDefinitionAPIService) {
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
                var ctor = new QualityConfigurationDefinitionExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "extendedSettingsCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationDefinition/Templates/QualityConfigurationDefinitionExtendedSettingsTemplate.html"
        };

        function QualityConfigurationDefinitionExtendedSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

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
                    var directivePayload;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var qualityConfigurationDefinitionExtendedSettings;

                    if (payload != undefined) {
                        qualityConfigurationDefinitionExtendedSettings = payload.qualityConfigurationDefinitionExtendedSettings;
                    }

                    var getQualityConfigurationDefinitionExtendedSettingsTemplateConfigsPromise = getQualityConfigurationDefinitionExtendedSettingsTemplateConfigs();
                    promises.push(getQualityConfigurationDefinitionExtendedSettingsTemplateConfigsPromise);

                    if (qualityConfigurationDefinitionExtendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getQualityConfigurationDefinitionExtendedSettingsTemplateConfigs() {
                        return WhS_Routing_QualityConfigurationDefinitionAPIService.GetQualityConfigurationDefinitionExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (qualityConfigurationDefinitionExtendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, qualityConfigurationDefinitionExtendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                qualityConfigurationDefinitionExtendedSettings: qualityConfigurationDefinitionExtendedSettings
                            };
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

    app.directive('vrWhsRoutingQualityconfigurationdefinitionExtendedsettings', QualityConfigurationDefinitionExtendedSettingsDirective);

})(app);