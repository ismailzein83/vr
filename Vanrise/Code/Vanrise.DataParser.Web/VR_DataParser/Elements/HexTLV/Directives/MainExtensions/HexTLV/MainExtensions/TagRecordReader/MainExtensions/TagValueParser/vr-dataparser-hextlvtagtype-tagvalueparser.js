﻿(function (app) {

    'use strict';

    TagValueParserDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_DataParser_ParserTypeConfigsAPIService'];

    function TagValueParserDirective(UtilsService, VRUIUtilsService, VR_DataParser_ParserTypeConfigsAPIService) {
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
                var ctor = new TagValueParserSelector($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function TagValueParserSelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;
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
                    var directivePayload = { context: getContext() };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var tagValueParserEntity;

                    if (payload != undefined) {
                        context = payload.context;
                        tagValueParserEntity = payload.tagValueParserEntity;
                    }

                    if (tagValueParserEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getTagValueParserPromise = getTagValueParserConfigs();
                    promises.push(getTagValueParserPromise);

                    function getTagValueParserConfigs() {

                        return VR_DataParser_ParserTypeConfigsAPIService.GetTagValueParserTemplateConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (tagValueParserEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, tagValueParserEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { context: getContext() };
                            if (tagValueParserEntity != undefined)
                                directivePayload.ValueParser = tagValueParserEntity;
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
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                '<vr-row>'
                    + '<vr-columns width="1/2row">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Parser Type" '
                            + ' ' + hideremoveicon + ' '
                             + 'isrequired ="ctrl.isrequired"'
                           + ' >'


                        + '</vr-select>'
                    + '</vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrDataparserHextlvtagtypeTagvalueparser', TagValueParserDirective);

})(app);