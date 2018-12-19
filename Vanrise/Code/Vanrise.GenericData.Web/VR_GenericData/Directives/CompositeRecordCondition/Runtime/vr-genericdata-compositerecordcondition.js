'use strict';

app.directive('vrGenericdataCompositerecordcondition', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CompositeRecordConditionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_CompositeRecordConditionAPIService) {
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
                var ctor = new CompositeRecordCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function CompositeRecordCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var compositeRecordConditionDefinitionGroup;
            var compositeRecordConditionResolvedDataList;

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

                    var directivePayload = {
                        compositeRecordConditionDefinitionGroup: compositeRecordConditionDefinitionGroup,
                        compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
                    };
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

                    var compositeRecordCondition;

                    if (payload != undefined) {
                        compositeRecordCondition = payload.conditionRecordDefinition;
                        compositeRecordConditionDefinitionGroup = payload.compositeRecordConditionDefinitionGroup;
                        compositeRecordConditionResolvedDataList = payload.compositeRecordConditionResolvedDataList;
                    }

                    var getCompositeRecordConditionConfigsPromise = getCompositeRecordConditionConfigs();
                    promises.push(getCompositeRecordConditionConfigsPromise);

                    if (compositeRecordCondition != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getCompositeRecordConditionConfigs() {
                        return VR_GenericData_CompositeRecordConditionAPIService.GetCompositeRecordConditionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (compositeRecordCondition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, compositeRecordCondition.ConfigId, 'ExtensionConfigurationId');
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
                                compositeRecordCondition: compositeRecordCondition,
                                compositeRecordConditionDefinitionGroup: compositeRecordConditionDefinitionGroup,
                                compositeRecordConditionResolvedDataList: compositeRecordConditionResolvedDataList
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
                + ' isrequired="ctrl.isrequired">'
                + '</vr-select>'
                + '</vr-columns>'
                + '<span vr-loader="scopeModel.isLoadingDirective">'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" '
                + ' normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
            + '</span>';

            return template;
        }
    }]);