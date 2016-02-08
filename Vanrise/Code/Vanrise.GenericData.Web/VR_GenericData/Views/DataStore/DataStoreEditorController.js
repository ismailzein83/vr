(function (appControllers) {

    "use strict";
    DataStoreEditorController.$inject = ["$scope", "VRNavigationService", "VRNotificationService", "UtilsService"];

    function DataStoreEditorController($scope, VRNavigationService, VRNotificationService, UtilsService) {

        var dataStoreId;
        var dataStoreEntity;
        var isEditMode;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                dataStoreId = parameters.DataStoreId;

            isEditMode = (dataStoreId != undefined);
        }

        function defineScope() {
            //$scope.saveBrand = function () {
            //    $scope.isLoading = true;
            //    if (isEditMode)
            //        return updateBrand();
            //    else
            //        return insertBrand();
            //}
            $scope.close = function () {
                $scope.modalContext.closeModal()
            }
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                GetDataStore.then(function () {
                    loadAllControls().finally(function () {
                        dataStoreEntity = undefined;
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

        function GetDataStore() {
            ////return CDRAnalysis_PSTN_SwitchBrandAPIService.GetBrandById(brandId).then(function (response) {
            //            brandEntity = {};
            //         //   });
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
            if (isEditMode && dataStoreEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataStoreEntity.Name, "Data Store");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Data Store");
        }

        function loadStaticData() {
            if (dataStoreEntity == undefined)
                return;
            $scope.name = dataStoreEntity.Name;
        }

        //function updateBrand() {
        //    var brandObj = buildBrandObjFromScope();

        //    return CDRAnalysis_PSTN_SwitchBrandAPIService.UpdateBrand(brandObj)
        //        .then(function (response) {
        //            $scope.isLoading = false;
        //            if (VRNotificationService.notifyOnItemUpdated("Switch Brand", response, "Name")) {

        //                if ($scope.onSwitchBrandUpdated != undefined)
        //                    $scope.onSwitchBrandUpdated(response.UpdatedObject);

        //                $scope.modalContext.closeModal();
        //            }
        //        })
        //        .catch(function (error) {
        //            VRNotificationService.notifyException(error, $scope);
        //        }).finally(function () {
        //            $scope.isLoading = false;
        //        });;
        //}

        //function insertBrand() {
        //    var brandObj = buildBrandObjFromScope();

        //    return CDRAnalysis_PSTN_SwitchBrandAPIService.AddBrand(brandObj)
        //        .then(function (response) {
        //            $scope.isLoading = false;
        //            if (VRNotificationService.notifyOnItemAdded("Switch Brand", response, "Name")) {
        //                if ($scope.onSwitchBrandAdded != undefined)
        //                    $scope.onSwitchBrandAdded(response.InsertedObject);

        //                $scope.modalContext.closeModal();
        //            }
        //        })
        //        .catch(function (error) {
        //            VRNotificationService.notifyException(error, $scope);
        //        }).finally(function () {
        //            $scope.isLoading = false;
        //        });;
        //}

        //function buildBrandObjFromScope() {
        //    return {
        //        BrandId: brandId,
        //        Name: $scope.name
        //    };
        //}
    }

    appControllers.controller("VR_GenericData_DataStoreEditorController", DataStoreEditorController);

})(appControllers);