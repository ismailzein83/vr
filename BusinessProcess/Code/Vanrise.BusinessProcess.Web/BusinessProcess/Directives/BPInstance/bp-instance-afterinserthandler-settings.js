(function (app) {

    'use strict';

    afterInsertHandlerSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceAPIService'];

    function afterInsertHandlerSettingsDirective(UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceAPIService) {
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
                var ctor = new AfterInsertHandlerSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "afterInsertHandlerSettingsCtrl",
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
                    + '<vr-columns colnum="{{afterInsertHandlerSettingsCtrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' label="' + label + '"'
                            + ' isrequired="afterInsertHandlerSettingsCtrl.isrequired"'
                            + ' ' + hideremoveicon + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{afterInsertHandlerSettingsCtrl.normalColNum}}" isrequired="afterInsertHandlerSettingsCtrl.isrequired" '
                        + ' customvalidate="afterInsertHandlerSettingsCtrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }

        function AfterInsertHandlerSettingsCtor($scope, ctrl, $attrs) {
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

                    var bpInstanceAfterInsertHandler;

                    if (payload != undefined) {
                        bpInstanceAfterInsertHandler = payload.bpInstanceAfterInsertHandler;
                    }

                    if (bpInstanceAfterInsertHandler != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSettingsConfigsPromise = getSettingsConfigs();
                    promises.push(getSettingsConfigsPromise);

                    function getSettingsConfigs() {
                        return BusinessProcess_BPInstanceAPIService.GetBPInstanceAfterInsertHandlerConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (bpInstanceAfterInsertHandler != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, bpInstanceAfterInsertHandler.ConfigId, 'ExtensionConfigurationId');
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
                            if (bpInstanceAfterInsertHandler != undefined) {
                                directivePayload = { bpInstanceAfterInsertHandler: bpInstanceAfterInsertHandler };
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

    app.directive('bpInstanceAfterinserthandlerSettings', afterInsertHandlerSettingsDirective);
})(app);