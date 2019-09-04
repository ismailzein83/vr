(function (appControllers) {

    'use strict';

    zooManagementController.$inject = ['$scope', 'Demo_Module_ZooService', 'UtilsService', 'VRUIUtilsService', 'ZooSizeEnum'];

    function zooManagementController($scope, Demo_Module_ZooService, UtilsService, VRUIUtilsService, ZooSizeEnum) {

        var gridAPI;

        var sizeSelectorAPI;
        var sizeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(getFilter());
            };

            $scope.scopeModel.onSizeSelectorReady = function (api) {
                sizeSelectorAPI = api;
                sizeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addZoo = function () {
                var onZooAdded = function (zoo) {
                    if (gridAPI != undefined) {
                        zoo.sizeDescription = UtilsService.getEnumDescription(ZooSizeEnum, zoo.Size, 'value');
                        gridAPI.onZooAdded(zoo);
                    }
                };

                Demo_Module_ZooService.addZoo(onZooAdded);
            };
        }

        function load() {
            return loadSizeSelector();
        }

        function getFilter() {
            return {
                Name: $scope.scopeModel.name,
                Sizes: sizeSelectorAPI.getSelectedIds()
            };
        }

        function loadSizeSelector() {
            var sizeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            sizeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(sizeSelectorAPI, undefined, sizeLoadPromiseDeferred);
            });

            return sizeLoadPromiseDeferred.promise;
        }
    }

    appControllers.controller('Demo_Module_ZooManagementController', zooManagementController);
})(appControllers);