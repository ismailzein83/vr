(function (app) {

    'use strict';

    DAProfCalcAlertRuleFilterDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];

    function DAProfCalcAlertRuleFilterDefinitionDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
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
                var ctor = new DAProfCalcAlertRuleFilterDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function DAProfCalcAlertRuleFilterDefinitionCtor($scope, ctrl, $attrs) {
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
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var daProfCalcAlertRuleFilterDefinition;

                    if (payload != undefined) {
                        daProfCalcAlertRuleFilterDefinition = payload.daProfCalcAlertRuleFilterDefinition;
                    }

                    if (daProfCalcAlertRuleFilterDefinition != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getDAProfCalcAlertRuleFilterDefinitionConfigsPromise = getDAProfCalcAlertRuleFilterDefinitionConfigs();
                    promises.push(getDAProfCalcAlertRuleFilterDefinitionConfigsPromise);

                    function getDAProfCalcAlertRuleFilterDefinitionConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetDAProfCalcAlertRuleFilterDefinitionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (daProfCalcAlertRuleFilterDefinition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, daProfCalcAlertRuleFilterDefinition.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { daProfCalcAlertRuleFilterDefinition: daProfCalcAlertRuleFilterDefinition };
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

            var template =
                  ' <vr-columns colnum="{{ctrl.normalColNum}}">'
                    + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + ' label="Filter Type">'
                    + ' </vr-select>'
                + ' </vr-columns>'
                + ' <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" vr-loader="scopeModel.isLoadingDirective"'
                    + ' on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                + ' </vr-directivewrapper>';

            return template;
        }
    }

    app.directive('vrAnalyticDaprofcalcAlertrulefilterdefinition', DAProfCalcAlertRuleFilterDefinitionDirective);

})(app);