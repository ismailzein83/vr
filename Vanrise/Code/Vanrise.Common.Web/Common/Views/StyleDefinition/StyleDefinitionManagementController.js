﻿(function (appControllers) {

    "use strict";

    StyleDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRCommon_StyleDefinitionService', 'VRCommon_StyleDefinitionAPIService'];

    function StyleDefinitionManagementController($scope, UtilsService, VRUIUtilsService, VRCommon_StyleDefinitionService, VRCommon_StyleDefinitionAPIService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onStyleDefinitionAdded = function (addedStyleDefinition) {
                    gridAPI.onStyleDefinitionAdded(addedStyleDefinition);
                };
                VRCommon_StyleDefinitionService.addStyleDefinition(onStyleDefinitionAdded);
            };

            $scope.hasAddStyleDefinitionPermission = function () {
                return VRCommon_StyleDefinitionAPIService.HasAddStyleDefinitionPermission()
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VRCommon_StyleDefinitionManagementController', StyleDefinitionManagementController);

})(appControllers);