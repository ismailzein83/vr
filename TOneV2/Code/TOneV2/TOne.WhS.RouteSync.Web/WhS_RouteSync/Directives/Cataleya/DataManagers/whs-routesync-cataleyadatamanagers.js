(function (app) {

    'use strict';

    CataleyaDataManagers.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_RouteSync_CataleyaAPIService'];

    function CataleyaDataManagers(UtilsService, VRUIUtilsService, WhS_RouteSync_CataleyaAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaDataManagersCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrlSettings',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/Cataleya/DataManagers/Templates/CataleyaDataManagersTemplate.html'
        };

        function CataleyaDataManagersCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var cataleyaDataManager;

                    if (payload != undefined) {
                        cataleyaDataManager = payload.cataleyaDataManager;
                    }

                    var cataleyaDataManagersTemplateConfigsPromise = getCataleyaDataManagersTemplateConfigs();
                    promises.push(cataleyaDataManagersTemplateConfigsPromise);

                    if (cataleyaDataManager != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getCataleyaDataManagersTemplateConfigs() {
                        return WhS_RouteSync_CataleyaAPIService.GetCataleyaDataManagerConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (cataleyaDataManager != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, cataleyaDataManager.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = { cataleyaDataManager: cataleyaDataManager };
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

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCataleyadatamanagers', CataleyaDataManagers);
})(app);