﻿'use strict';

app.directive('vrGenericdataCompositerecordconditiondefinitionSettings', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CompositeRecordConditionDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_CompositeRecordConditionDefinitionAPIService) {
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
                var ctor = new CompositeRecordConditionDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function CompositeRecordConditionDefinitionSettings($scope, ctrl, $attrs) {
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
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    selectorAPI.clearDataSource();

                    var compositeRecordConditionDefinitionSetting;

                    if (payload != undefined) {
                        compositeRecordConditionDefinitionSetting = payload.compositeRecordConditionDefinitionSetting;
                    }

                    var getCompositeRecordConditionDefinitionSettingConfigsPromise = getCompositeRecordConditionDefinitionSettingConfigs();
                    promises.push(getCompositeRecordConditionDefinitionSettingConfigsPromise);

                    if (compositeRecordConditionDefinitionSetting != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getCompositeRecordConditionDefinitionSettingConfigs() {
                        return VR_GenericData_CompositeRecordConditionDefinitionAPIService.GetCompositeRecordConditionDefinitionSettingConfigs().then(function (response) {

                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (compositeRecordConditionDefinitionSetting != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, compositeRecordConditionDefinitionSetting.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload;
                            if (compositeRecordConditionDefinitionSetting != undefined) {
                                directivePayload = { DataRecordTypeId: compositeRecordConditionDefinitionSetting.DataRecordTypeId };
                            }
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

        function getTamplate(attrs) {

            var label = "label='Composite Record Condition'";
            if (attrs.hidelabel != undefined) {
                label = "";
            }
            var template =
                '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + label
                + ' isrequired="ctrl.isrequired"'
                + '>'
                + '</vr-select>'
                + ' </vr-columns>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
            return template;
        }

    }]);