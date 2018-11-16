(function (appControllers) {

    'use strict';

    VRCommonDynamicapiMethodSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService','VRCommon_VRDynamicAPIAPIService'];

    function VRCommonDynamicapiMethodSettingsDirective(UtilsService, VRUIUtilsService,VRCommon_VRDynamicAPIAPIService) {
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
                var ctor = new VRCommonDynamicAPIMethodSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function VRCommonDynamicAPIMethodSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var settingsSelectedPromiseDeferred;
            var vrDynamicAPIMethodSettingsEntity;

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
                        vrDynamicAPIMethodSettingsEntity = payload.Settings;
                    }
                    if (vrDynamicAPIMethodSettingsEntity != undefined) {

                        var directivePayload = {
                            vrDynamicAPIMethodSettingsEntity: vrDynamicAPIMethodSettingsEntity
                        };
                        var loadDirectivePromise = loadDirective(directivePayload);
                        promises.push(loadDirectivePromise);
                    }

                    var getVRDynamicAPIMethodSettingsConfigsPromise = getVRDynamicAPIMethodSettingsConfigs();
                    promises.push(getVRDynamicAPIMethodSettingsConfigsPromise);

                    function getVRDynamicAPIMethodSettingsConfigs() {
                        return VRCommon_VRDynamicAPIAPIService.GetVRDynamicAPIMethodSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (vrDynamicAPIMethodSettingsEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, vrDynamicAPIMethodSettingsEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        settingsSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    });
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
                    '<vr-row><vr-columns colnum="{{ctrl.normalColNum}}" >'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Dynamic API Module Method" '
                            + ' ' + hideremoveicon + ' '
                             + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns></vr-row>'
               

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    appControllers.directive('vrCommonDynamicapiMethodSettings', VRCommonDynamicapiMethodSettingsDirective);

})(appControllers);

