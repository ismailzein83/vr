(function (app) {
    'use strict';
    SpecialroutingTypeSelective.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonManualRoutesAPIService'];
    function SpecialroutingTypeSelective(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonManualRoutesAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SpecialroutingTypeSelectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/SpecialRouting/Templates/EricssonSpecialroutingTypeSelectiveTemplate.html"
        };

        function SpecialroutingTypeSelectiveCtor($scope, ctrl, $attrs) {
            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var settings;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.selectedExtensionConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = {
                        Settings: settings
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        settings = payload.Settings;
                    }

                    var loadSettingsTypeExtensionConfigsPromise = loadSettingsTypeExtensionConfigs();

                    promises.push(loadSettingsTypeExtensionConfigsPromise);

                    function loadSettingsTypeExtensionConfigs() {
                        return WhS_RouteSync_EricssonManualRoutesAPIService.GetSpecialRoutingTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (settings != undefined) {
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, settings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedExtensionConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                    }
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonSpecialroutingTypeSelective', SpecialroutingTypeSelective);

})(app);