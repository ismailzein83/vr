'use strict';

app.directive('demoModuleZoosectiontypeAfrican', ['UtilsService', 'Demo_Module_ZooSectionAPIService', 'VRUIUtilsService',
    function (UtilsService, Demo_Module_ZooSectionAPIService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AfricanType($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/AfricanSectionTypeTemplate.html'
        };

        function AfricanType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridAPIdeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.animalConfigs = [];

                $scope.scopeModel.isValid = function () {
                    if (checkItemsDuplication())
                        return 'Same name exist';

                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return 'You should add at least one animal';

                    return null;
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIdeferred.resolve();
                };

                $scope.scopeModel.addAnimal = function () {
                    extendItemToGrid();
                };

                $scope.scopeModel.removeAnimal = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, item.rowIndex, 'rowIndex');
                    if (index > -1)
                        $scope.scopeModel.datasource.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var zooSectionTypeEntity;
                    var firstLayerPromises = [];

                    if (payload != undefined) {
                        zooSectionTypeEntity = payload.zooSectionTypeEntity;
                    }

                    var loadZooSectionTypeAnimalConfigsPromise = loadZooSectionTypeAnimalConfigs();
                    firstLayerPromises.push(loadZooSectionTypeAnimalConfigsPromise);

                    var rootPromiseNode = {
                        promises: firstLayerPromises,
                        getChildNode: function () {
                            var loadZooSectionTypeAnimalsGridPromise = loadZooSectionTypeAnimalsGrid(zooSectionTypeEntity);

                            return {
                                promises: [loadZooSectionTypeAnimalsGridPromise]
                            };
                        }
                    };

                    function loadZooSectionTypeAnimalConfigs() {
                        return Demo_Module_ZooSectionAPIService.GetZooSectionTypeAnimalConfigs().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.scopeModel.animalConfigs.push(item);
                            });
                        });
                    }

                    function loadZooSectionTypeAnimalsGrid(payload) {
                        var promises = [];

                        if (payload != undefined && payload.Animals != undefined) {
                            for (var i = 0; i < payload.Animals.length; i++) {
                                var extendItemToGridPromise = extendItemToGrid(payload.Animals[i]);
                                promises.push(extendItemToGridPromise);
                            }
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var animals;
                    if ($scope.scopeModel.datasource.length > 0) {
                        animals = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i];
                            if (currentItem != undefined) {
                                if (currentItem.animalSelectorAPI != undefined) {
                                    var animal = currentItem.animalSelectorAPI.getData();
                                    animal.Name = currentItem.Name;
                                    animal.Weight = currentItem.Weight;
                                    animals.push(animal);
                                }
                                else {
                                    animals.push(currentItem.Animal);
                                }
                            }
                        }
                    }

                    return {
                        $type: "Demo.Module.MainExtension.ZooSection.African, Demo.Module.MainExtension",
                        Animals: animals
                    };
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }

            function extendItemToGrid(animal) {
                var extendItemToGridDeferred = UtilsService.createPromiseDeferred();

                var gridItem = {};

                if (animal != undefined) {
                    gridItem.Name = animal.Name;
                    gridItem.Weight = animal.Weight;
                    gridItem.Animal = animal;
                }
                else {
                    gridAPI.expandRow(gridItem);
                }

                gridItem.animalSelectorReadyDeferred = UtilsService.createPromiseDeferred();

                gridItem.onAnimalSelectorReady = function (api) {
                    gridItem.animalSelectorAPI = api;

                    var zooSectionTypeAnimalPayload;

                    if (animal != undefined) {
                        zooSectionTypeAnimalPayload = { zooSectionTypeAnimalEntity: animal };
                    }

                    var setLoader = function (value) {
                        gridItem.isAnimalSelectorLoading = value;
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.animalSelectorAPI, zooSectionTypeAnimalPayload, setLoader);
                    gridItem.animalSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.datasource.push(gridItem);
                extendItemToGridDeferred.resolve();

                return extendItemToGridDeferred.promise;
            }

            function checkItemsDuplication() {
                if ($scope.scopeModel.datasource != undefined) {
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var item = $scope.scopeModel.datasource[i];
                        for (var j = i + 1; j < $scope.scopeModel.datasource.length; j++) {
                            var currentItem = $scope.scopeModel.datasource[j];
                            if (item.Name == currentItem.Name) {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }]);