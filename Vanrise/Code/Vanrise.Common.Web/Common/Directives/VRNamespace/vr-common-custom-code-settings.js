(function (appControllers) {

    'use strict';

    VRCommonCustomCodeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService','VRCommon_VRNamespaceAPIService'];

    function VRCommonCustomCodeSettingsDirective(UtilsService, VRUIUtilsService, VRCommon_VRNamespaceAPIService) {
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
                var ctor = new VRCommonCustomCodeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function VRCommonCustomCodeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var vrCustomCodeSettingsEntity;

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

            function loadDirective(directivePayload) {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });
                return directiveLoadDeferred.promise;
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    
                    selectorAPI.clearDataSource();
                    var promises = [];
                    if (payload != undefined && payload.Settings!=undefined) {
                        vrCustomCodeSettingsEntity = payload.Settings;
                    }
                    if (vrCustomCodeSettingsEntity != undefined) {

                        var directivePayload = {
                            vrCustomCodeSettingsEntity: vrCustomCodeSettingsEntity
                        };
                        var loadDirectivePromise = loadDirective(directivePayload);
                        promises.push(loadDirectivePromise);
                    }

                    var getVRCustomCodeSettingsConfigsPromise = getVRCustomCodeSettingsConfigs();
                    promises.push(getVRCustomCodeSettingsConfigsPromise);

                    function getVRCustomCodeSettingsConfigs() {
                        return VRCommon_VRNamespaceAPIService.GetVRDynamicCodeSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrCustomCodeSettingsEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrCustomCodeSettingsEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined && data.ConfigId == undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                api.clear = function () {
                    selectorAPI.clearDataSource();
                };
                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                    '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Type" '
                            + ' ' + hideremoveicon + ' '
                             + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
               

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    appControllers.directive('vrCommonCustomCodeSettings', VRCommonCustomCodeSettingsDirective);

})(appControllers);

