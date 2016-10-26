(function (appControllers) {

    'use stict';

    VolumeCommitmentService.$inject = ['UtilsService', 'VRModalService'];

    function VolumeCommitmentService(UtilsService, VRModalService) {
        return ({
            addVolumeCommitment: addVolumeCommitment,
            editVolumeCommitment: editVolumeCommitment,
            addVolumeCommitmentItem: addVolumeCommitmentItem,
            editVolumeCommitmentItem: editVolumeCommitmentItem,
            addVolumeCommitmentItemTier: addVolumeCommitmentItemTier,
            editVolumeCommitmentItemTier: editVolumeCommitmentItemTier
        });

        function addVolumeCommitment(onVolumeCommitmentAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentAdded = onVolumeCommitmentAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentItemEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentItemEditor.html', parameters, settings);
        }

        function addVolumeCommitmentItemTier(onVolumeCommitmentItemTierAdded,tiers) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierAdded = onVolumeCommitmentItemTierAdded;
            };
            var parameters = {
                tiers: tiers
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentItemtierEditor.html', parameters, settings);
        }

        function editVolumeCommitmentItemTier(volumeCommitmentItemTierEntity, onVolumeCommitmentItemTierUpdated, tiers) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierUpdated = onVolumeCommitmentItemTierUpdated;
            };
            var parameters = {
                volumeCommitmentItemTierEntity: volumeCommitmentItemTierEntity,
                tiers: tiers
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentItemTierEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_Deal_VolumeCommitmentService', VolumeCommitmentService);

})(appControllers);
