'use strict';

app.directive('demoModuleZoosectionTypeAfrican', ['UtilsService', 'Demo_Module_ZooSectionAPIService', 'VRUIUtilsService',
    function (UtilsService, Demo_Module_ZooSectionAPIService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                validationcontext: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ZooSectionTypeAfrican($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/AfricanSectionTypeTemplate.html'
        };

        function ZooSectionTypeAfrican($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridAPIdeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.datasource = [];

                ctrl.animalConfigs = [];

                ctrl.isValid = function () {
                    if (checkDuplicateInArray(ctrl.datasource))
                        return 'Same name exist';

                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return 'You Should Add at least one animal';

                    return null;
                };

                ctrl.disableAddButton = function () {
                    if (ctrl.isValid() != null)
                        return false;

                    return ctrl.validationcontext.validate() != null;
                };

                function checkDuplicateInArray(array) {
                    if (array != undefined) {
                        for (var i = 0; i < array.length; i++) {
                            var item = array[i];
                            for (var j = i + 1; j < array.length; j++) {
                                var currentItem = array[j];
                                if (item.Name == currentItem.Name) {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }

                ctrl.removeFilter = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.rowIndex, 'rowIndex');
                    if (index > -1)
                        ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridAPIdeferred.resolve();
                };

                ctrl.addAnimal = function () {

                    var gridItem = {
                        Name: undefined,
                        Weight: undefined,
                        Animal: undefined,
                        animalSelectorAPI: undefined,
                        animalSelectorReadyDeferred: UtilsService.createPromiseDeferred()
                    };

                    gridItem.onAnimalSelectorReady = function (api) {
                        gridItem.animalSelectorAPI = api;
                        gridItem.animalSelectorReadyDeferred.resolve();
                    };

                    gridItem.animalSelectorReadyDeferred.promise.then(function () {
                        var zooSectionTypeAnimalPayload = {};
                        zooSectionTypeAnimalPayload.additionalParameters = { showDependantAnimalsGrid: true };
                        var setLoader = function (value) { gridItem.isAnimalSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.animalSelectorAPI, zooSectionTypeAnimalPayload, setLoader);
                    });

                    gridAPI.expandRow(gridItem);

                    ctrl.datasource.push(gridItem);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                var zooSectionTypeEntity;

                api.getData = function () {
                    var animals;
                    if (ctrl.datasource != undefined) {
                        animals = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
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

                api.load = function (payload) {
                    var firstLayerPromises = [];

                    if (payload != undefined) {
                        zooSectionTypeEntity = payload.zooSectionTypeEntity;
                    }

                    firstLayerPromises.push(loadZooSectionTypeAnimalConfigs());

                    var rootPromiseNode = {
                        promises: firstLayerPromises,
                        getChildNode: function () {
                            return {
                                promises: loadZooSectionTypeAnimalsGrid(zooSectionTypeEntity)
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadZooSectionTypeAnimalConfigs() {
                return Demo_Module_ZooSectionAPIService.GetZooSectionTypeAnimalConfigs().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.animalConfigs.push(item);
                    });
                });
            }

            function loadZooSectionTypeAnimalsGrid(payload) {
                var promises = [];
                if (payload != undefined && payload.Animals != undefined) {
                    for (var i = 0; i < payload.Animals.length; i++) {
                        var animal = payload.Animals[i];
                        if (animal != undefined) {
                            var gridItem = {
                                Name: animal.Name,
                                Weight: animal.Weight,
                                Animal: animal,
                                animalSelectorReadyDeferred: UtilsService.createPromiseDeferred()
                            };
                            addColumnOnEdit(gridItem);
                            ctrl.datasource.push(gridItem);
                        }
                    }
                }

                return promises;
            }

            function addColumnOnEdit(gridItem) {

                gridItem.onAnimalSelectorReady = function (api) {
                    gridItem.animalSelectorAPI = api;
                    gridItem.animalSelectorReadyDeferred.resolve();
                    var zooSectionTypeAnimalPayload = { zooSectionTypeAnimalEntity: gridItem.Animal };

                    var setLoader = function (value) {
                        gridItem.isAnimalSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.animalSelectorAPI, zooSectionTypeAnimalPayload, setLoader, undefined);
                };
            }
        }
    }]);