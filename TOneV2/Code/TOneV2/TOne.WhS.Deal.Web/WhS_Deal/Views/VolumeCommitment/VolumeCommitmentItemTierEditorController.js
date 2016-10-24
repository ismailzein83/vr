(function (appControllers) {

    'use strict';

    VolumeCommitmentItemTierEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function VolumeCommitmentItemTierEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var volumeCommitmentItemTierEntity;
        var context;
        var isEditMode;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var zoneDirectiveAPI;

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                volumeCommitmentItemTierEntity = parametersObj.volumeCommitmentItemTierEntity;
            }
            isEditMode = (volumeCommitmentItemTierEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitmentItem() : addVolumeCommitmentItem();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }



      
        function setTitle() {
            if (isEditMode && volumeCommitmentItemTierEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentItemTierEntity.Name, 'Volume Commitment Item');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment Item');
        }

        function loadStaticData() {
           
        }
    
        function builVolumeCommitmentItemTierObjFromScope() {
            return {
             
            };
        }

        function addVolumeCommitmentItem() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemAdded != undefined) {
                $scope.onVolumeCommitmentItemAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateVolumeCommitmentItem() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemUpdated != undefined) {
                $scope.onVolumeCommitmentItemUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('WhS_Deal_VolumeCommitmentItemTierEditorController', VolumeCommitmentItemTierEditorController);

})(appControllers);