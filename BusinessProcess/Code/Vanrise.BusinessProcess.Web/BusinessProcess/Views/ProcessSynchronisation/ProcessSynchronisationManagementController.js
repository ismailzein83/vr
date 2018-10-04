﻿(function (appControllers) {

    "use strict";

    ProcessSynchronisationManagementController.$inject = ['$scope', 'BusinessProcess_ProcessSynchronisationService', 'BusinessProcess_ProcessSynchronisationAPIService'];

    function ProcessSynchronisationManagementController($scope, BusinessProcess_ProcessSynchronisationService, BusinessProcess_ProcessSynchronisationAPIService) {


        var gridAPI;

        defineScope();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.addClicked = function () {
                var onProcessSynchronisationAdded = function (addedProcessSynchronisation) {
                    gridAPI.onProcessSynchronisationAdded(addedProcessSynchronisation);
                };

                BusinessProcess_ProcessSynchronisationService.addProcessSynchronisation(onProcessSynchronisationAdded);
            };

            $scope.scopeModel.hasAddProcessSynchronisationPermission = function () {
                return BusinessProcess_ProcessSynchronisationAPIService.HasAddProcessSynchronisationPermission();
            };
        }

        function getFilterObject() {
            return { Name: $scope.scopeModel.name };
        }
    }

    appControllers.controller('BusinessProcess_ProcessSynchronisationManagementController', ProcessSynchronisationManagementController);
})(appControllers);