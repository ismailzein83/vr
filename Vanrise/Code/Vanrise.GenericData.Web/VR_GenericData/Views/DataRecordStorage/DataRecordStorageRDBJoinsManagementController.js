//(function (appControllers) {

//    'use strict';

//    DataRecordStorageRDBJoinsManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

//    function DataRecordStorageRDBJoinsManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

//        var joinEntity;
//        var context;
//        var isEditMode;

//        var joinSettingsSelectiveAPI;
//        var joinSettingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined) {
//                joinEntity = parameters.joinEntity;
//                context = parameters.context;
//            }

//            isEditMode = (joinEntity != undefined);
//        }

//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.onJoinSettingsSelectiveReady = function (api) {
//                joinSettingsSelectiveAPI = api;
//                joinSettingsSelectiveReadyDeferred.resolve();
//            };

//            $scope.scopeModel.saveRDBJoin = function () {
//                if (isEditMode)
//                    return updateRDBJoin();
//                else
//                    return insertRDBJoin();
//            };

//            $scope.scopeModel.closeRDBJoin = function () {
//                $scope.modalContext.closeModal();
//            };
//        }

//        function load() {
//            $scope.isLoading = true;

//            UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRDBJoinsSelective]).catch(function (error) {
//                VRNotificationService.notifyException(error, $scope);
//            }).finally(function () {
//                $scope.isLoading = false;
//            });
//        }

//        function setTitle() {
//            $scope.title = (isEditMode) ?
//                UtilsService.buildTitleForUpdateEditor((joinEntity != undefined) ? joinEntity.RDBRecordStorageJoinName : null, 'RDB Join') :
//                UtilsService.buildTitleForAddEditor('RDB Join');
//        }

//        function loadStaticData() {
//            if (joinEntity != undefined) {
//                $scope.scopeModel.rdbRecordStorageJoinName = joinEntity.RDBRecordStorageJoinName;
//            }
//        }

//        function loadRDBJoinsSelective() {
//            var joinSettingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

//            joinSettingsSelectiveReadyDeferred.promise.then(function () {
//                var joinSettingsPayload = {
//                    joinSettings: joinEntity != undefined ? joinEntity.Settings : undefined,
//                    context: context 
//                };
//                VRUIUtilsService.callDirectiveLoad(joinSettingsSelectiveAPI, joinSettingsPayload, joinSettingsSelectiveLoadDeferred);
//            });
//            return joinSettingsSelectiveLoadDeferred.promise;
//        }

//        function insertRDBJoin (){
//            if ($scope.onJoinAdded != undefined && typeof ($scope.onJoinAdded) == 'function') {
//                $scope.onJoinAdded(buildRDBJoinObjFromScope());
//            }
//            $scope.modalContext.closeModal();
//        }

//        function updateRDBJoin() {
//            if ($scope.onJoinUpdated != undefined && typeof ($scope.onJoinUpdated) == 'function') {
//                $scope.onJoinUpdated(buildRDBJoinObjFromScope());
//            }
//            $scope.modalContext.closeModal();
//        }

//        function buildRDBJoinObjFromScope() {
//            var rdbJoinObj = {
//                RDBRecordStorageJoinName: $scope.scopeModel.rdbRecordStorageJoinName,
//                Settings: joinSettingsSelectiveAPI.getData()
//            };
//            return rdbJoinObj;
//        }
//    }

//    appControllers.controller('VR_GenericData_DataRecordStorageRDBJoinsManagementController', DataRecordStorageRDBJoinsManagementController);

//})(appControllers);