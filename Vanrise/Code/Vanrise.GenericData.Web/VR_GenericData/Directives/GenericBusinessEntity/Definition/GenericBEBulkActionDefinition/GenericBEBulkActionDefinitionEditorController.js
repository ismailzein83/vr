(function (appControllers) {

    'use strict';

    GenericBEBulkActionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBEBulkActionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var context;
        var genericBEDefinitionId;
        var genericBEBulkActionEntity;

        var genericBEBulkActionSettingsSelectiveAPI;
        var genericBEBulkActionSettingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                genericBEDefinitionId = parameters.genericBEDefinitionId;
                genericBEBulkActionEntity = parameters.genericBEBulkActionEntity;
                context = parameters.context;
            }
            isEditMode = (genericBEBulkActionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBEBulkActionSettingsSelectiveReady = function (api) {
                genericBEBulkActionSettingsSelectiveAPI = api;
                genericBEBulkActionSettingsSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((genericBEBulkActionEntity != undefined) ? genericBEBulkActionEntity.Title : null, 'Bulk Action') :
                    UtilsService.buildTitleForAddEditor('Bulk Action');
            }
            function loadStaticData() {
                if (genericBEBulkActionEntity == undefined)
                    return;
                $scope.scopeModel.title = genericBEBulkActionEntity.Title;
            }
            function loadGenericBEBulkActionSettingsSelective() {
                var genericBEBulkActionSettingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                genericBEBulkActionSettingsSelectiveReadyDeferred.promise.then(function () {

                    var settingsDirectivePayload = {
                        settings: genericBEBulkActionEntity != undefined ? genericBEBulkActionEntity.Settings : undefined,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(genericBEBulkActionSettingsSelectiveAPI, settingsDirectivePayload, genericBEBulkActionSettingsSelectiveLoadDeferred);
                });

                return genericBEBulkActionSettingsSelectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGenericBEBulkActionSettingsSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var genericBEBulkActionObject = buildGenericBEBulkActionObjectFromScope();

            if ($scope.onGenericBEBulkActionAdded != undefined && typeof ($scope.onGenericBEBulkActionAdded) == 'function') {
                $scope.onGenericBEBulkActionAdded(genericBEBulkActionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var genericBEBulkActionObject = buildGenericBEBulkActionObjectFromScope();

            if ($scope.onGenericBEBulkActionUpdated != undefined && typeof ($scope.onGenericBEBulkActionUpdated) == 'function') {
                $scope.onGenericBEBulkActionUpdated(genericBEBulkActionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildGenericBEBulkActionObjectFromScope() {
            return {
                GenericBEBulkActionId: genericBEBulkActionEntity != undefined ? genericBEBulkActionEntity.GenericBEBulkActionId : UtilsService.guid(),
                Title: $scope.scopeModel.title,
                Settings: genericBEBulkActionSettingsSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericBEBulkActionDefinitionEditorController', GenericBEBulkActionEditorController);

})(appControllers);