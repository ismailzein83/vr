"use strict";
 
app.directive("vrAnalyticReportgenerationSettingsFilter", ["UtilsService", "VR_Analytic_ReportGenerationAPIService", "VRUIUtilsService",
function (UtilsService, VR_Analytic_ReportGenerationAPIService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            enableadd: '=',
            normalColNum: '@',
            label: '@',
            customvalidate: '=',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var settingSelector = new SettingSelector($scope, ctrl);
            settingSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function SettingSelector($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var selectorAPI;
        var directiveAPI;
        var directiveReadyDeferred ;

        var context;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onFilterReady = function (api) {
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
                var filter;
                if (payload != undefined) {
                    filter = payload.filter;
                    context = payload.context;
                }
                if (filter != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }
                var getFilterTemplateConfigsPromise = getFilterTemplateConfigs();
                promises.push(getFilterTemplateConfigsPromise);

                function getFilterTemplateConfigs() {
                    return VR_Analytic_ReportGenerationAPIService.GetFilterTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (filter != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, filter.ConfigId, 'ExtensionConfigurationId');
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
                            context: getContext()
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
                    + ' <vr-select on-ready="scopeModel.onFilterReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + 'label="Filter"  entityName="Type" '
                        + ' ' + hideremoveicon + ' '
                       + ' >'
                    + '</vr-select>'
                + ' </vr-columns>'
            + '</vr-row>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                    + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
            + '</vr-directivewrapper>';
        return template;
    }
}]);