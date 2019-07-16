﻿(function (app) {

    'use strict';

    customActionDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function customActionDefinitionSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
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
                var ctor = new CustomActionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "customActionDefinitionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function CustomActionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var context;
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
                    var directivepPayload = {
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivepPayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }

                    if (settings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getParameterSettingsConfigsPromise = getGenericBECustomActionDefinitionSettingsConfigs();
                    promises.push(getParameterSettingsConfigsPromise);

                    function getGenericBECustomActionDefinitionSettingsConfigs() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBECustomActionDefinitionSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (settings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
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
                                context: getContext()
                            };
                            if (settings != undefined) {
                                directivePayload.settings = settings;
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
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        function getTamplate(attrs) {
            var label = "Custom Action Type";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{customActionDefinitionCtrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="' + label + '"'
                            + ' isrequired="true"'
                            + 'hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{customActionDefinitionCtrl.normalColNum/2}}" isrequired="customActionDefinitionCtrl.isrequired" customvalidate="customActionDefinitionCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrGenericdataGenericbeCustomactiondefinitionSettings', customActionDefinitionSettingsDirective);

})(app);