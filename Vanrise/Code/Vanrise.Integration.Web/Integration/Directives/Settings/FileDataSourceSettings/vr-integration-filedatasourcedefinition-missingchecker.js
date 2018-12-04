(function (app) {

    "use strict";

    fileMissingCheckerSettingsDirective.$inject = ["UtilsService", "VRUIUtilsService", "VR_Integration_DataSourceSettingAPIService"];

    function fileMissingCheckerSettingsDirective(UtilsService, VRUIUtilsService, VR_Integration_DataSourceSettingAPIService) {
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
                ctrl.datasource = [];
                ctrl.selectedvalues;

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new FileMissingCheckerSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'missingCheckerCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function FileMissingCheckerSettingsCtor($scope, ctrl, attrs) {
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
                    var fileMissingChecker;

                    if (payload != undefined)
                        fileMissingChecker = payload.fileMissingChecker;

                    var getFileMissingCheckerSettingsConfigsPromise = getFileMissingCheckerSettingsConfigs();
                    promises.push(getFileMissingCheckerSettingsConfigsPromise);

                    if (fileMissingChecker != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getFileMissingCheckerSettingsConfigs() {
                        return VR_Integration_DataSourceSettingAPIService.GetFileMissingCheckerSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (fileMissingChecker != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, fileMissingChecker.ConfigId, 'ExtensionConfigurationId');
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
                                fileMissingChecker: fileMissingChecker
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

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
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
                '<vr-columns colnum="{{missingCheckerCtrl.normalColNum}}">'
                + '	<vr-select on-ready="scopeModel.onSelectorReady"'
                + '	datasource="scopeModel.templateConfigs"'
                + '	selectedvalues="scopeModel.selectedTemplateConfig"'
                + '	datavaluefield="ExtensionConfigurationId"'
                + '	datatextfield="Title"'
                + ' ' + hideremoveicon + ' '
                + '	label="File Type"'
                + '	isrequired="missingCheckerCtrl.isrequired">'
                + '	</vr-select>'
                + '</vr-columns>'
                +'<span vr-loader="scopeModel.isLoadingDirective">'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                + '	on-ready="scopeModel.onDirectiveReady" normal-col-num="{{missingCheckerCtrl.normalColNum}}" isrequired="{{missingCheckerCtrl.isrequired}}" customvalidate="missingCheckerCtrl.customvalidate">'
                + '</vr-directivewrapper>'
                +'</span>';

            return template;
        }
    }

    app.directive("vrIntegrationFiledatasourcedefinitionMissingchecker", fileMissingCheckerSettingsDirective);
})(app);