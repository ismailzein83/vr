﻿(function (appControllers) {

    'use strict';

    GeneratedScriptVariableSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Devtools_ColumnsAPIService'];

    function GeneratedScriptVariableSettingsDirective(UtilsService, VRUIUtilsService, VR_Devtools_ColumnsAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },

            controller: function ($scope, $element, $attrs) {


                var ctrl = this;
                var ctor = new GeneratedScriptVariableSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function GeneratedScriptVariableSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var settingsSelectedPromiseDeferred;
            var generatedScriptVariableSettingsEntity;

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

                $scope.scopeModel.onSettingsChanged = function (value) {
                    if (selectorAPI) {
                        if (value) {
                            if (settingsSelectedPromiseDeferred != undefined) {
                                loadDirective(generatedScriptVariableSettingsEntity);
                            }
                        }
                    }

                };

            }

            function loadDirective(directivePayload) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var promises = [];
                    if (payload != undefined) {
                        generatedScriptVariableSettingsEntity = payload;

                    }
                    if (generatedScriptVariableSettingsEntity.settings != undefined) {

                        var directivePayload = generatedScriptVariableSettingsEntity;
                        var loadDirectivePromise = loadDirective(directivePayload);
                        promises.push(loadDirectivePromise);
                    }

                    var getGeneratedScriptVariableSettingsConfigsPromise = getGeneratedScriptVariableSettingsConfigs();
                    promises.push(getGeneratedScriptVariableSettingsConfigsPromise);

                    function getGeneratedScriptVariableSettingsConfigs() {
                        return VR_Devtools_ColumnsAPIService.GetGeneratedScriptVariableSettingsConfigs().then(function (response) {

                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (generatedScriptVariableSettingsEntity != undefined && generatedScriptVariableSettingsEntity.settings!=undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, generatedScriptVariableSettingsEntity.settings.ConfigId, 'ExtensionConfigurationId');
                                }

                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        settingsSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                    });
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined && data.ConfigId == undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                        var title = $scope.scopeModel.selectedTemplateConfig.Title;

                    }
                    return data;
                };

                api.clear = function () {

                    selectorAPI.clearDataSource();

                };
                if (ctrl.onReady != null) {

                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + ' <vr-select on-ready="scopeModel.onSelectorReady" onselectionchanged="scopeModel.onSettingsChanged"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + 'label="Settings" '
                + ' ' + hideremoveicon + ' '
                + 'isrequired ="ctrl.isrequired"'
                + ' >'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    appControllers.directive('vrDevtoolsGeneratedScriptVariableSettings', GeneratedScriptVariableSettingsDirective);

})(appControllers);
