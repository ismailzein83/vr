(function (appControllers) {

    "use strict";
    vrDynamicAPIModuleEditorController.$inject = ['$scope', 'VRCommon_VRDynamicAPIModuleAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIModuleEditorController($scope, VRCommon_VRDynamicAPIModuleAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var vrDynamicAPIModuleId;
        var vrDynamicAPIModuleEntity;

        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrDynamicAPIModuleId = parameters.vrDynamicAPIModuleId;
            }
            isEditMode = (vrDynamicAPIModuleId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.saveVRDynamicAPIModule = function () {
                if (isEditMode)
                    return updateVRDynamicAPIModule();
                else
                    return insertVRDynamicAPIModule();

            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVRDynamicAPIModule().then(function () {
                    loadAllControls().finally(function () {
                        vrDynamicAPIModuleEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        }

        function getVRDynamicAPIModule() {
            return VRCommon_VRDynamicAPIModuleAPIService.GetVRDynamicAPIModuleById(vrDynamicAPIModuleId).then(function (response) {
                vrDynamicAPIModuleEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && vrDynamicAPIModuleEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrDynamicAPIModuleEntity.Name, "Dynamic API Module");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Dynamic API Module");
            }

            function loadStaticData() {
                if (vrDynamicAPIModuleEntity != undefined) {
                    $scope.scopeModel.name = vrDynamicAPIModuleEntity.Name;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }

        function buildVRDynamicAPIModuleObjectFromScope() {
            var object = {
                VRDynamicAPIModuleId: (vrDynamicAPIModuleId != undefined) ? vrDynamicAPIModuleId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        }

        function insertVRDynamicAPIModule() {

            $scope.scopeModel.isLoading = true;
            var vrDynamicAPIModuleObject = buildVRDynamicAPIModuleObjectFromScope();
            return VRCommon_VRDynamicAPIModuleAPIService.AddVRDynamicAPIModule(vrDynamicAPIModuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Dynamic API Module", response, "Name")) {
                    if ($scope.onVRDynamicAPIModuleAdded != undefined) {
                        $scope.onVRDynamicAPIModuleAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateVRDynamicAPIModule() {
            $scope.scopeModel.isLoading = true;
            var vrDynamicAPIModuleObject = buildVRDynamicAPIModuleObjectFromScope();
            VRCommon_VRDynamicAPIModuleAPIService.UpdateVRDynamicAPIModule(vrDynamicAPIModuleObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Dynamic API Module", response, "Name")) {
                    if ($scope.onVRDynamicAPIModuleUpdated != undefined) {
                        $scope.onVRDynamicAPIModuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        }
    }
    appControllers.controller('VR_Dynamic_API_ModuleEditorController', vrDynamicAPIModuleEditorController);
})(appControllers);