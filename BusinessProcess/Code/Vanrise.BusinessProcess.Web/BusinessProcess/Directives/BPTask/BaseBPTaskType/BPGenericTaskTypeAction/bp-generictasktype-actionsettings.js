//'use strict';

//app.directive('bpGenerictasktypeActionsettings', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPTaskTypeAPIService',
//    function (UtilsService, VRUIUtilsService, BusinessProcess_BPTaskTypeAPIService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope: {
//                onReady: "=",
//                normalColNum: '@',
//            },

//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var genericTaskTypeActionSettings = new GenericTaskTypeActionSettings($scope, ctrl, $attrs);
//                genericTaskTypeActionSettings.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            template: function (element, attrs) {
//                return getTemplate(attrs);
//            }
//        };

//        function GenericTaskTypeActionSettings($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var settings;
//            var selectorAPI;

//            var directiveAPI;
//            var directiveReadyDeferred;


//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.templateConfigs = [];
//                $scope.scopeModel.selectedTemplateConfig;

//                $scope.scopeModel.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;
//                    var payload = {
//                    };
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
//                };
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        if (payload.settings != undefined) {
//                            settings = payload.settings;
//                        }
//                    }

//                    function getBPGenericTaskTypeActionSettingsExtensionConfigsPromise() {
//                        return BusinessProcess_BPTaskTypeAPIService.GetBPGenericTaskTypeActionSettingsExtensionConfigs().then(function (response) {
//                            if (response != null) {
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.templateConfigs.push(response[i]);
//                                }
//                                if (settings != undefined) {
//                                    $scope.scopeModel.selectedTemplateConfig =
//                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
//                                }
//                            }
//                        });
//                    }

//                    function loadDirective() {
//                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
//                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                        directiveReadyDeferred.promise.then(function () {
//                            directiveReadyDeferred = undefined;
//                            var payload = {
//                            };
//                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadDeferred);
//                        });

//                        return directiveLoadDeferred.promise;
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];
//                            directivePromises.push(getBPGenericTaskTypeActionSettingsExtensionConfigsPromise());

//                            if (settings != undefined)
//                                directivePromises.push(loadDirective());

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    var data;
//                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
//                        data = directiveAPI.getData();
//                        if (data != undefined) {
//                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
//                        }
//                    }
//                    return data;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }
//        }

//        function getTemplate(attrs) {
//            var label = "label='Action Settings'";
//            if (attrs.hidelabel != undefined) {
//                label = "";
//            }
//            var template = '<vr-columns colnum="{{ctrl.normalColNum}}">'
//                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
//                + ' datasource="scopeModel.templateConfigs"'
//                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
//                + ' datavaluefield="ExtensionConfigurationId"'
//                + ' datatextfield="Title"'
//                + label
//                + ' isrequired="true"'
//                + 'hideremoveicon>'
//                + '</vr-select>'
//                + '</vr-columns>'
//                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
//            return template;

//        }
//        return directiveDefinitionObject;
//    }]);