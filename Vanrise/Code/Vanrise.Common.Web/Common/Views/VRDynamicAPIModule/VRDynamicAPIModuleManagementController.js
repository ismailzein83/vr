﻿(function (appControllers) {
    "use strict";

    vrDynamicAPIModuleManagementController.$inject = ['$scope', 'VRNotificationService', 'VRCommon_DynamicAPIModuleService', 'VRCommon_VRDynamicAPIModuleAPIService','VRCommon_VRDynamicAPIAPIService','UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIModuleManagementController($scope, VRNotificationService, VRCommon_DynamicAPIModuleService, VRCommon_VRDynamicAPIModuleAPIService, VRCommon_VRDynamicAPIAPIService, UtilsService, VRUIUtilsService) {

        var gridApi;
        defineScope();
      load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                api.load(getFilter());
            };

            $scope.scopeModel.search = function () {
                VRCommon_VRDynamicAPIAPIService.BuildAllDynamicAPIControllers();
                return gridApi.load(getFilter());
            };

            $scope.scopeModel.addVRDynamicAPIModule = function () {
                var onVRDynamicAPIModuleAdded = function (dynamicAPI) {
                    if (gridApi != undefined) {
                        gridApi.onVRDynamicAPIModuleAdded(dynamicAPI);
                    }
                };
                VRCommon_DynamicAPIModuleService.addVRDynamicAPIModule(onVRDynamicAPIModuleAdded);
            };

            $scope.scopeModel.hasAddVRDynamicAPIModulePermission = function () {
                return VRCommon_VRDynamicAPIModuleAPIService.HasAddVRDynamicAPIModulePermission();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
                 $scope.scopeModel.isLoading = false;
        }
   

        function getFilter() {
            return {
                query: {
                    Name: $scope.scopeModel.name,
                }
            };
        }
    }

    appControllers.controller('VR_Dynamic_API_ModuleManagementController', vrDynamicAPIModuleManagementController);
})(appControllers);