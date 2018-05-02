"use strict";

app.directive("vrRuntimeRuntimenodeconfigurationRuntimeservice", ["UtilsService", "VRRuntime_RuntimeNodeConfigurationAPIService", "VRUIUtilsService",
function (UtilsService, VRRuntime_RuntimeNodeConfigurationAPIService, VRUIUtilsService) {
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
        var directiveReadyDeferred;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onRuntimeServiceReady = function (api) {
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
                var runtimeService;
                if (payload != undefined) {
                    runtimeService = payload.runtimeService;
                }
                if (runtimeService != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }
                var getRuntimeServiceTemplateConfigsPromise = getRuntimeServiceTemplateConfigs();
                promises.push(getRuntimeServiceTemplateConfigsPromise);

                function getRuntimeServiceTemplateConfigs() {
                    return VRRuntime_RuntimeNodeConfigurationAPIService.GetRuntimeServiceTypeTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (runtimeService != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, runtimeService.ConfigId, 'ExtensionConfigurationId');
                            }
                        }
                    });
                }

                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        directiveReadyDeferred = undefined;
                        var directivePayload = { runtimeService: runtimeService };
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


    function getTemplate(attrs) {
        var hideremoveicon = '';
        if (attrs.hideremoveicon != undefined) {
            hideremoveicon = 'hideremoveicon';
        }
        var template =
            '<vr-row>'
                + '<vr-columns width="1/2row" colnum="{{ctrl.normalColNum}}">'
                    + ' <vr-select on-ready="scopeModel.onRuntimeServiceReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + 'label="Runtime Service"  entityName="Setting" '
                        + ' ' + hideremoveicon + ' '
                       + 'isrequired >'
                    + '</vr-select>'
                + ' </vr-columns>'
            + '</vr-row>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                    + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
            + '</vr-directivewrapper>';
        return template;
    }
}]);