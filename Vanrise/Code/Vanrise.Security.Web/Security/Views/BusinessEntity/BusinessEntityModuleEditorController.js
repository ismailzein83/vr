(function (appControllers) {

    'use strict';

    BusinessEntityModuleEditorController.$inject = ['$scope', 'VR_Sec_BusinessEntityModuleAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_MenuAPIService'];

    function BusinessEntityModuleEditorController($scope, VR_Sec_BusinessEntityModuleAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_MenuAPIService) {
        $scope.scopeModal = {};
        $scope.scopeModal.isEditMode;
        var businessEntityModuleId;
        var businessEntityModuleEntity;
        var parentId;
        var menuItems = [];
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                parentId = parameters.parentId;
                businessEntityModuleId = parameters.businessEntityModuleId;
                console.log(parentId);
            }

            $scope.scopeModal.isEditMode = (businessEntityModuleId != undefined);
        }

        function defineScope() {

            $scope.scopeModal.save = function () {
                if ($scope.scopeModal.isEditMode)
                    return updateBusinessEntityModule();
                else
                    return insertBusinessEntityModule();
            };
          
            //$scope.hasSaveModulePermission = function () {
            //    if ($scope.scopeModal.isEditMode)
            //        return VR_Sec_ModuleAPIService.HasUpdateModulePermission();
            //    else
            //        return VR_Sec_ModuleAPIService.HasAddModulePermission();
            //};
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if ($scope.scopeModal.isEditMode) {
                getBusinessEntityModule().then(function () {
                    loadAllControls().finally(function () {
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getBusinessEntityModule() {
            return VR_Sec_BusinessEntityModuleAPIService.GetBusinessEntityModuleById(businessEntityModuleId).then(function (response) {
                businessEntityModuleEntity = response;
            });
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
            if ($scope.scopeModal.isEditMode && businessEntityModuleEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(businessEntityModuleEntity.Name, 'Business Entity Module');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Business Entity Module');
        }

        function loadStaticData() {

            if (businessEntityModuleEntity == undefined)
                return;

            $scope.scopeModal.name = businessEntityModuleEntity.Name;
        }

        function buildModuleObjFromScope() {
            var moduleObject = {
                ModuleId: businessEntityModuleId,
                Name: $scope.scopeModal.name,
                ParentId: businessEntityModuleEntity != undefined ? businessEntityModuleEntity.ParentId : parentId,
                BreakInheritance: businessEntityModuleEntity != undefined ? businessEntityModuleEntity.BreakInheritance : undefined,
            };
            return moduleObject;
        }

        function insertBusinessEntityModule() {
            $scope.scopeModal.isLoading = true;

            var moduleObject = buildModuleObjFromScope();

            return VR_Sec_BusinessEntityModuleAPIService.AddBusinessEntityModule(moduleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Business Entity Module', response, 'Name')) {
                    if ($scope.onBusinessEntityModuleAdded != undefined)
                        $scope.onBusinessEntityModuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });

        }

        function updateBusinessEntityModule() {
            $scope.scopeModal.isLoading = true;

            var moduleObject = buildModuleObjFromScope();

            return VR_Sec_BusinessEntityModuleAPIService.UpdateBusinessEntityModule(moduleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Business Entity Module', response, 'Name')) {
                    if ($scope.onBusinessEntityModuleUpdated != undefined)
                        $scope.onBusinessEntityModuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }


    }

    appControllers.controller('VR_Sec_BusinessEntityModuleEditorController', BusinessEntityModuleEditorController);

})(appControllers);
