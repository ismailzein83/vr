(function (appControllers) {

    'use strict';

    VolumeCommitmentService.$inject = ['UtilsService', 'VRModalService'];

    function VolumeCommitmentService(UtilsService, VRModalService) {
        return ({
            addVolumeCommitment: addVolumeCommitment,
            editVolumeCommitment: editVolumeCommitment,
            addVolumeCommitmentItem: addVolumeCommitmentItem,
            editVolumeCommitmentItem: editVolumeCommitmentItem,
            addVolumeCommitmentItemTier: addVolumeCommitmentItemTier,
            editVolumeCommitmentItemTier: editVolumeCommitmentItemTier,
            addVolumeCommitmentItemTierExRate: addVolumeCommitmentItemTierExRate,
            editVolumeCommitmentItemTierExRate: editVolumeCommitmentItemTierExRate
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

        function editVolumeCommitment(dealId, onVolumeCommitmentUpdated) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentUpdated = onVolumeCommitmentUpdated;
            };
            var parameters = {
                dealId: dealId
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

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemEditor.html', parameters, settings);
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

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemEditor.html', parameters, settings);
        }

        function addVolumeCommitmentItemTier(onVolumeCommitmentItemTierAdded, tiers, context) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierAdded = onVolumeCommitmentItemTierAdded;
            };
            var parameters = {
                tiers: tiers,
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemtierEditor.html', parameters, settings);
        }

        function editVolumeCommitmentItemTier(volumeCommitmentItemTierEntity, onVolumeCommitmentItemTierUpdated, tiers, context) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierUpdated = onVolumeCommitmentItemTierUpdated;
            };
            var parameters = {
                volumeCommitmentItemTierEntity: volumeCommitmentItemTierEntity,
                tiers: tiers,
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemTierEditor.html', parameters, settings);
        }

        function addVolumeCommitmentItemTierExRate(onVolumeCommitmentItemTierExRateAdded,context) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierExRateAdded = onVolumeCommitmentItemTierExRateAdded;
            };
            var parameters = {
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemTierExRateEditor.html', parameters, settings);
        }

        function editVolumeCommitmentItemTierExRate(exRateEntity, onVolumeCommitmentItemTierExRateUpdated, context) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemTierExRateUpdated = onVolumeCommitmentItemTierExRateUpdated;
            };
            var parameters = {
                exRateEntity: exRateEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemTierExRateEditor.html', parameters, settings);
        }

    }

    appControllers.service('WhS_Deal_VolumeCommitmentService', VolumeCommitmentService);

})(appControllers);
