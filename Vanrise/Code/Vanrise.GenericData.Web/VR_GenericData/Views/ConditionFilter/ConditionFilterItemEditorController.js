(function (appControllers) {
    "use strict";
    conditionFilterItemEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function conditionFilterItemEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var entity;
        var permanentFilterSettingsAPI;
        var permanentFilterSettingsReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeId;
        $scope.scopeModel = {};

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope); 
            if (parameters != undefined && parameters != null) {
                dataRecordTypeId = parameters.dataRecordTypeId;
                entity = parameters.entity;
            }
            isEditMode = (entity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.saveConditionFilterItem = function () {

                if (isEditMode)
                    return updateDataRow();
                else
                    return insertDataRow();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onPermanentFilterSettingsDirectiveReady = function (api) {
                permanentFilterSettingsAPI = api;
                permanentFilterSettingsReadyDeferred.resolve();
            };
        }

        function loadPermanentFilterSettingsDirective() {
            var loadPermanentFilterSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
            permanentFilterSettingsReadyDeferred.promise.then(function () {
                var payload;

                    payload = {
                        dataRecordTypeId: dataRecordTypeId,
                        settings: entity != undefined  ? entity.Settings : undefined
                    };
                VRUIUtilsService.callDirectiveLoad(permanentFilterSettingsAPI, payload, loadPermanentFilterSettingsPromiseDeferred);
            });
            return loadPermanentFilterSettingsPromiseDeferred.promise;
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls().finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor("Edit Condition Filter", entity.Name);
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Add Condition Filter");
            }
            function loadStaticData() {
                $scope.scopeModel.Name = entity != undefined ? entity.Name : undefined;
            }
            return UtilsService.waitMultipleAsyncOperations([loadPermanentFilterSettingsDirective, loadStaticData,setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildDataRowFromScope() {

            var obj = {
                ConditionFilterItemId: UtilsService.guid(),
                Name: $scope.scopeModel.Name,
                Settings: permanentFilterSettingsAPI.getData()
            };
            return { Entity: obj };
        }


        function insertDataRow() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onConditionFilterItemAdded != undefined) {
                $scope.onConditionFilterItemAdded(buildDataRowFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;
        }

        function updateDataRow() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onConditionFilterItemUpdated != undefined) {
                $scope.onConditionFilterItemUpdated(buildDataRowFromScope());
            }
            $scope.modalContext.closeModal();
            $scope.scopeModel.isLoading = true;

        }

    }
    appControllers.controller('VR_GenericData_ConditionFilterItemEditorController', conditionFilterItemEditorController);
})(appControllers);