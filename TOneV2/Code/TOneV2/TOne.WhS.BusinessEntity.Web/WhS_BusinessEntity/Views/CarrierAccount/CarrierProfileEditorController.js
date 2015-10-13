(function (appControllers) {

    "use strict";

    carrierProfileEditorController.$inject = ['$scope', 'WhS_BE_CarrierProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function carrierProfileEditorController($scope, WhS_BE_CarrierProfileAPIService, UtilsService, VRNotificationService, VRNavigationService) {
        var editMode;
        var carrierProfileId;
        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierProfileId = parameters.CarrierProfileId;
            }
            editMode = (carrierProfileId != undefined);

        }

        function defineScope() {
            $scope.SaveCarrierProfile = function () {
                if (editMode) {
                    return updateCarrierProfile();
                }
                else {
                    return insertCarrierProfile();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isGettingData = true;
            if (editMode) {
                getCarrierProfile();
            }
            else {
                $scope.isGettingData = false;
            }
        }

        function getCarrierProfile() {
            return WhS_BE_CarrierProfileAPIService.GetCarrierProfile(carrierProfileId).then(function (carrierProfile) {
                fillScopeFromCarrierProfileObj(carrierProfile);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function fillScopeFromCarrierProfileObj(carrierProfile) {
            $scope.name = carrierProfile.Name;
        }

        function insertCarrierProfile() {
            var carrierProfileObject = buildCarrierProfileObjFromScope();
            return WhS_BE_CarrierProfileAPIService.AddCarrierProfile(carrierProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Carrier Profile", response)) {
                    if ($scope.onCarrierProfileAdded != undefined)
                        $scope.onCarrierProfileAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function buildCarrierProfileObjFromScope() {
            var obj = {
                CarrierProfileId: (carrierProfileId != null) ? carrierProfileId : 0,
                Name: $scope.name,
            };
            return obj;
        }

        function updateCarrierProfile() {
            var carrierProfileObject = buildCarrierProfileObjFromScope();
            WhS_BE_CarrierProfileAPIService.UpdateCarrierProfile(carrierProfileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Carrier Profile", response)) {
                    if ($scope.onCarrierProfileUpdated != undefined)
                        $scope.onCarrierProfileUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('WhS_BE_CarrierProfileEditorController', carrierProfileEditorController);
})(appControllers);
