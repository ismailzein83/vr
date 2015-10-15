SwitchBrandEditorController.$inject = ["$scope", "SwitchBrandAPIService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function SwitchBrandEditorController($scope, SwitchBrandAPIService, VRNavigationService, VRNotificationService, VRModalService) {

    var brandId = undefined;
    var editMode = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            brandId = parameters.BrandId;

        editMode = (brandId != undefined);
    }

    function defineScope() {

        $scope.name = undefined;

        $scope.saveBrand = function () {
            if (editMode)
                return updateBrand();
            else
                return insertBrand();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() {

        if (editMode) {
            $scope.isGettingData = true;

            SwitchBrandAPIService.GetBrandById(brandId)
                .then(function (response) {
                    fillScopeFromBrandObj(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isGettingData = false;
                });
        }
    }

    function fillScopeFromBrandObj(brandObj) {
        $scope.name = brandObj.Name;
    }

    function updateBrand() {
        var brandObj = buildBrandObjFromScope();

        return SwitchBrandAPIService.UpdateBrand(brandObj)
            .then(function (response) {
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
