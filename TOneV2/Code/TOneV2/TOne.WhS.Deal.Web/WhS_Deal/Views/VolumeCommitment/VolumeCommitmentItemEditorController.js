﻿(function (appControllers) {

    'use strict';

    VolumeCommitmentItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_VolumeCommitmentService'];

    function VolumeCommitmentItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Deal_VolumeCommitmentService) {

        var volumeCommitmentItemEntity;
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
                volumeCommitmentItemEntity = parametersObj.volumeCommitmentItemEntity;
                context = parametersObj.context;
                if (context != undefined)
                    $scope.scopeModel.zoneSelector = context.getZoneSelector();
            }
            isEditMode = (volumeCommitmentItemEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.tiers = [];
            $scope.scopeModel.addTier = function ()
            {
                var onVolumeCommitmentItemtierAdded = function (volumeCommitmentItemTier) {
                    var obj = bulidTierObject(volumeCommitmentItemTier, $scope.scopeModel.tiers.length);
                    $scope.scopeModel.tiers.push(obj);
                }
                WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItemTier(onVolumeCommitmentItemtierAdded, $scope.scopeModel.tiers);
            }

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.tiers != undefined && $scope.scopeModel.tiers.length > 0)
                    return null;
                return "You Should add at least one rate.";
            }
            $scope.scopeModel.removeTier = function (dataItem) {
                var index = $scope.scopeModel.tiers.indexOf(dataItem);
                $scope.scopeModel.tiers.splice(index, 1);
            }
            $scope.scopeModel.onZoneSelectorReady = function(api)
            {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            }
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadZoneSection]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }



        function loadZoneSection() {
            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();

            zoneReadyPromiseDeferred.promise.then(function () {
                var payload = context != undefined ? context.getZoneSelectorPayload(volumeCommitmentItemEntity) : undefined;
                VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, payload, loadZonePromiseDeferred);
            });
            return loadZonePromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode && volumeCommitmentItemEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentItemEntity.Name, 'Volume Commitment Item');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment Item');
        }

        function loadStaticData() {
            if (volumeCommitmentItemEntity != undefined) {
                $scope.scopeModel.name = volumeCommitmentItemEntity.Name;

                if (volumeCommitmentItemEntity.Tiers != undefined && volumeCommitmentItemEntity.Tiers.length > 0) {
                    for (var i = 0; i < volumeCommitmentItemEntity.Tiers.length; i++) {
                        var tier = volumeCommitmentItemEntity.Tiers[i];
                        var obj = bulidTierObject(tier,i)
                        $scope.scopeModel.tiers.push(obj);
                    }
                }
            }
        }
    
        function builVolumeCommitmentItemObjFromScope() {
            var tiers;
            if ($scope.scopeModel.tiers != undefined) {
                tiers = [];
                for (var i = 0; i < $scope.scopeModel.tiers.length; i++) {
                    var tier = $scope.scopeModel.tiers[i];
                    tiers.push({
                        DefaultRate: tier.DefaultRate,
                        UpToVolume: tier.UpToVolume,
                        RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber
                    });
                }
            }
            return {
                Name: $scope.scopeModel.name,
                ZoneIds: zoneDirectiveAPI.getSelectedIds(),
                Tiers: tiers
            };
        }
        function bulidTierObject(tier , index) {
            return {
                tierId: index,
                tierName: 'Tier ' + parseInt(index + 1),
                FromVol: index == 0 ? 0 : $scope.scopeModel.tiers[index - 1].UpToVolume,
                UpToVolume: tier.UpToVolume,
                DefaultRate: tier.DefaultRate,
                RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                RetroActiveFromTier: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').tierName : '',
                RetroActiveVolume: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A'
            };
        }

        function addVolumeCommitmentItem() {
            var parameterObj = builVolumeCommitmentItemObjFromScope();
            if ($scope.onVolumeCommitmentItemAdded != undefined) {
                $scope.onVolumeCommitmentItemAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateVolumeCommitmentItem() {
            var parameterObj = builVolumeCommitmentItemObjFromScope();
            if ($scope.onVolumeCommitmentItemUpdated != undefined) {
                $scope.onVolumeCommitmentItemUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('WhS_Deal_VolumeCommitmentItemEditorController', VolumeCommitmentItemEditorController);

})(appControllers);