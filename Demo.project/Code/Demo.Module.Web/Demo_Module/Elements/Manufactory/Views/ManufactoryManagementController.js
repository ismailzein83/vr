(function (appControllers) {
    'use strict';

    manufactoryManagementController.$inject = ['$scope', 'UtilsService', 'Demo_Module_ManufactoryService', 'VRUIUtilsService'];

    function manufactoryManagementController($scope, UtilsService, Demo_Module_ManufactoryService, VRUIUtilsService) {

        var manufactoryGridAPI;
        var manufactoryGridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.manufactories = [];

            $scope.scopeModel.onSearchClicked = function () {
                manufactoryGridAPI.load(getFilter());
            };

            $scope.scopeModel.onAddClicked = function () {
                var onManufactoryAdded = function (manufactory) {
                    manufactoryGridAPI.onManufactoryAdded(manufactory);
                };
                Demo_Module_ManufactoryService.addManufactory(onManufactoryAdded);
            };

            $scope.scopeModel.onManufactoryGridReady = function (api) {
                manufactoryGridAPI = api;
                manufactoryGridReadyDeferred.resolve();
            };
        }

        function load() {
            loadManufactoryGrid();

            function loadManufactoryGrid() {
                var manufactoryGridLoadDeferred = UtilsService.createPromiseDeferred();

                manufactoryGridReadyDeferred.promise.then(function () {
                    var payload = getFilter();
                    VRUIUtilsService.callDirectiveLoad(manufactoryGridAPI, payload, manufactoryGridLoadDeferred);
                });

                return manufactoryGridLoadDeferred.promise;
            }
        }

        function getFilter() {
            return {
                Name: $scope.scopeModel.name,
                CountryOfOrigin: $scope.scopeModel.countryOfOrigin
            };
        }
    }

    appControllers.controller('Demo_Module_ManufactoryManagementController', manufactoryManagementController);
})(appControllers);