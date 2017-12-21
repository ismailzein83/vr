(function (app) {

    'use strict';

    SubviewDefinitionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];

    function SubviewDefinitionSettingsDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                customvalidate: '=',
                isrequired: '@',
                label: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SubviewDefinitionSettingsTemplateCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RecordSearch/SourceSettings/Templates/DRSearchPageSubviewDefinitionSettingsTemplate.html"
        };

        function SubviewDefinitionSettingsTemplateCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

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

                    var directivePayload = { dataRecordTypeId: dataRecordTypeId };

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var subviewDefinitionSettings;

                    if (payload != undefined) {
                        subviewDefinitionSettings = payload.subviewDefinitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                    }

                    var getSubviewDefinitionSettingsTemplateConfigsPromise = getSubviewDefinitionSettingsTemplateConfigs();
                    promises.push(getSubviewDefinitionSettingsTemplateConfigsPromise);

                    if (subviewDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getSubviewDefinitionSettingsTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetDRSearchPageSubviewDefinitionSettingsConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (subviewDefinitionSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, subviewDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
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
                                subviewDefinitionSettings: subviewDefinitionSettings,
                                dataRecordTypeId: dataRecordTypeId
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

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDatarecordsearchpageSubviewdefinitionSettings', SubviewDefinitionSettingsDirective);

})(app);