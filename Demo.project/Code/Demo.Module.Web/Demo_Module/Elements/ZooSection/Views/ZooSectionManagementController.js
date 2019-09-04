(function (appControllers) {

    'use strict';

    zooSectionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_ZooSectionService'];

    function zooSectionManagementController($scope, UtilsService, VRUIUtilsService, Demo_Module_ZooSectionService) {

        var gridAPI;
        var zooSelectorAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(getFilter());
            };

            $scope.scopeModel.onZooSelectorReady = function (api) {
                zooSelectorAPI = api;
                zooSelectorAPI.load();
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addZooSection = function () {
                var onZooSectionAdded = function (zooSection) {
                    if (gridAPI != undefined) {
                        gridAPI.onZooSectionAdded(zooSection);
                    }
                };

                Demo_Module_ZooSectionService.addZooSection(onZooSectionAdded);
            };
        }

        function load() { }

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                    ZooIds: zooSelectorAPI.getSelectedIds(),
                    MaxNbOfAnimals: $scope.scopeModel.maxNbOfAnimals
                }
            };
        }
    }

    appControllers.controller('Demo_Module_ZooSectionManagementController', zooSectionManagementController);
})(appControllers);