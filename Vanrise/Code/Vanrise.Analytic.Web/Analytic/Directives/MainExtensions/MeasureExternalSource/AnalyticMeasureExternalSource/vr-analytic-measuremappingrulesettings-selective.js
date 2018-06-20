(function (appControllers) {

    "use strict";
    MeasureMappingRuleSettingsSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];
    function MeasureMappingRuleSettingsSelective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var measureMappingRuleSettingsSelective = new MeasureMappingRuleSettingsSelective($scope, ctrl, $attrs);
                measureMappingRuleSettingsSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function ($attrs) {
                return getTemplate($attrs);
            }
        };
        function MeasureMappingRuleSettingsSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var tableId;
            var context;
            var ruleEntity;

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

                    var payload = {
                        context: getContext(),
                        tableId: tableId,
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    selectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined) {

                        ruleEntity = payload.ruleEntity;
                        context = payload.context;
                        tableId = payload.tableId;

                    }

                    if (ruleEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    var getTemplateConfigsPromise = getTemplateConfigs();
                    promises.push(getTemplateConfigsPromise);

                    function getTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetMeasureMappingRuleSettingConfigs().then(function (response) {
                            if (response != null) {

                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);

                                }
                                if (ruleEntity != undefined) {

                                    var measureRuleEntity = ruleEntity.Entity != undefined ? ruleEntity.Entity.Settings : undefined;
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, measureRuleEntity.ConfigId, 'ExtensionConfigurationId');

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
                                ruleEntity: ruleEntity,
                                context: getContext(),
                                tableId: tableId,
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
                            + 'label="Type"  entityName="Measure Mapping Rule Setting" '
                            + ' ' + hideremoveicon + ' '
                            + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + ' vr-loader="scopeModel.isLoadingDirective"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
        return directiveDefinitionObject;
    }
    app.directive("vrAnalyticMeasuremappingrulesettingsSelective", MeasureMappingRuleSettingsSelective);
})(appControllers);