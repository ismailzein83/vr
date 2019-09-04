'use strict';

app.directive('demoModuleZoosectiontype', ['UtilsService', 'VRUIUtilsService', 'Demo_Module_ZooSectionAPIService',
    function ZooSectionTypeDirective(UtilsService, VRUIUtilsService, Demo_Module_ZooSectionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Type($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/Templates/ZooSectionTypeTemplate.html'
        };

        function Type($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var zooSectionTypeEntity;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var positionSelectorAPI;

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

                    var selectorPayload;
                    if (zooSectionTypeEntity != undefined) {
                        selectorPayload = {
                            selectedIds: zooSectionTypeEntity.Positions
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(positionSelectorAPI, selectorPayload);
                };

                $scope.scopeModel.onSelectionChanged = function (selectedTemplateConfig) {
                    if (selectedTemplateConfig == undefined) return;

                    if (positionSelectorAPI != undefined) {
                        $scope.scopeModel.selectedPositions.length = 0;
                        $scope.scopeModel.nbOfVisitors = undefined;
                        $scope.scopeModel.hasRiver = undefined;
                    }
                };

                function defineAPI() {
                    var api = {};

                    api.load = function (payload) {
                        var promises = [];

                        selectorAPI.clearDataSource();

                        if (payload != undefined) {
                            zooSectionTypeEntity = payload.zooSectionTypeEntity;
                        }

                        var getZooSectionTypeConfigsPromise = getZooSectionTypeConfigs();
                        promises.push(getZooSectionTypeConfigsPromise);

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

                    if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                        ctrl.onReady(api);
                }
            }
        }
    }]);