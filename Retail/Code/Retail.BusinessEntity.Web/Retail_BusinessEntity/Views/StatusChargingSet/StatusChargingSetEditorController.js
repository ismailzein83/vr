(function (appControllers) {
    "use strict";

    StatusChargingSetEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_StatusChargingSetAPIService '];

    function StatusChargingSetEditorController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, retailBeStatusChargingSetApiService) {
        var isEditMode;
        var statusChargingSetId;
        var statusChargingEntity;
        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        function buildStatusChargingSetObjFromScope() {

            var settings = {
                // StyleDefinitionId: styleDefinitionAPI.getSelectedIds()
            }

            return {
                Name: $scope.scopeModel.name,
                Settings: settings,
                EntityType: entityTypeAPI.getSelectedIds()
            };
        }
        function insert() {
            $scope.scopeModel.isLoading = true;
            return retailBeStatusChargingSetApiService.AddStatusChargingSet(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemAdded('StatusChargingSet', response, 'Name')) {
                    if ($scope.onStatusChargingSetAdded != undefined)
                        $scope.onStatusChargingSetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    // return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            }
        }
        function setTitle() {
            if (isEditMode) {

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('StatusChargingSet');
            }
        }
        function loadEntityTypeSelector() {
            var entityTypeSelectorLoadDeferred = utilsService.createPromiseDeferred();
            entityTypeSelectorReadyDeferred.promise.then(function () {
                var entityTypeSelectorPayload = null;
                if (isEditMode) {
                    entityTypeSelectorPayload = {
                        selectedIds: statusChargingEntity.EntityType
                    };
                }
                vruiUtilsService.callDirectiveLoad(entityTypeAPI, entityTypeSelectorPayload, entityTypeSelectorLoadDeferred);
            });
            return entityTypeSelectorLoadDeferred.promise;
        }
        function loadStaticData() {
            if (statusChargingEntity == undefined)
                return;
            $scope.scopeModel.name = statusChargingEntity.Name;
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEntityTypeSelector]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
            }
            else {
                loadAllControls();
            }
        }
        defineScope();
        load();
    }

    appControllers.controller('Retail_BE_StatusChargingSetEditorController', StatusChargingSetEditorController);

})(appControllers);