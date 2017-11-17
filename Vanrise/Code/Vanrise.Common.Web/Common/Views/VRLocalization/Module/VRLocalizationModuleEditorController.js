(function (appControllers) {

    "use strict";

    VRLocalizationModuleController.$inject = ['$scope', 'VRCommon_VRLocalizationModuleAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function VRLocalizationModuleController($scope, VRCommon_VRLocalizationModuleAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrLocalizationModuleId;
        var vrLocalizationModuleEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrLocalizationModuleId = parameters.vrLocalizationModuleId;
            }
            isEditMode = (vrLocalizationModuleId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.saveVRLocalizationModule = function () {
                if (isEditMode)
                    return updateVRLocalizationModule();
                else
                    return insertVRLocalizationModule();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRLocalizationModule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            vrLocalizationModuleEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRLocalizationModule() {
            return VRCommon_VRLocalizationModuleAPIService.GetVRLocalizationModule(vrLocalizationModuleId).then(function (entity) {
                vrLocalizationModuleEntity = entity;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrLocalizationModuleEntity.Name, "Localization Module");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Localization Module");
            }

            function loadStaticData() {
                if (vrLocalizationModuleEntity != undefined) {
                    $scope.scopeModel.name = vrLocalizationModuleEntity.Name;
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

        function buildVRLocalizationModuleObjFromScope() {
            return {
                VRLocalizationModuleId: vrLocalizationModuleId,
                Name: $scope.scopeModel.name
            };
        }

        function insertVRLocalizationModule() {
            $scope.scopeModel.isLoading = true;

            var VRLocalizationModuleObject = buildVRLocalizationModuleObjFromScope();
            return VRCommon_VRLocalizationModuleAPIService.AddVRLocalizationModule(VRLocalizationModuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Localization Module", response, "Name")) {
                    if ($scope.onVRLocalizationModuleAdded != undefined) {
                        $scope.onVRLocalizationModuleAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateVRLocalizationModule() {
            $scope.scopeModel.isLoading = true;

            var VRLocalizationModuleObject = buildVRLocalizationModuleObjFromScope();
            VRCommon_VRLocalizationModuleAPIService.UpdateVRLocalizationModule(VRLocalizationModuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Localization Module", response, "Name")) {
                    if ($scope.onVRLocalizationModuleUpdated != undefined)
                        $scope.onVRLocalizationModuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    appControllers.controller('VRCommon_VRLocalizationModuleController', VRLocalizationModuleController);
})(appControllers);
