'use strict';

app.directive('bpVrWorkflowTaskassignees', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPTaskTypeAPIService',
    function (UtilsService, VRUIUtilsService, BusinessProcess_BPTaskTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var taskAssignees = new TaskAssignees($scope, ctrl, $attrs);
                taskAssignees.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function TaskAssignees($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var settings;
            var getWorkflowArguments;
            var getParentVariables;
            var isVRWorkflowActivityDisabled;

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
                    var payload = {
                        getParentVariables: getParentVariables,
                        getWorkflowArguments: getWorkflowArguments,
                        isVRWorkflowActivityDisabled: isVRWorkflowActivityDisabled,
                    };
                    if (settings != undefined && settings.UserIds) {
                        payload.userIds = settings.UserIds;
                    }
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var bPDefinitionSettings;

                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined) {
                        settings = payload.assigneesSettings;
                        getWorkflowArguments = payload.getWorkflowArguments;
                        getParentVariables = payload.getParentVariables;
                        isVRWorkflowActivityDisabled = payload.isVRWorkflowActivityDisabled;
                    }

                    if (settings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getBPWorkflowTaskAssigneesSettingExtensionConfigsPromise = getBPWorkflowTaskAssigneesSettingExtensionConfigs();
                    promises.push(getBPWorkflowTaskAssigneesSettingExtensionConfigsPromise);

                    function getBPWorkflowTaskAssigneesSettingExtensionConfigs() {
                        return BusinessProcess_BPTaskTypeAPIService.GetVRWorkflowTaskAssigneesSettingExtensionConfigs().then(function (response) {
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
                            console.log(settings);
                            var payload = {
                                getParentVariables: getParentVariables,
                                getWorkflowArguments: getWorkflowArguments,
                                isVRWorkflowActivityDisabled: isVRWorkflowActivityDisabled,
                                userIds: settings.UserIds
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadDeferred);
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
            var withemptyline = 'withemptyline';
            var label = "label='Task Assignees'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + label
                + ' isrequired="true"'
                + 'hideremoveicon>'
                + '</vr-select>'
                + '</vr-columns>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
            return template;

        }
        return directiveDefinitionObject;
    }]);