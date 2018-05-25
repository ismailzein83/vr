(function (appControllers) {

    'use strict';

    VolumeCommitmentService.$inject = ['UtilsService', 'VRModalService', 'VRCommon_ObjectTrackingService'];

    function VolumeCommitmentService(UtilsService, VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addVolumeCommitment: addVolumeCommitment,
            editVolumeCommitment: editVolumeCommitment,
            addVolumeCommitmentItem: addVolumeCommitmentItem,
            editVolumeCommitmentItem: editVolumeCommitmentItem,
            addVolumeCommitmentItemTier: addVolumeCommitmentItemTier,
            editVolumeCommitmentItemTier: editVolumeCommitmentItemTier,
            addVolumeCommitmentItemTierExRate: addVolumeCommitmentItemTierExRate,
            editVolumeCommitmentItemTierExRate: editVolumeCommitmentItemTierExRate,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToVolCommitmentDeal: registerObjectTrackingDrillDownToVolCommitmentDeal,
            registerHistoryViewAction: registerHistoryViewAction
        });
        function viewHistoryVolumeCommitment(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/WhS_Deal/Views/VolumeCommitment/VolumeCommitmentEditor.html', modalParameters, modalSettings);
        }
        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_Deal_VolCommitmentDeal_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryVolumeCommitment(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }
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

        function addVolumeCommitmentItem(onVolumeCommitmentItemAdded, context, carrierAccountId, dealId, dealBED, dealEED, volumeCommitmentType) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemAdded = onVolumeCommitmentItemAdded;
            };
            var parameters = {
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED,
                volumeCommitmentType: volumeCommitmentType
            };

            VRModalService.showModal('/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemEditor.html', parameters, settings);
        }

        function editVolumeCommitmentItem(volumeCommitmentItemEntity, onVolumeCommitmentItemUpdated, context, carrierAccountId, dealId, dealBED, dealEED, volumeCommitmentType) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVolumeCommitmentItemUpdated = onVolumeCommitmentItemUpdated;
            };
            var parameters = {
                volumeCommitmentItemEntity: volumeCommitmentItemEntity,
                context: context,
                carrierAccountId: carrierAccountId,
                dealId: dealId,
                dealBED: dealBED,
                dealEED: dealEED,
                volumeCommitmentType: volumeCommitmentType
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


        function getEntityUniqueName() {
            return "WhS_Deal_VolCommitmentDeal";
        }

        function registerObjectTrackingDrillDownToVolCommitmentDeal() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, volCommitmentDealItem) {
                volCommitmentDealItem.objectTrackingGridAPI = directiveAPI;
               
                var query = {
                    ObjectId: volCommitmentDealItem.Entity.DealId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return volCommitmentDealItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }

    appControllers.service('WhS_Deal_VolumeCommitmentService', VolumeCommitmentService);

})(appControllers);
