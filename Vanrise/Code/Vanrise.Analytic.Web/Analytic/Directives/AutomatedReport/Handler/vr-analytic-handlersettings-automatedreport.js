"use strict";

app.directive("vrAnalyticHandlersettingsAutomatedreport", ["UtilsService", "VR_Analytic_AutomatedReportHandlerSettingsAPIService", "VRUIUtilsService",
function (UtilsService, VR_Analytic_AutomatedReportHandlerSettingsAPIService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            enableadd: '=',
            normalColNum: '@',
            label: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var automatedReport = new AutomatedReport($scope, ctrl);
            automatedReport.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };


    function AutomatedReport($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var selectorAPI;
        var directiveAPI;
        var directiveReadyDeferred;

        var context;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onAutomatedReportReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivePayload = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
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
                var getAutomatedReportTemplateConfigsPromise = GetAutomatedReportTemplateConfigs();
                promises.push(getAutomatedReportTemplateConfigsPromise);

                function GetAutomatedReportTemplateConfigs() {

                    return VR_Analytic_AutomatedReportHandlerSettingsAPIService.GetAutomatedReportHandlerTemplateConfigs().then(function (response) {

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
                        var directivePayload = { settings: settings, context: getContext() };
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

            api.validate = function () {
                return directiveAPI.validate();
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
                + '<vr-columns width="1/2row" colnum="{{ctrl.normalColNum}}">'
                    + ' <vr-select on-ready="scopeModel.onAutomatedReportReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + 'label="Action Type"  entityName="Type" '
                        + ' ' + hideremoveicon + ' '
                       + 'isrequired >'
                    + '</vr-select>'
                + ' </vr-columns>'
            + '</vr-row>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                    + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired >'
            + '</vr-directivewrapper>';
        return template;
    }



}]);