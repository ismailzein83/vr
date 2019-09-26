'use strict';

app.directive('networkprovisionHandlertypeExtendedsettings', ['NetworkProvision_HandlerTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (NetworkProvision_HandlerTypeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                customvalidate: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HandlerTypeExtendedSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function HandlerTypeExtendedSettings($scope, ctrl, $attrs) {
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
                    var extendedSettings;

                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                    }

                    if (extendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getHandlerTypeExtendedSettingsConfigsPromise = getHandlerTypeExtendedSettingsConfigs();
                    promises.push(getHandlerTypeExtendedSettingsConfigsPromise);

                    function getHandlerTypeExtendedSettingsConfigs() {
                        return NetworkProvision_HandlerTypeAPIService.GetHandlerTypeExtendedSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (extendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
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
                                extendedSettings: extendedSettings
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

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {
            var label = 'Handler Type';
            if (attrs.hidelabel != undefined) {
                label = '';
            }

            var template =
                '<vr-row>'
                + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + label
                + ' isrequired="true"'
                + ' hideremoveicon>'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined"'
                + ' directive="scopeModel.selectedTemplateConfig.DefinitionEditor"'
                + ' on-ready="scopeModel.onDirectiveReady"'
                + ' normal-col-num="{{ctrl.normalColNum}}"'
                + ' isrequired="ctrl.isrequired"'
                + ' customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }
]);