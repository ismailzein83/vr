(function (app) {

    'use strict';

    StyleDefinitionSettingsDirective.$inject = ['VRCommon_StyleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function StyleDefinitionSettingsDirective(VRCommon_StyleDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                var styleDefinitionSettings = new StyleDefinitionSettings($scope, ctrl, $attrs);
                styleDefinitionSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function StyleDefinitionSettings($scope, ctrl, $attrs) {
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
                    var styleDefinitionSettings;

                    if (payload != undefined) {
                        styleDefinitionSettings = payload.styleDefinitionSettings;
                    }

                    if (styleDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getStyleDefinitionSettingsTemplateConfigsPromise = getStyleDefinitionSettingsTemplateConfigs();
                    promises.push(getStyleDefinitionSettingsTemplateConfigsPromise);

                    function getStyleDefinitionSettingsTemplateConfigs() {
                        return VRCommon_StyleDefinitionAPIService.GetStyleFormatingExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (styleDefinitionSettings != undefined && styleDefinitionSettings.StyleFormatingSettings != null) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, styleDefinitionSettings.StyleFormatingSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { styleFormatingSettings: styleDefinitionSettings.StyleFormatingSettings };
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

        function getTamplate(attrs) {

            var template =
                '<vr-row>'
                    + ' <vr-columns width="1/2row">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' isrequired="true"'
                            + ' label="Style Formating"'
                            + ' hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + ' </vr-row>'
                    + ' <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                            + ' on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                    + ' </vr-directivewrapper>'
            return template;
        }
    }

    app.directive('vrCommonStyledefinitionSettings', StyleDefinitionSettingsDirective);

})(app);