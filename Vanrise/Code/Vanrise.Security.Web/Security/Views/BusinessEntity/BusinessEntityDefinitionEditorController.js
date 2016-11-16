(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionEditorController.$inject = ['$scope', 'VR_Sec_BusinessEntityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function BusinessEntityDefinitionEditorController($scope, VR_Sec_BusinessEntityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        $scope.scopeModal = {};
        $scope.scopeModal.isEditMode;
        var entityId;
        var moduleId;
        var businessEntityDefinitionEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
            {
                entityId = parameters.entityId;
                moduleId = parameters.moduleId;
            }
               

            $scope.scopeModal.isEditMode = (entityId != undefined);
        }

        function defineScope() {
            $scope.scopeModal.save = function () {
                if ($scope.scopeModal.isEditMode)
                    return updateBusinessEntityDefinition();
                else
                    return insertBusinessEntityDefinition();
            };

            $scope.scopeModal.permissionOptions = [];

            $scope.scopeModal.addPermissionOption = function () {
                $scope.scopeModal.permissionOptions.push($scope.scopeModal.permissionOption);
                $scope.scopeModal.permissionOption = undefined;
            };

            $scope.scopeModal.validate = function () {
                return validate();
            };

            $scope.scopeModal.validatePremissionOptions = function () {
                if ($scope.scopeModal.permissionOptions == undefined || $scope.scopeModal.permissionOptions.length == 0)
                    return "At least one option should be added.";
                return null;
            };

            $scope.hasSaveBusinessEntityPermission = function () {
                if ($scope.scopeModal.isEditMode)
                    return VR_Sec_BusinessEntityAPIService.HasUpdateBusinessEntityPermission();
                else
                    return VR_Sec_BusinessEntityAPIService.HasAddBusinessEntityPermission();
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if ($scope.scopeModal.isEditMode) {
                getBusinessEntity().then(function () {
                    loadAllControls().finally(function () {
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            } else
            {
                loadAllControls();
            }
            function getBusinessEntity() {
                return VR_Sec_BusinessEntityAPIService.GetBusinessEntity(entityId).then(function (response) {
                    businessEntityDefinitionEntity = response;
                });
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        function setTitle() {
            if ($scope.scopeModal.isEditMode && businessEntityDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(businessEntityDefinitionEntity.Name, 'Business Entity');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Business Entity');
        }

        function loadStaticData() {

            if (businessEntityDefinitionEntity == undefined)
                return;

            $scope.scopeModal.name = businessEntityDefinitionEntity.Name;
            $scope.scopeModal.titleValue = businessEntityDefinitionEntity.Title;
            if (businessEntityDefinitionEntity.PermissionOptions != undefined)
                $scope.scopeModal.permissionOptions = businessEntityDefinitionEntity.PermissionOptions;
        }

        function validate() {
            if ($scope.scopeModal.permissionOption == undefined)
                return true;
            if (UtilsService.contains($scope.scopeModal.permissionOptions, $scope.scopeModal.permissionOption))
                return true;
            return false;
        }

        function buildBusinessEntityDefinitionObjFromScope() {
            var businessEntityDefinitionObject = {
                EntityId: entityId,
                Name: $scope.scopeModal.name,
                Title: $scope.scopeModal.titleValue,
                ModuleId: businessEntityDefinitionEntity != undefined ? businessEntityDefinitionEntity.ModuleId :  moduleId,
                BreakInheritance: businessEntityDefinitionEntity != undefined?businessEntityDefinitionEntity.BreakInheritance:0,
                PermissionOptions: $scope.scopeModal.permissionOptions
            };
            return businessEntityDefinitionObject;
        }

        function insertBusinessEntityDefinition() {
            $scope.scopeModal.isLoading = true;

            var businessEntityDefinitionObject = buildBusinessEntityDefinitionObjFromScope();

            return VR_Sec_BusinessEntityAPIService.AddBusinessEntity(businessEntityDefinitionObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Business Entity Definition', response, 'Name')) {
                    if ($scope.onBusinessEntityDefinitionAdded != undefined)
                        $scope.onBusinessEntityDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });

        }

        function updateBusinessEntityDefinition() {
            $scope.scopeModal.isLoading = true;

            var businessEntityDefinitionObject = buildBusinessEntityDefinitionObjFromScope();

            return VR_Sec_BusinessEntityAPIService.UpdateBusinessEntity(businessEntityDefinitionObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Business Entity Definition', response, 'Name')) {
                    if ($scope.onBusinessEntityDefinitionUpdated != undefined)
                        $scope.onBusinessEntityDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_BusinessEntityDefinitionEditorController', BusinessEntityDefinitionEditorController);

})(appControllers);
