(function (app) {

    'use strict';

    ZooSectionTypeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_ZooSectionAPIService'];

    function ZooSectionTypeDirective(UtilsService, VRUIUtilsService, Demo_Module_ZooSectionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ZooSectionType($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/Templates/ZooSectionTypeTemplate.html'
        };

        function ZooSectionType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var positionSelectorAPI;
            var positionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

                $scope.scopeModel.onPositionSelectorReady = function (api) {
                    positionSelectorAPI = api;
                    positionSelectorReadyDeferred.resolve();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var zooSectionTypeEntity;

                    if (payload != undefined) {
                        zooSectionTypeEntity = payload.zooSectionTypeEntity;
                    }

                    var getZooSectionTypeConfigsPromise = getZooSectionTypeConfigs();
                    promises.push(getZooSectionTypeConfigsPromise);

                    var loadPositionSelectorPromise = loadPositionSelector();
                    promises.push(loadPositionSelectorPromise);

                    if (zooSectionTypeEntity != undefined) {
                        $scope.scopeModel.hasRiver = zooSectionTypeEntity.HasRiver;
                        $scope.scopeModel.nbOfVisitors = zooSectionTypeEntity.NbOfVisitors;

                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getZooSectionTypeConfigs() {
                        return Demo_Module_ZooSectionAPIService.GetZooSectionTypeConfigs().then(function (response) {
                            if (response != null) {

                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (zooSectionTypeEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, zooSectionTypeEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadPositionSelector() {
                        var positionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        positionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;

                            if (zooSectionTypeEntity != undefined)
                                selectorPayload = {
                                    selectedIds: zooSectionTypeEntity.Positions
                                };

                            VRUIUtilsService.callDirectiveLoad(positionSelectorAPI, selectorPayload, positionLoadPromiseDeferred);
                        });

                        return positionLoadPromiseDeferred.promise;
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                zooSectionTypeEntity: zooSectionTypeEntity
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
                            data.HasRiver = $scope.scopeModel.hasRiver;
                            data.NbOfVisitors = $scope.scopeModel.nbOfVisitors;
                            data.Positions = positionSelectorAPI.getSelectedIds();
                        }
                    }

                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('demoModuleZoosectionType', ZooSectionTypeDirective);
})(app);