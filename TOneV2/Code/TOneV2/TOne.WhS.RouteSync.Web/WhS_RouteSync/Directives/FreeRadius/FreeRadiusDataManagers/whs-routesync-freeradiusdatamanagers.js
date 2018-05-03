(function (app) {

    'use strict';

    FreeRadiusDataManagersDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_FreeRadiusAPIService'];

    function FreeRadiusDataManagersDirective(UtilsService, VRUIUtilsService, WhS_RouteSync_FreeRadiusAPIService) {
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
                var ctor = new FreeRadiusPostgresDataManagersCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSettings",
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/FreeRadius/FreeRadiusDataManagers/Templates/FreeRadiusDataManagersTemplate.html'
        };

        function FreeRadiusPostgresDataManagersCtor($scope, ctrl, $attrs) {
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

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var freeRadiusDataManager;

                    if (payload != undefined) {
                        freeRadiusDataManager = payload.freeRadiusDataManager;
                    }

                    var freeRadiusPostgresDataManagersTemplateConfigsPromise = getFreeRadiusPostgresDataManagersTemplateConfigs();
                    promises.push(freeRadiusPostgresDataManagersTemplateConfigsPromise);

                    if (freeRadiusDataManager != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getFreeRadiusPostgresDataManagersTemplateConfigs() {
                        return WhS_RouteSync_FreeRadiusAPIService.GetFreeRadiusDataManagerConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (freeRadiusDataManager != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, freeRadiusDataManager.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { freeRadiusDataManager: freeRadiusDataManager };
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

    app.directive('whsRoutesyncFreeradiusdatamanagers', FreeRadiusDataManagersDirective);

})(app);