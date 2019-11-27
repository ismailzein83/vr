(function (app) {

    'use strict';

    whsRoutesyncEricssonbranchroutesettings.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonBranchRouteSettingsAPIService'];

    function whsRoutesyncEricssonbranchroutesettings(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonBranchRouteSettingsAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BranchRouteSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSwitch/BranchRoutes/Templates/EricssonSwitchBranchRoutesSettingsTemplate.html"

        };
        function BranchRouteSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var branchRouteSettingsAPI;
            var branchRouteSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var branchRouteSettingsSelectedPromiseDeferred;
            var context = {};

            function initializeController() {
                $scope.scopeModel = {};
                var initPromises = [];

                $scope.scopeModel.onBranchRouteSettingsReady = function (api) {
                    branchRouteSettingsAPI = api;
                    branchRouteSettingsReadyDeferred.resolve();
                };

                var loadBranchRouteSettingsConfigsDeferred = UtilsService.createPromiseDeferred();
                initPromises.push(loadBranchRouteSettingsConfigsDeferred.promise);

                WhS_RouteSync_EricssonBranchRouteSettingsAPIService.GetEricssonBranchRouteSettingsConfigs().then(function (response) {
                    $scope.scopeModel.branchRouteSettingConfigs = response;
                    loadBranchRouteSettingsConfigsDeferred.resolve();
                });

                UtilsService.waitMultiplePromises(initPromises).then(function () {
                    defineAPI();
                });

                $scope.scopeModel.onBeforeBranchRouteSettingsSelectionChanged = function () {
                    if (branchRouteSettingsSelectedPromiseDeferred != undefined)
                        return;

                    branchRouteSettingsReadyDeferred = UtilsService.createPromiseDeferred();
                };

                $scope.scopeModel.onBranchRouteSettingsSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined)
                        return;

                    if (branchRouteSettingsSelectedPromiseDeferred != undefined) {
                        branchRouteSettingsSelectedPromiseDeferred.resolve();
                    }
                    else {

                        branchRouteSettingsReadyDeferred.promise.then(function () {
                            var setLoader = function (value) {
                                $scope.scopeModel.isDirectiveLoading = value;
                            };

                            var payload = {
                                context: getContext()
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, branchRouteSettingsAPI, payload, setLoader, undefined);
                        });
                    }
                };

                $scope.scopeModel.validateBranchRouteSettings = function () {
                    if ($scope.scopeModel.selectedBranchRouteSettings == undefined)
                        return;

                    if (context != undefined && context.validateBranchRouteSettings != undefined && typeof context.validateBranchRouteSettings == 'function')
                        return context.validateBranchRouteSettings();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var branchRouteSettings;

                    if (payload != undefined)
                        branchRouteSettings = payload.branchRouteSettings;

                    var loadBranchRouteSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    branchRouteSettingsSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                    promises.push(branchRouteSettingsSelectedPromiseDeferred.promise);
                    promises.push(loadBranchRouteSettingsPromiseDeferred.promise);

                    if (branchRouteSettings != undefined)
                        $scope.scopeModel.selectedBranchRouteSettings = UtilsService.getItemByVal($scope.scopeModel.branchRouteSettingConfigs, branchRouteSettings.ConfigId, "ExtensionConfigurationId");
                    else
                        $scope.scopeModel.selectedBranchRouteSettings = $scope.scopeModel.branchRouteSettingConfigs[0];

                    branchRouteSettingsReadyDeferred.promise.then(function () {
                        var payload = {
                            branchRouteSettings: branchRouteSettings,
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(branchRouteSettingsAPI, payload, loadBranchRouteSettingsPromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        branchRouteSettingsSelectedPromiseDeferred = undefined;
                    });
                };

                api.getData = function () {
                    if (branchRouteSettingsAPI != undefined)
                        return branchRouteSettingsAPI.getData();
                    return null;
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function') {
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
    }

    app.directive('whsRoutesyncEricssonbranchroutesettings', whsRoutesyncEricssonbranchroutesettings);

})(app);