(function (app) {

    'use strict';

    expressionFieldSettingsSelective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_RDBDataRecordStorageAPIService'];

    function expressionFieldSettingsSelective(UtilsService, VRUIUtilsService, VR_GenericData_RDBDataRecordStorageAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExpressionFieldSettingsSelectiveController($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/RDBDataRecordStorageExpressionFieldSettingsSelective.html"
        };

        function ExpressionFieldSettingsSelectiveController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var expressionFieldSettings;
            var context = [];
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
                    var directivePayload = {
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var initialPromises = [];

                    if (payload != undefined) {
                        expressionFieldSettings = payload.settings;
                        context = payload.context;
                    }

                    initialPromises.push(getExpressionFieldSettingsConfigs());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (expressionFieldSettings != undefined) {
                                directivePromises.push(loadDirective());
                            }

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
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

            function getExpressionFieldSettingsConfigs() {
                return VR_GenericData_RDBDataRecordStorageAPIService.GetRDBDataRecordStorageExpressionFieldSettingsConfigs().then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }

                        if (expressionFieldSettings != undefined) {
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, expressionFieldSettings.ConfigId, 'ExtensionConfigurationId');
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
                        expressionFieldSettings: expressionFieldSettings,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

        }
    }

    app.directive('vrGenericdataDatarecordstorageRdbexpressionfieldsettingsSelective', expressionFieldSettingsSelective);

})(app);