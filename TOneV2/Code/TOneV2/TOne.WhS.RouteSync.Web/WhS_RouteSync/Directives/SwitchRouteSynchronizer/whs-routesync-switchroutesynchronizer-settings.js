(function (app) {

    'use strict';

    SwitchSynchronizerSettingsDirective.$inject = ['WhS_RouteSync_SwitchRouteSynchronizerAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SwitchSynchronizerSettingsDirective(WhS_RouteSync_SwitchRouteSynchronizerAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var switchSynchronizerSettings = new SwitchSynchronizerSettings($scope, ctrl, $attrs);
                switchSynchronizerSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/SwitchRouteSynchronizer/Templates/SwitchRouteSynchronizerTemplate.html'
        };

        function SwitchSynchronizerSettings($scope, ctrl, $attrs) {
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
                    var switchSynchronizerSettings;

                    if (payload != undefined) {
                        switchSynchronizerSettings = payload.switchSynchronizerSettings;
                    }

                    if (switchSynchronizerSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSwitchSynchronizerSettingsTemplateConfigsPromise = getSwitchSynchronizerSettingsTemplateConfigs();
                    promises.push(getSwitchSynchronizerSettingsTemplateConfigsPromise);

                    function getSwitchSynchronizerSettingsTemplateConfigs() {
                        return WhS_RouteSync_SwitchRouteSynchronizerAPIService.GetSwitchRouteSynchronizerExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (switchSynchronizerSettings != undefined ) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, switchSynchronizerSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { switchSynchronizerSettings: switchSynchronizerSettings };
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
                    return {
                        SwitchRouteSynchronizer: data
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncSwitchroutesynchronizerSettings', SwitchSynchronizerSettingsDirective);

})(app);