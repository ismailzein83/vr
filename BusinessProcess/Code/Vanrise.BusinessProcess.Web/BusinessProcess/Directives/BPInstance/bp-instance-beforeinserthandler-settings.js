(function (app) {

    'use strict';

    beforeInsertHandlerSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceAPIService'];

    function beforeInsertHandlerSettingsDirective(UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceAPIService) {
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
                var ctor = new BeforeInsertHandlerSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "beforeInsertHandlerSettingsCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function getTamplate(attrs) {

            var label = "Handler Type";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{beforeInsertHandlerSettingsCtrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' label="' + label + '"'
                            + ' isrequired="beforeInsertHandlerSettingsCtrl.isrequired"'
                            + ' ' + hideremoveicon + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{beforeInsertHandlerSettingsCtrl.normalColNum}}" isrequired="beforeInsertHandlerSettingsCtrl.isrequired" '
                        + ' customvalidate="beforeInsertHandlerSettingsCtrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }

        function BeforeInsertHandlerSettingsCtor($scope, ctrl, $attrs) {
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
                var serviceSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var bpInstanceBeforeInsertHandler;

                    if (payload != undefined) {
                        bpInstanceBeforeInsertHandler = payload.bpInstanceBeforeInsertHandler;
                    }

                    if (bpInstanceBeforeInsertHandler != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSettingsConfigsPromise = getSettingsConfigs();
                    promises.push(getSettingsConfigsPromise);

                    function getSettingsConfigs() {
                        return BusinessProcess_BPInstanceAPIService.GetBPInstanceBeforeInsertHandlerConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (bpInstanceBeforeInsertHandler != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, bpInstanceBeforeInsertHandler.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload;
                            if (bpInstanceBeforeInsertHandler != undefined) {
                                directivePayload = { bpInstanceBeforeInsertHandler: bpInstanceBeforeInsertHandler };
                            }
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
    }

    app.directive('bpInstanceBeforeinserthandlerSettings', beforeInsertHandlerSettingsDirective);
})(app);