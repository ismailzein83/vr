(function (appControllers) {

    "use strict";

    StateBackupManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_StateBackupAPIService'];

    function StateBackupManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_StateBackupAPIService) {
        var gridAPI;
        var stateBackupTypesDirectiveAPI;
        var stateBackupTypesDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.stateBackupTypes = [];
            $scope.searchClicked = function () {

                setFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            }


            $scope.onStateBackupTypesDirectiveReady = function (api) {
                stateBackupTypesDirectiveAPI = api;
                //var setLoader = function (value) {
                //    $scope.isLoadingAction = value;
                //};

                //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stateBackupTypesDirectiveAPI, undefined, setLoader, stateBackupTypesDirectiveReadyPromiseDeferred);
            }
        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStateBackupsTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }


        function loadStateBackupsTypes() {
            var promises = [];

            var loadStateBackupTypesPromise = WhS_BE_StateBackupAPIService.GetStateBackupTypes().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.scopeModel.stateBackupTypes.push(item);
                });
            });

            promises.push(loadStateBackupTypesPromise);

            var loadStateBackupDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadStateBackupDirectivePromiseDeferred.promise);


            var setLoader = function (value) {
                $scope.isLoadingAction = value;
            };

            stateBackupTypesDirectiveReadyPromiseDeferred.promise.then(function () {
                stateBackupTypesDirectiveReadyPromiseDeferred = undefined;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stateBackupTypesDirectiveAPI, undefined, setLoader, loadStateBackupDirectivePromiseDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
        }


        function setFilterObject() {
            filter = {
            };

        }

    }

    appControllers.controller('WhS_BE_StateBackupManagementController', StateBackupManagementController);
})(appControllers);