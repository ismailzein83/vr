(function (appControllers) {

    "use strict";

    StateBackupManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_StateBackupAPIService'];

    function StateBackupManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_StateBackupAPIService) {
        var gridAPI;
        var stateBackupTypesDirectiveAPI;
        var stateBackupTypesDirectiveReadyPromiseDeferred;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.stateBackupTypes = [];
            $scope.scopeModel.fromBackupDate = new Date();

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
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
            var backupTypeFilterObject;
            if (stateBackupTypesDirectiveAPI != undefined) {
                backupTypeFilterObject = stateBackupTypesDirectiveAPI.getData();

            }
            if (backupTypeFilterObject != undefined)
                filter.BackupTypeFilterObject = backupTypeFilterObject;

            return filter;
        }

    }

    appControllers.controller('WhS_BE_StateBackupManagementController', StateBackupManagementController);
})(appControllers);