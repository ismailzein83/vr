(function (appControllers) {

    "use strict";
    SwitchBrandEditorController.$inject = ["$scope", "CDRAnalysis_PSTN_SwitchBrandAPIService", "VRNavigationService", "VRNotificationService", "UtilsService"];

    function SwitchBrandEditorController($scope, CDRAnalysis_PSTN_SwitchBrandAPIService, VRNavigationService, VRNotificationService, UtilsService) {

    var brandId;
    var brandEntity;
    var isEditMode;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            brandId = parameters.BrandId;

        isEditMode = (brandId != undefined);
    }

    function defineScope() {
        $scope.hasSaveSwitchBrandPermission = function () {
            if (isEditMode)
                return CDRAnalysis_PSTN_SwitchBrandAPIService.HasUpdateSwitchBrandPermission();
            else
                return CDRAnalysis_PSTN_SwitchBrandAPIService.HasAddSwitchBrandPermission();
        };

        $scope.saveBrand = function () {
            $scope.isLoading = true;
            if (isEditMode)
                return updateBrand();
            else
                return insertBrand();
        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        $scope.isLoading = true;
        if (isEditMode) {
            GetBrand().then(function () {
                loadAllControls().finally(function () {
                    brandEntity = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            });
        }
        else {
            loadAllControls();
        }
             
    }

    function GetBrand() {
        return CDRAnalysis_PSTN_SwitchBrandAPIService.GetBrandById(brandId).then(function (response) {
            brandEntity = response;
        });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function setTitle() {
        if (isEditMode && brandEntity != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(brandEntity.Name, "Brand");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Brand");
    }

    function loadStaticData() {
        if (brandEntity == undefined)
            return;
        $scope.name = brandEntity.Name;
    }

    function updateBrand() {
        var brandObj = buildBrandObjFromScope();

        return CDRAnalysis_PSTN_SwitchBrandAPIService.UpdateBrand(brandObj)
            .then(function (response) {
                $scope.isLoading = false;
                if (VRNotificationService.notifyOnItemUpdated("Switch Brand", response, "Name")) {

                    if ($scope.onSwitchBrandUpdated != undefined)
                        $scope.onSwitchBrandUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });;
    }

    function insertBrand() {
        var brandObj = buildBrandObjFromScope();

        return CDRAnalysis_PSTN_SwitchBrandAPIService.AddBrand(brandObj)
            .then(function (response) {
                $scope.isLoading = false;
                if (VRNotificationService.notifyOnItemAdded("Switch Brand", response, "Name")) {
                    if ($scope.onSwitchBrandAdded != undefined)
                        $scope.onSwitchBrandAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });;
    }

    function buildBrandObjFromScope() {
        return {
            BrandId: brandId,
            Name: $scope.name
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchBrandEditorController", SwitchBrandEditorController);

})(appControllers);