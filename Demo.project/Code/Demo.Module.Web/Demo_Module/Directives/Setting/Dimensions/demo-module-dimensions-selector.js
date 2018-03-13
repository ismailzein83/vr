﻿(function (appControllers) {
    "use strict";
    demoModuleDimensionsSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_SettingConfigsAPIService'];
    function demoModuleDimensionsSelector(UtilsService, VRUIUtilsService, Demo_Module_SettingConfigsAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function($scope, $element, $attrs){
                var ctrl = this;
                var dimensionsSelector = new DimensionsSelector($scope, ctrl, $attrs);
                dimensionsSelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController:true,
            template: function($attrs){
                return getTemplate($attrs)
            }
        };
        function DimensionsSelector($scope, ctrl, $attrs){
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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            };
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var dimensionsEntity;

                    if (payload != undefined) {
                        dimensionsEntity = payload;
                    }
                    if (dimensionsEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    var getDimensionsTemplateConfigsPromise = getDimensionsTemplateConfigs();
                    promises.push(getDimensionsTemplateConfigsPromise);

                    function getDimensionsTemplateConfigs() {

                        return Demo_Module_SettingConfigsAPIService.GetDimensionsTypeTemplateConfigs().then(function (response) {
                            if (response != null) {                              
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (dimensionsEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, dimensionsEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = dimensionsEntity;
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
        function getTemplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Dimensions"  entityName="Dimensions Type" '
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
    app.directive('demoModuleDimensionsSelector', demoModuleDimensionsSelector);    
})(appControllers);