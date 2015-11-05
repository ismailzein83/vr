(function (appControllers) {

    "use strict";

    rateTypeEditorController.$inject = ['$scope', 'WhS_BE_RateTypeAPIService', 'VRNotificationService', 'VRNavigationService'];

    function rateTypeEditorController($scope, WhS_BE_RateTypeAPIService, VRNotificationService, VRNavigationService) {


        var rateTypetId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                rateTypetId = parameters.RateTypeId;
            }
            editMode = (rateTypetId != undefined);
        }
        function defineScope() {
            $scope.saveRateType = function () {
                if (editMode) {
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
            $scope.isGettingData = true;
            if (editMode) {
                getRateType();
            }
            else {
                $scope.isGettingData = false;
            }

        }
        function getRateType() {
            return WhS_BE_RateTypeAPIService.GetRateType(rateTypetId).then(function (rateType) {
                fillScopeFromRateTypeObj(rateType);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
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

        function fillScopeFromRateTypeObj(rateType) {
            $scope.name = rateType.Name;
        }
        function insertRateType() {
            var rateTypeObject = buildRateTypeObjFromScope();
            return WhS_BE_RateTypeAPIService.AddRateType(rateTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("RateType", response)) {
                    if ($scope.onRateTypeAdded != undefined)
                        $scope.onRateTypeAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updateRateType() {
            var rateTypeObject = buildRateTypeObjFromScope();
            WhS_BE_RateTypeAPIService.UpdateRateType(rateTypeObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("RateType", response)) {
                    if ($scope.onRateTypeUpdated != undefined)
                        $scope.onRateTypeUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_RateTypeEditorController', rateTypeEditorController);
})(appControllers);
