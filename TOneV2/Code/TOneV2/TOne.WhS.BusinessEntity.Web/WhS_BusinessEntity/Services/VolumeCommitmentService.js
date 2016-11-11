(function (appControllers) {

    'use strict';

    VolumeCommitmentService.$inject = ['UtilsService', 'VRModalService'];

    function VolumeCommitmentService(UtilsService, VRModalService) {
        return ({
            addVolumeCommitment: addVolumeCommitment,
            editVolumeCommitment: editVolumeCommitment,
            addVolumeCommitmentItem: addVolumeCommitmentItem,
            editVolumeCommitmentItem: editVolumeCommitmentItem
        });

        function addVolumeCommitment(onVolumeCommitmentAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentAdded = onVolumeCommitmentAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Deal/VolumeCommitmentEditor.html', parameters, settings);
        }

        function editVolumeCommitment(volumeCommitmentId, onVolumeCommitmentUpdated) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentUpdated = onVolumeCommitmentUpdated;
            };
            var parameters = {
                volumeCommitmentId: volumeCommitmentId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Deal/VolumeCommitmentEditor.html', parameters, settings);
        }

        function addVolumeCommitmentItem(onVolumeCommitmentItemAdded,context) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemAdded = onVolumeCommitmentItemAdded;
            };
            var parameters = {
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Deal/VolumeCommitmentItemEditor.html', parameters, settings);
        }

        function editVolumeCommitmentItem(volumeCommitmentItemEntity, onVolumeCommitmentItemUpdated, context) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemUpdated = onVolumeCommitmentItemUpdated;
            };
            var parameters = {
                volumeCommitmentItemEntity: volumeCommitmentItemEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Deal/VolumeCommitmentItemEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_BE_VolumeCommitmentService', VolumeCommitmentService);

})(appControllers);
