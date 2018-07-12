(function (app) {

    'use strict';

    RuleActionsSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPBusinessRuleSetAPIService', 'BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService'];

    function RuleActionsSelective(UtilsService, VRUIUtilsService, BusinessProcess_BPBusinessRuleSetAPIService, BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService) {
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
                var ruleActionSelective = new RuleActionSelective($scope, ctrl, $attrs);
                ruleActionSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function RuleActionSelective($scope, ctrl, $attrs) {
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
                var businessRuleDefinitionId;
                var bPDefinitionSettings;

                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var filter;

                    if (payload != undefined) {
                        filter = payload.filter;
                    }

                    var getRuleActionsExtensionConfigsPromise = getRuleActionsExtensionConfigs();
                    promises.push(getRuleActionsExtensionConfigsPromise);

                    function getRuleActionsExtensionConfigs() {
                        var serializedFilter = UtilsService.serializetoJson(filter);
                        return BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.GetRuleActionsExtensionConfigs(serializedFilter).then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (bPDefinitionSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, bPDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
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
            var withemptyline = 'withemptyline';
            var label = "label='Actions'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }
            var template = ' <vr-select on-ready="scopeModel.onSelectorReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + label
                        + ' isrequired="true"'
                        + 'hideremoveicon>'
                    + '</vr-select>'
                    + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
            return template;

        }
    }

    app.directive('bpBusinessRuleActionsSelective', RuleActionsSelective);

})(app);