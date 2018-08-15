"use strict";

app.directive("vrAnalyticAutomatedreportActiontypeSelective", ["UtilsService", "VR_Analytic_AutomatedReportHandlerSettingsAPIService", "VRUIUtilsService",
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
            var actionTypeSelective = new ActionTypeSelective($scope, ctrl);
            actionTypeSelective.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };


    function ActionTypeSelective($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var selectorAPI;
        var directiveAPI;
        var directiveReadyDeferred;

        var context;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onActionTypeSelectiveReady = function (api) {
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
                var actionType;
                if (payload != undefined) {
                    actionType = payload.actionType;
                    context = payload.context;
                }
                if (actionType != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }
                var getAutomatedReportActionTypesTemplateConfigsPromise = getAutomatedReportHandlerActionTypesTemplateConfigs();
                promises.push(getAutomatedReportActionTypesTemplateConfigsPromise);

                function getAutomatedReportHandlerActionTypesTemplateConfigs() {

                    return VR_Analytic_AutomatedReportHandlerSettingsAPIService.GetAutomatedReportHandlerActionTypesTemplateConfigs().then(function (response) {

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (actionType != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, actionType.ConfigId, 'ExtensionConfigurationId');
                            }
                        }
                    });
                }

                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { actionType: actionType, context: getContext() };
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
                    + ' <vr-select on-ready="scopeModel.onActionTypeSelectiveReady"'
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