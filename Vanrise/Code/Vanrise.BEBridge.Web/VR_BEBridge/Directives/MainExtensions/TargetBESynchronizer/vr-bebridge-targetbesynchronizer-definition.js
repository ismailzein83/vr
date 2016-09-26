﻿(function (app) {

    'use strict';

    function targetbesynchronizerDefinition(beRecieveDefinitionApiService, utilsService, vruiUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var targetbesynchronizer = new Targetbesynchronizer($scope, ctrl, $attrs);
                targetbesynchronizer.initializeController();
            },
            controllerAs: "targetbesynchronizerCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function Targetbesynchronizer($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.extensionConfigs = [];
                $scope.selectedExtensionConfig;
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = undefined;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    var settings;
                    if (payload != undefined) {
                        settings = payload.targetBESynchronizer;
                    }
                    var loadDefinitionExtensionConfigsPromise = loadDefinitionExtensionConfigs();
                    promises.push(loadDefinitionExtensionConfigsPromise);

                    if (settings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    function loadDefinitionExtensionConfigs() {
                        return beRecieveDefinitionApiService.GetTargetSynchronizerExtensionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (settings != undefined)
                                    $scope.selectedExtensionConfig = utilsService.getItemByVal($scope.extensionConfigs, settings.ConfigId, 'ExtensionConfigurationId');
                                else if ($scope.extensionConfigs.length > 0)
                                    $scope.selectedExtensionConfig = $scope.extensionConfigs[0];
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = utilsService.createPromiseDeferred();
                        var directiveLoadDeferred = utilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = settings;
                            vruiUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    return utilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    var data = directiveAPI.getData();
                    if (data != undefined)
                        data.ConfigId = $scope.selectedExtensionConfig.ExtensionConfigurationId;
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        function getTemplate(attrs) {
            var label = "label='Target Synchronizer'";

            if (attrs.hidelabel != undefined) {
                label = "label='Target Synchronizers'";
            }

            return '<vr-row><vr-columns colnum="{{targetbesynchronizerCtrl.normalColNum * 2}}">'
                   + '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="targetbesynchronizerCtrl.isrequired" hideremoveicon></vr-select>'
               + '</vr-columns></vr-row>'
               + '<vr-row><vr-directivewrapper directive="selectedExtensionConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{targetbesynchronizerCtrl.normalColNum}}" isrequired="targetbesynchronizerCtrl.isrequired" customvalidate="targetbesynchronizerCtrl.customvalidate"></vr-directivewrapper></vr-row>';
        }
    }

    targetbesynchronizerDefinition.$inject = ['VR_BEBridge_BERecieveDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    app.directive('vrBebridgeTargetbesynchronizerDefinition', targetbesynchronizerDefinition);
})(app);