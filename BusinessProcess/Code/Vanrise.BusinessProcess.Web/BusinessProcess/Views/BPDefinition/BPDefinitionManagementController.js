﻿(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionManagementController.$inject = ['$scope'];

    function BusinessProcess_BPDefinitionManagementController($scope) {
        var gridAPI;
        var filter = {};
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
        }

        function getFilterObject() {
            filter = {
                Title: $scope.title,
                ShowOnlyVisibleInManagementScreen:true
            };
        }
    }

    appControllers.controller('BusinessProcess_BP_DefinitionManagementController', BusinessProcess_BPDefinitionManagementController);
})(appControllers);