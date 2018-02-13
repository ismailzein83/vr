(function (app) {

    'use strict';

    function sourceReaderDefinitionDirective(beRecieveDefinitionApiService, utilsService, vruiUtilsService) {

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
                var targetBeConvertorDefinition = new TargetBeConvertorDefinition($scope, ctrl, $attrs);
                targetBeConvertorDefinition.initializeController();
            },
            controllerAs: "targetBeConvertorCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function TargetBeConvertorDefinition($scope, ctrl, $attrs) {

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
                        settings = payload.targetBEConvertor;
                    }
                    var loadDefinitionExtensionConfigsPromise = loadDefinitionExtensionConfigs();
                    promises.push(loadDefinitionExtensionConfigsPromise);

                    if (settings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    function loadDefinitionExtensionConfigs() {
                        return beRecieveDefinitionApiService.GetTargetConvertorExtensionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (settings != undefined)
                                    $scope.selectedExtensionConfig = utilsService.getItemByVal($scope.extensionConfigs, settings.ConfigId, 'ExtensionConfigurationId');
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
            var label = "label='Target Converter'";

            if (attrs.hidelabel != undefined) {
                label = "label='Target Converters'";
            }

            return '<vr-row><vr-columns colnum="{{targetBeConvertorCtrl.normalColNum}}">'
                    + '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="targetBeConvertorCtrl.isrequired" hideremoveicon></vr-select>'
                + '</vr-columns></vr-row>'
                + '<vr-directivewrapper directive="selectedExtensionConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{targetBeConvertorCtrl.normalColNum}}" isrequired="targetBeConvertorCtrl.isrequired" customvalidate="targetBeConvertorCtrl.customvalidate"></vr-directivewrapper>';
        }
    }

    sourceReaderDefinitionDirective.$inject = ['VR_BEBridge_BERecieveDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    app.directive('vrBebridgeTargetbeconvertorDefinition', sourceReaderDefinitionDirective);
})(app);