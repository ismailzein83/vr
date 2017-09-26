﻿(function (appControllers) {

    "use strict";

    StateBackupManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_StateBackupAPIService', 'VRDateTimeService'];

    function StateBackupManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_StateBackupAPIService, VRDateTimeService) {
        var gridAPI;
        var stateBackupTypesDirectiveAPI;
        var stateBackupTypesDirectiveReadyPromiseDeferred;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.stateBackupTypes = [];
            $scope.scopeModel.fromBackupDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };


            $scope.onStateBackupTypesDirectiveReady = function (api) {
                stateBackupTypesDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isGettingData = value;
                };

                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stateBackupTypesDirectiveAPI, undefined, setLoader, stateBackupTypesDirectiveReadyPromiseDeferred);
            };
        }

        function load() {
            $scope.scopeModel.isGettingData = true;
            loadAllControls();
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStateBackupsTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isGettingData = false;
              });
        }


        function loadStateBackupsTypes() {

            var loadStateBackupTypesPromise = WhS_BE_StateBackupAPIService.GetStateBackupTypes().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModel.stateBackupTypes.push(item);
                });
            });

            return loadStateBackupTypesPromise;
        }


        function getFilterObject() {
            var filter = {
                BackupTypeFilterConfigId: $scope.scopeModel.selectedStateBackupType != undefined ? $scope.scopeModel.selectedStateBackupType.ExtensionConfigurationId : undefined,
                From: $scope.scopeModel.fromBackupDate,
                To: $scope.scopeModel.toBackupDate
            };
          
            if ($scope.scopeModel.selectedStateBackupType != undefined)
                filter.BackupTypeFilterObject = stateBackupTypesDirectiveAPI.getData();

            return filter;
        }

    }

    appControllers.controller('WhS_BE_StateBackupManagementController', StateBackupManagementController);
})(appControllers);