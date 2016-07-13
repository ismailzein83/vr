﻿(function (appControllers) {

    'use strict';

    ActionDefinitionManagementController.$inject = ['$scope', 'Retail_BE_ActionDefinitionService', 'Retail_BE_ActionDefinitionAPIService', 'UtilsService'];

    function ActionDefinitionManagementController($scope, Retail_BE_ActionDefinitionService, Retail_BE_ActionDefinitionAPIService, UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

            $scope.scopeModel.add = function () {
                var onActionDefinitionAdded = function (addedActionDefinition) {
                    gridAPI.onActionDefinitionAdded(addedActionDefinition);
                };
                Retail_BE_ActionDefinitionService.addActionDefinition(onActionDefinitionAdded);
            };

            $scope.scopeModel.hasAddActionDefinitionPermission = function () {
                return Retail_BE_ActionDefinitionAPIService.HasAddActionDefinitionPermission();
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Retail_BE_ActionDefinitionManagementController', ActionDefinitionManagementController);

})(appControllers);