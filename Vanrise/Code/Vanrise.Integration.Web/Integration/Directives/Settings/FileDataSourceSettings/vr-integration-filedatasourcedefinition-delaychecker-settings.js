(function (app) {

    "use strict";

    fileDelayCheckerSettingsDirective.$inject = ["UtilsService", "VRUIUtilsService", "VR_Integration_DataSourceSettingAPIService"];

    function fileDelayCheckerSettingsDirective(UtilsService, VRUIUtilsService, VR_Integration_DataSourceSettingAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "@",
                label: "@",
                customvalidate: "=",
                isrequired: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FileDelayCheckerSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function FileDelayCheckerSettingsCtor($scope, ctrl, attrs) {
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
                    var fileDelayCheckerSettings;

                    if (payload != undefined)
                        fileDelayCheckerSettings = payload.settings;

                    var getFileDelayCheckerSettingsConfigsPromise = getFileDelayCheckerSettingsConfigs();
                    promises.push(getFileDelayCheckerSettingsConfigsPromise);

                    if (fileDelayCheckerSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getFileDelayCheckerSettingsConfigs() {
                        return VR_Integration_DataSourceSettingAPIService.GetFileDelayCheckerSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (fileDelayCheckerSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, fileDelayCheckerSettings.ConfigId, 'ExtensionConfigurationId');
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
                                fileDelayCheckerSettings: fileDelayCheckerSettings
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });
                        return directiveLoadDeferred.promise;
                    }
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

                if (ctrl.onReady != null && typeof ctrl.onReady == "function") {
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
                '<vr-columns  width="1/2row">'
                + '	<vr-select on-ready="scopeModel.onSelectorReady"'
                + '	datasource="scopeModel.templateConfigs"'
                + '	selectedvalues="scopeModel.selectedTemplateConfig"'
                + '	datavaluefield="ExtensionConfigurationId"'
                + '	datatextfield="Title"'
                + ' ' + hideremoveicon + ' '
                + '	label="File Type"'
                + '	isrequired="ctrl.isrequired">'
                + '	</vr-select>'
                + '</vr-columns>'
                + '<span vr-loader="scopeModel.isLoadingDirective">'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig !=undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + '	on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="{{ctrl.isrequired}}" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>'                     
                + '</span>';

            return template;
        }
    }

    app.directive("vrIntegrationFiledatasourcedefinitionDelaycheckerSettings", fileDelayCheckerSettingsDirective);
})(app);