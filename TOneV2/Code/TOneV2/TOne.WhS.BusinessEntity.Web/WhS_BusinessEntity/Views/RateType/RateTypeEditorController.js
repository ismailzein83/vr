(function (appControllers) {

    "use strict";

    rateTypeEditorController.$inject = ['$scope', 'WhS_BE_RateTypeAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function rateTypeEditorController($scope, WhS_BE_RateTypeAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var isEditMode;
        var rateTypetId;
        var rateTypeEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                rateTypetId = parameters.RateTypeId;
            }

            isEditMode = (rateTypetId != undefined);
        }

        function defineScope() {
            $scope.saveRateType = function () {
                if (isEditMode) {
                    return updateRateType();
                }
                else {
                    return insertRateType();
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getRateType().then(function () {
                    loadAllControls().finally(function () { rateTypeEntity = undefined; });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getRateType() {
            return WhS_BE_RateTypeAPIService.GetRateType(rateTypetId).then(function (response) {
                rateTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticControls]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(rateTypeEntity ? rateTypeEntity.Name : null, 'Rate Type') : UtilsService.buildTitleForAddEditor('Rate Type');
        }

        function loadStaticControls() {
            if (rateTypeEntity) {
                $scope.name = rateTypeEntity.Name;
            }
        }

        function insertRateType() {
            var rateTypeObject = buildRateTypeObjFromScope();
            $scope.isGettingData = true;
            return WhS_BE_RateTypeAPIService.AddRateType(rateTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Rate Type", response, "Name")) {
                    if ($scope.onRateTypeAdded != undefined)
                        $scope.onRateTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });

        }

        function updateRateType() {
            var rateTypeObject = buildRateTypeObjFromScope();
            $scope.isGettingData = true;

            WhS_BE_RateTypeAPIService.UpdateRateType(rateTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Rate Type", response, "Name")) {
                    if ($scope.onRateTypeUpdated != undefined)
                        $scope.onRateTypeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildRateTypeObjFromScope() {
            var obj = {
                RateTypeId: (rateTypetId != null) ? rateTypetId : 0,
                Name: $scope.name,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_RateTypeEditorController', rateTypeEditorController);

})(appControllers);
