﻿(function (appControllers) {
    'use strict';

    GenericBEDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionService', 'VR_GenericData_BusinessEntityDefinitionAPIService','UtilsService'];

    function GenericBEDefinitionManagementController($scope, VR_GenericData_BusinessEntityDefinitionService, businessEntityDefinitionAPIService, UtilsService) {

        var gridAPI;
       



        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.bEDefinitionSettingConfigs = [];
            $scope.scopeModel.selectedSettingsTypeConfig = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.search = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
            $scope.hasAddGenericBEPermission = function () {
                return businessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            };
            $scope.addGenericBE = function () {
                var onBusinessEntityDefinitionAdded = function (onGenericBusinessEntityDefinitionObj) {
                    gridAPI.onGenericBusinessEntityDefinitionAdded(onGenericBusinessEntityDefinitionObj);
                };

                VR_GenericData_BusinessEntityDefinitionService.addBusinessEntityDefinition(onBusinessEntityDefinitionAdded);
            };
        }

        function load() {
            $scope.isLoading = true;
            getBEDefinitionSettingConfigs().then(function () {
                loadAllControls();
            });
        }

        function getBEDefinitionSettingConfigs() {

            return businessEntityDefinitionAPIService.GetBEDefinitionSettingConfigs().then(function (response) {
                if (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.bEDefinitionSettingConfigs[i] = response[i];
                    }
                }
            });
        }



        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([]).then(function () {
            }).finally(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getFilterObject() {
            var typeIds;
            if ($scope.scopeModel.selectedSettingsTypeConfig.length > 0) {
                typeIds = [];
                for (var i = 0; i < $scope.scopeModel.selectedSettingsTypeConfig.length; i++) {
                    typeIds.push($scope.scopeModel.selectedSettingsTypeConfig[i].ExtensionConfigurationId);
                }
            }

            return {
                Name: $scope.name,
                TypeIds: typeIds,
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericBEDefinitionManagementController', GenericBEDefinitionManagementController);

})(appControllers);
