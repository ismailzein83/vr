(function (appControllers) {

    "use strict";

    rateTypeEditorController.$inject = ['$scope', 'VRCommon_RateTypeAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function rateTypeEditorController($scope, VRCommon_RateTypeAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var isEditMode;
        var rateTypetId;
        var rateTypeEntity;
        var context;
        var isViewHistoryMode;
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                rateTypetId = parameters.RateTypeId;
                context = parameters.context;
            }
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            isEditMode = (rateTypetId != undefined);
        }

        function defineScope() {
            $scope.hasSaveRateTypePermission = function () {
                if (isEditMode)
                    return VRCommon_RateTypeAPIService.HasUpdateRateTypePermission();
                else
                    return VRCommon_RateTypeAPIService.HasAddRateTypePermission();
            };

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
            else if (isViewHistoryMode) {
                getRateTypeHistory().then(function () {
                    loadAllControls().finally(function () {
                        rateTypeEntity = undefined;
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
        function getRateTypeHistory() {
            return VRCommon_RateTypeAPIService.GetRateTypeHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                rateTypeEntity = response;

            });
        }
        function getRateType() {
            return VRCommon_RateTypeAPIService.GetRateType(rateTypetId).then(function (response) {
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
         
            if (isEditMode && rateTypeEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(rateTypeEntity.Name, "Rate Type");
            else if (isViewHistoryMode && rateTypeEntity != undefined)
                $scope.title = "View Rate Type: " + rateTypeEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Rate Type");
        }

        function loadStaticControls() {
            if (rateTypeEntity != undefined) {
                $scope.name = rateTypeEntity.Name;
            }
        }

        function insertRateType() {
            var rateTypeObject = buildRateTypeObjFromScope();
            $scope.isGettingData = true;
            return VRCommon_RateTypeAPIService.AddRateType(rateTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Rate Type", response, "Name")) {
                    if ($scope.onRateTypeAdded != undefined && typeof ($scope.onRateTypeAdded) == 'function') {
                        $scope.onRateTypeAdded(response.InsertedObject);
                    }
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

            VRCommon_RateTypeAPIService.UpdateRateType(rateTypeObject)
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
                Name: $scope.name
            };
            return obj;
        }
    }

    appControllers.controller('VRCommon_RateTypeEditorController', rateTypeEditorController);

})(appControllers);