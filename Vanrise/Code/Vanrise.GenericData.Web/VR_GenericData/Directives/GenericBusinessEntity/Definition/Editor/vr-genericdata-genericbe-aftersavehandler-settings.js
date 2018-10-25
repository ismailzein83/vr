(function (app) {

    'use strict';

    afterSaveSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService','VR_GenericData_HandlerOperationTypeEnum'];

    function afterSaveSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_HandlerOperationTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "afterSaveSettingsCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;
                $scope.scopeModel.handlerOperationTypes = UtilsService.getArrayEnum(VR_GenericData_HandlerOperationTypeEnum);
                $scope.scopeModel.selectedHandlerOperationTypes = [];

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivepPayload = {
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivepPayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                var serviceSettings;

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }
                    if (settings != undefined) {
                        if (settings.HandlerOperationTypes != undefined && settings.HandlerOperationTypes.length > 0) {
                            for (var i = 0; i < settings.HandlerOperationTypes.length; i++) {
                                var item = UtilsService.getItemByVal($scope.scopeModel.handlerOperationTypes, settings.HandlerOperationTypes[i], "value");
                                if (item != null)
                                    $scope.scopeModel.selectedHandlerOperationTypes.push(item);
                            }
                        }
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSettingsConfigsPromise = getSettingsConfigs();
                    promises.push(getSettingsConfigsPromise);

                    function getSettingsConfigs() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEOnAfterSaveHandlerSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (settings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
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
                                context: getContext()
                            };
                            if (settings != undefined) {
                                directivePayload.settings = settings;
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
                            if ($scope.scopeModel.selectedHandlerOperationTypes != null && $scope.scopeModel.selectedHandlerOperationTypes.length > 0) {
                                data.HandlerOperationTypes = [];
                                for (var i = 0; i < $scope.scopeModel.selectedHandlerOperationTypes.length; i++) {
                                    data.HandlerOperationTypes.push($scope.scopeModel.selectedHandlerOperationTypes[i].value);
                                }
                            }
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        function getTamplate(attrs) {
            var label = "Handler Type";
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;
            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{afterSaveSettingsCtrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' label="' + label + '"'
                            + ' isrequired="afterSaveSettingsCtrl.isrequired"'
                            + ' '+ hideremoveicon + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                    + '<vr-columns colnum="{{afterSaveSettingsCtrl.normalColNum}}">'
                            + ' <vr-select'
                            + ' datasource="scopeModel.handlerOperationTypes"'
                            + ' selectedvalues="scopeModel.selectedHandlerOperationTypes"'
                            + ' datavaluefield="value"'
                            + ' datatextfield="description"'
                            + ' label=" Handler Operation Types"'
                            + ' ismultipleselection >'
                            + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{afterSaveSettingsCtrl.normalColNum}}" isrequired="afterSaveSettingsCtrl.isrequired" customvalidate="afterSaveSettingsCtrl.customvalidate">'
                + '</vr-directivewrapper>';
            return template;
        }
    }

    app.directive('vrGenericdataGenericbeAftersavehandlerSettings', afterSaveSettingsDirective);

})(app);