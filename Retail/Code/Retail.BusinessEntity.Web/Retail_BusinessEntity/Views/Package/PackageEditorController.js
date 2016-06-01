(function (appControllers) {

    "use strict";

    packageEditorController.$inject = ['$scope', 'Retail_BE_PackageAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_ContactTypeEnum', 'VRUIUtilsService'];

    function packageEditorController($scope, Retail_BE_PackageAPIService, UtilsService, VRNotificationService, VRNavigationService, WhS_BE_ContactTypeEnum, VRUIUtilsService) {
        var isEditMode;
        var packageId;
        var packageEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                packageId = parameters.PackageId;

            }
            isEditMode = (packageId != undefined);

        }

        function defineScope() {

            $scope.hasSavePackagePermission = function () {
                if ($scope.scopeModal.isEditMode)
                    return Retail_BE_PackageAPIService.HasUpdatePackagePermission();
                else
                    return Retail_BE_PackageAPIService.HasAddPackagePermission();
            }

            $scope.scopeModal = {};

            $scope.SavePackage = function () {
                if (isEditMode) {
                    return updatePackage();
                }
                else {
                    return insertPackage();
                }
            };
      
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.addService = function () {

            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getPackage().then(function () {
                    loadAllControls()
                        .finally(function () {
                            packageEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getPackage() {
            return Retail_BE_PackageAPIService.GetPackage(packageId).then(function (packageObj) {
                packageEntity = packageObj;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(packageEntity ? packageEntity.Name : undefined, 'Package') : UtilsService.buildTitleForAddEditor('Package');
        }
   
        function loadStaticSection() {
            if (packageEntity != undefined) {
                $scope.scopeModal.name = packageEntity.Name;
                $scope.scopeModal.description = packageEntity.Description;
            }

        }

        function insertPackage() {
            $scope.isLoading = true;
            return Retail_BE_PackageAPIService.AddPackage(buildPackageObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Package", response, "Name")) {
                    if ($scope.onPackageAdded != undefined)
                        $scope.onPackageAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updatePackage() {
            $scope.isLoading = true;
            return Retail_BE_PackageAPIService.UpdatePackage(buildPackageObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Package", response, "Name")) {
                    if ($scope.onPackageUpdated != undefined)
                        $scope.onPackageUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildPackageObjFromScope() {
            var obj = {
                PackageId: packageId,
                Name: $scope.scopeModal.name,
                Description: $scope.scopeModal.description,
                Settings:undefined
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_PackageEditorController', packageEditorController);
})(appControllers);
