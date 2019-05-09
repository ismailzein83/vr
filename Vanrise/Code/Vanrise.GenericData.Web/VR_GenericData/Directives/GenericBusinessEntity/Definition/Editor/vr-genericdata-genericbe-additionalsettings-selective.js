
(function (app) {

    'use strict';

    AdditionalSettingsSelectiveDirective.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AdditionalSettingsSelectiveDirective(VR_GenericData_GenericBEDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AdditionalSettingsSelectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };
        function getDirectiveTemplate(attrs) {
            var template =
                ' <vr-row>'
                + '     <vr-columns colnum="{{ctrl.normalColNum}}">'
                + '         <vr-select on-ready="scopeModel.onSelectorReady"'
                + '         datasource="scopeModel.templateConfigs"'
                + '         selectedvalues="scopeModel.selectedTemplateConfig"'
                + '         datavaluefield="ExtensionConfigurationId"'
                + '         datatextfield="Title"'
                + '         isrequired="true"'
                + '         label="Additional Settings"'
                + '         hideremoveicon>'
                + '         </vr-select>'
                + '     </vr-columns>'
                + ' </vr-row>'
                + ' <vr-row>'
                + '     <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" vr-loader="scopeModel.isLoadingDirective"'
                + '     on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                + '     </vr-directivewrapper>'
                + ' </vr-row>';
            return template;
        }
        function AdditionalSettingsSelectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var configId;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                        directivePayload = payload;
                    }

                    var getAdditionalSettingsConfigsPromise = getAdditionalSettingsConfigs();
                    promises.push(getAdditionalSettingsConfigsPromise);

                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectiveDeferred.promise);

                    getAdditionalSettingsConfigsPromise.then(function () {
                        if (configId != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, configId, 'ExtensionConfigurationId');

                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
                            });
                        }
                        else {
                            loadDirectiveDeferred.resolve();
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getAdditionalSettingsConfigs() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEAdditionalSettingsConfigs().then(function (response) {
                            if (response != null) {
                                selectorAPI.clearDataSource();
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {
                    var data = null;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined) {
                        data = directiveAPI.getData();
                        data.objectSettings.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                    return data;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericbeAdditionalsettingsSelective', AdditionalSettingsSelectiveDirective);

})(app);