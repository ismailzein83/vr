(function (app) {

    'use strict';

    remotecommunicatorsettingsSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_RemoteCommunicatorSettingsAPIService'];

    function remotecommunicatorsettingsSelective(UtilsService, VRUIUtilsService, VRCommon_RemoteCommunicatorSettingsAPIService) {
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
                var ctor = new remoteCommunicatorSettingsSelective($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "remoteCommunicatorSettingsSelectiveCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        }

        function remoteCommunicatorSettingsSelective($scope, ctrl, $attrs) {
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
                    var directivePayload = {
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var remoteCommunicatorSettings;

                    if (payload != undefined) {
                        remoteCommunicatorSettings = payload.remoteCommunicatorSettings;
                    }

                    var getConfigsPromise = getConfigs();
                    promises.push(getConfigsPromise);

                    if (remoteCommunicatorSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getConfigs() {
                        return VRCommon_RemoteCommunicatorSettingsAPIService.GetRemoteCommunicatorSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (remoteCommunicatorSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, remoteCommunicatorSettings.ConfigId, 'ExtensionConfigurationId');
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
                                settings: remoteCommunicatorSettings.Settings
                            };

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    var rootPromiseNode = { promises: promises };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
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

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {
            var label = "Communicator Settings Type";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{remoteCommunicatorSettingsSelectiveCtrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + ' label="' + label + '"'
                + ' isrequired="remoteCommunicatorSettingsSelectiveCtrl.isrequired"'
                + ' ' + hideremoveicon + ' >'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined && scopeModel.selectedTemplateConfig.Editor != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{remoteCommunicatorSettingsSelectiveCtrl.normalColNum}}" isrequired="remoteCommunicatorSettingsSelectiveCtrl.isrequired" customvalidate="remoteCommunicatorSettingsSelectiveCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrCommonRemotecommunicatorsettingsSelective', remotecommunicatorsettingsSelective);

})(app);