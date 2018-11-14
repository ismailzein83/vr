﻿(function (appControllers) {

    "use strict";

    parentManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_BestPractices_ParentService', 'UtilsService', 'VRUIUtilsService'];

    function parentManagementController($scope, VRNotificationService, Demo_BestPractices_ParentService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(getFilter());
            };

            $scope.scopeModel.addParent = function () {
                var onParentAdded = function (parent) {
                    if (gridAPI != undefined) {
                        gridAPI.onParentAdded(parent);
                    }
                };

                Demo_BestPractices_ParentService.addParent(onParentAdded);
            };
        };

        function load() {

        }

        function getFilter() {
            return {
                Name: $scope.scopeModel.name
            };
        };
    };

    appControllers.controller('Demo_BestPractices_ParentManagementController', parentManagementController);
})(appControllers);