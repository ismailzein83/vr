(function (appControllers) {

    "use strict";

    StatusDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'Retail_BE_StatusDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function StatusDefinitionEditorController($scope, UtilsService, Retail_BE_StatusDefinitionAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var statusDefinitionId;
        var statusDefinitionEntity;

        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var styleDefinitionAPI;
        var styleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                statusDefinitionId = parameters.statusDefinitionId;
            }

            isEditMode = (statusDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onStyleDefinitionSelectorReady = function (api) {
                styleDefinitionAPI = api;
                styleDefinitionSelectorReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getStatusDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getStatusDefinition() {
            return Retail_BE_StatusDefinitionAPIService.GetStatusDefinition(statusDefinitionId).then(function (response) {
                statusDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEntityTypeSelector, loadStyleDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var statusDefinitionName = (statusDefinitionEntity != undefined) ? statusDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(statusDefinitionName, 'StatusDefinition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('StatusDefinition');
                }
            }
            function loadStaticData() {
                if (statusDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = statusDefinitionEntity.Name;
                $scope.scopeModel.HasInitialCharge = statusDefinitionEntity.Settings.HasInitialCharge;
                $scope.scopeModel.HasRecurringCharge = statusDefinitionEntity.Settings.HasRecurringCharge;
            }
            function loadEntityTypeSelector() {
                var entityTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                entityTypeSelectorReadyDeferred.promise.then(function () {
                    var entityTypeSelectorPayload = null;
                    if (isEditMode) {
                        entityTypeSelectorPayload = {
                            selectedIds: statusDefinitionEntity.EntityType
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(entityTypeAPI, entityTypeSelectorPayload, entityTypeSelectorLoadDeferred);
                });
                return entityTypeSelectorLoadDeferred.promise;
            }
            function loadStyleDefinitionSelector() {
                var styleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                styleDefinitionSelectorReadyDeferred.promise.then(function () {
                    var styleDefinitionSelectorPayload = null;
                    if (isEditMode) {
                        styleDefinitionSelectorPayload = {
                            selectedIds: statusDefinitionEntity.Settings.StyleDefinitionId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(styleDefinitionAPI, styleDefinitionSelectorPayload, styleDefinitionSelectorLoadDeferred);
                });
                return styleDefinitionSelectorLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_StatusDefinitionAPIService.AddStatusDefinition(buildStatusDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('StatusDefinition', response, 'Name')) {
                    if ($scope.onStatusDefinitionAdded != undefined)
                        $scope.onStatusDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_StatusDefinitionAPIService.UpdateStatusDefinition(buildStatusDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('StatusDefinition', response, 'Name')) {
                    if ($scope.onStatusDefinitionUpdated != undefined) {
                        $scope.onStatusDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildStatusDefinitionObjFromScope() {

            var settings = {
                StyleDefinitionId: styleDefinitionAPI.getSelectedIds(),
                HasInitialCharge: $scope.scopeModel.HasInitialCharge,
                HasRecurringCharge: $scope.scopeModel.HasRecurringCharge
            };

            return {
                StatusDefinitionId: statusDefinitionEntity != undefined ? statusDefinitionEntity.StatusDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings,
                EntityType: entityTypeAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionEditorController', StatusDefinitionEditorController);

})(appControllers);