(function (app) {

    'use strict';

    vrGenericdataDatarecordStorageRdbJoinSettingsSelectiveDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_RDBDataRecordStorageAPIService'];

    function vrGenericdataDatarecordStorageRdbJoinSettingsSelectiveDirective(UtilsService, VRUIUtilsService, VR_GenericData_RDBDataRecordStorageAPIService) {
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
                var ctor = new VRGenericdataDatarecordStorageRdbJoinSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/RDBDataRecordStorageJoinSettingsSelectiveTemplate.html"
        };

        function VRGenericdataDatarecordStorageRdbJoinSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var joinSettings;
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
                        joinSettings = payload.joinSettings;
                        context = payload.context;
                    }

                    initialPromises.push(getRDBDataRecordStorageJoinSettingsConfigs());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (joinSettings != undefined) {
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

            function getRDBDataRecordStorageJoinSettingsConfigs() {
                return VR_GenericData_RDBDataRecordStorageAPIService.GetRDBDataRecordStorageJoinSettingsConfigs().then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }

                        if (joinSettings != undefined) {
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, joinSettings.ConfigId, 'ExtensionConfigurationId');
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
                        joinSettings: joinSettings,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

        }
    }

    app.directive('vrGenericdataDatarecordstorageRdbjoinsettingsSelective', vrGenericdataDatarecordStorageRdbJoinSettingsSelectiveDirective);

})(app);