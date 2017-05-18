(function (app) {

    'use strict';

    VRRuleDefinitionExtendedSettingsDirective.$inject = ['VRCommon_VRRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRRuleDefinitionExtendedSettingsDirective(VRCommon_VRRuleDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                var ctor = new VRRuleDefinitionExtendedSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRRule/VRRuleDefinition/Templates/VRRuleDefinitionExtendedSettingsTemplate.html'
        };

        function VRRuleDefinitionExtendedSettingsCtor($scope, ctrl, $attrs) {
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
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var vrRuleDefinitionExtendedSettings;

                    if (payload != undefined) {
                        vrRuleDefinitionExtendedSettings = payload.vrRuleDefinitionExtendedSettings;
                    }

                    var getVRRuleDefinitionExtendedSettingsConfigsPromise = GetVRRuleDefinitionExtendedSettingsConfigsPromise();
                    promises.push(getVRRuleDefinitionExtendedSettingsConfigsPromise);

                    if (vrRuleDefinitionExtendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function GetVRRuleDefinitionExtendedSettingsConfigsPromise() {
                        return VRCommon_VRRuleDefinitionAPIService.GetVRRuleDefinitionExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrRuleDefinitionExtendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrRuleDefinitionExtendedSettings.ConfigId, 'ExtensionConfigurationId');
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
                                vrRuleDefinitionExtendedSettings: vrRuleDefinitionExtendedSettings
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

    app.directive('vrCommonRuledefinitionExtendedsettings', VRRuleDefinitionExtendedSettingsDirective);

})(app);