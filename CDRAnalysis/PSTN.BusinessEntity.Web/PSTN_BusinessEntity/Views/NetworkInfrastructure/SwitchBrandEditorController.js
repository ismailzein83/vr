SwitchBrandEditorController.$inject = ["$scope", "SwitchBrandAPIService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchBrandEditorController($scope, SwitchBrandAPIService, VRNavigationService, VRNotificationService, VRModalService) {

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

        $scope.saveBrand = function () {
            $scope.isLoading = true;
            if (isEditMode)
                return updateBrand();
            else
                return insertBrand();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() {
        $scope.isLoading = true;
        if (isEditMode) {
            GetBrand().then(function () {
                loadAllControls();

            }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                })
        }
        else {
            loadAllControls();
        }
             

    }

    function loadAllControls() {
        loadFilterBySection();
        brandEntity = undefined;
        $scope.isLoading = false;
    }
    function GetBrand()
    {
        return SwitchBrandAPIService.GetBrandById(brandId).then(function (response) {
            brandEntity = response;
        });
    }
    function loadFilterBySection() {
        if (brandEntity !=undefined)
            $scope.name = brandEntity.Name;
    }

    function updateBrand() {
        var brandObj = buildBrandObjFromScope();

        return SwitchBrandAPIService.UpdateBrand(brandObj)
            .then(function (response) {
                $scope.isLoading = false;
                if (VRNotificationService.notifyOnItemUpdated("Switch Brand", response, "Name")) {

                    if ($scope.onBrandUpdated != undefined)
                        $scope.onBrandUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertBrand() {
        var brandObj = buildBrandObjFromScope();

        return SwitchBrandAPIService.AddBrand(brandObj)
            .then(function (response) {
                $scope.isLoading = false;
                if (VRNotificationService.notifyOnItemAdded("Switch Brand", response, "Name")) {
                    if ($scope.onBrandAdded != undefined)
                        $scope.onBrandAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildBrandObjFromScope() {
        return {
            BrandId: brandId,
            Name: $scope.name
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchBrandEditorController", SwitchBrandEditorController);
