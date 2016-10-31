(function (app) {

    'use strict';

    VolumeCommitmentItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_VolumeCommitmentService'];

    function VolumeCommitmentItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Deal_VolumeCommitmentService) {

        var volumeCommitmentItemEntity;
        var context;
        var isEditMode;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var zoneDirectiveAPI;
        var zoneSelectionPromise;
        var zoneLoaded;
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
                var onVolumeCommitmentItemTierAdded = function (volumeCommitmentItemTier) {
                    var obj = bulidTierObject(volumeCommitmentItemTier, $scope.scopeModel.tiers.length);
                    $scope.scopeModel.tiers.push(obj);
                }
                WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItemTier(onVolumeCommitmentItemTierAdded, $scope.scopeModel.tiers, getContext());
            }
           
            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.tiers.length== 0)
                    return "You Should add at least one tier.";
                if (!hasNotLastTierRecord() && $scope.scopeModel.tiers.length == 1 )
                    return "invalide data first tier can not have unknown up to volume.";
                if (hasNotLastTierRecord())
                    return "At least one record  should be marked as last tier.";
                return null;
            }
            $scope.scopeModel.removeTier = function (dataItem) {
                var index = $scope.scopeModel.tiers.indexOf(dataItem);
                reorderAllTier(index);
            };

            $scope.scopeModel.onZoneSelectorSelection = function () {
                if (zoneLoaded) {
                    rebulidTiers($scope.scopeModel.tiers);
                };
            };

            $scope.scopeModel.disabelTierAdd = function () {
                return !hasNotLastTierRecord();
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
            $scope.scopeModel.tierGridMenuActions = [
            {
                name: "Edit",
                clicked: editVolumeCommitmentItemTier
            }];

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

        function getContext() {
            var currentContext = context;
            currentContext.getSelectedZonesIds = function () {
               return zoneDirectiveAPI.getSelectedIds();
            }
            currentContext.getZonesNames = function (ids) {
                var names = [];
                for (var i = 0; i < ids.length; i++){
                    var attName = currentContext.getZoneIdAttName();                
                    var zone = UtilsService.getItemByVal($scope.scopeModel.selectedZones, ids[i], attName);
                    if(zone)
                        names.push(zone.Name);
                }
                return names.join(",");
            };



            return currentContext;
        }

        function loadZoneSection() {

            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();
            var payload;
            if (volumeCommitmentItemEntity != undefined && volumeCommitmentItemEntity.ZoneIds != undefined) {
                payload = context != undefined ? context.getZoneSelectorPayload(volumeCommitmentItemEntity) : undefined;
                zoneSelectionPromise = UtilsService.createPromiseDeferred();
            }
            zoneReadyPromiseDeferred.promise.then(function () {              
                VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, payload, loadZonePromiseDeferred)
                loadZonePromiseDeferred.promise.then(function () {
                    rebulidTiers(volumeCommitmentItemEntity.Tiers);
                    zoneLoaded = true;
                });
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
                        RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                        ExceptionZoneRates: tier.ExceptionZoneRates
                    });
                }
            }
            return {
                Name: $scope.scopeModel.name,
                ZoneIds: zoneDirectiveAPI.getSelectedIds(),
                Tiers: tiers
            };
        }

        function bulidTierObject(tier, index, hasException) {
            return {
                tierId: index,
                tierName: 'Tier ' + parseInt(index + 1),
                FromVol: index == 0 ? 0 : $scope.scopeModel.tiers[index - 1].UpToVolume,
                UpToVolume: tier.UpToVolume,
                DefaultRate: tier.DefaultRate,
                RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                RetroActiveFromTier: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').tierName : '',
                RetroActiveVolume: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A',
                ExceptionZoneRates: tier.ExceptionZoneRates,
                HasException: (hasException != undefined) ? hasException : tier.ExceptionZoneRates != undefined && tier.ExceptionZoneRates.length > 0
            };
        };

        function hasNotLastTierRecord() {
            for (var i = 0; i < $scope.scopeModel.tiers.length; i++) {
                var tier = $scope.scopeModel.tiers[i];
                if (tier.UpToVolume == null) return false;
            }
            return true;

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

        function editVolumeCommitmentItemTier(tierEntity) {

            var onVolumeCommitmentItemTierUpdated = function (updatedItem) {
                var index = $scope.scopeModel.tiers.indexOf(tierEntity);
                $scope.scopeModel.tiers[index] = bulidTierObject(updatedItem, index);
                var nextindex = index + 1;
                if ($scope.scopeModel.tiers[nextindex] != undefined) {
                    $scope.scopeModel.tiers[nextindex] = bulidTierObject($scope.scopeModel.tiers[nextindex], nextindex);
                }
            };
            WhS_Deal_VolumeCommitmentService.editVolumeCommitmentItemTier(tierEntity, onVolumeCommitmentItemTierUpdated, $scope.scopeModel.tiers, getContext());
        };

        function findExceptionZoneIds(exrates) {
            var newexrates = []
            var allselectedzoneids = $scope.scopeModel.selectedZones;
            if (allselectedzoneids == undefined || allselectedzoneids.length==0)
                return newexrates;
            for (var i = 0 ; i < exrates.length; i++) {
                if (exrates[i].ZoneIds.some(containesInZonesIds)) {
                    newexrates[newexrates.length] = {
                        Rate: exrates[i].Rate,
                        ZoneIds: exrates[i].ZoneIds.filter(containesInZonesIds)
                    }
                }
            };
            return newexrates;
        }

        function reorderAllTier(index) {
            if (index + 1 == $scope.scopeModel.tiers.length) {
                $scope.scopeModel.tiers.splice(index, 1);
                return;
            }
            var newTiersArray = [];
            for (var i = 0 ; i < $scope.scopeModel.tiers.length; i++) {
                var tier = $scope.scopeModel.tiers[i];
                if (i != index) {
                    var obj = {
                        UpToVolume: tier.UpToVolume,
                        DefaultRate: tier.DefaultRate,
                        RetroActiveFromTierNumber: (tier.RetroActiveFromTierNumber != index) ? tier.RetroActiveFromTierNumber : undefined,
                        RetroActiveFromTier: (tier.RetroActiveFromTierNumber != undefined && tier.RetroActiveFromTierNumber != index) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').tierName : '',
                        RetroActiveVolume: (tier.RetroActiveFromTierNumber != undefined && tier.RetroActiveFromTierNumber != index) ? UtilsService.getItemByVal($scope.scopeModel.tiers, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A',
                        ExceptionZoneRates: tier.ExceptionZoneRates,
                        HasException: tier.ExceptionZoneRates!= undefined && tier.ExceptionZoneRates.length > 0 
                }
                newTiersArray.push(obj);
            };
         };
           rebuildTier(newTiersArray);
        };

        function rebulidTiers(tiers) {
            var newTiersArray = [];
            for (var i = 0 ; i < tiers.length; i++) {
                var tier = tiers[i];
                var obj = {
                    UpToVolume: tier.UpToVolume,
                    DefaultRate: tier.DefaultRate,
                    RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                    RetroActiveFromTier: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal(tiers, tier.RetroActiveFromTierNumber, 'tierId').tierName : '',
                    RetroActiveVolume: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal(tiers, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A',
                    ExceptionZoneRates: findExceptionZoneIds(tier.ExceptionZoneRates),
                    HasException: findExceptionZoneIds(tier.ExceptionZoneRates).length > 0 
                }
                newTiersArray.push(obj);
            };            
            rebuildTier(newTiersArray);
        };
    
        function rebuildTier(array) {
            for (var j = 0 ; j < array.length; j++) {
                var tier = array[j];
                tier.tierId = j;
                tier.tierName = 'Tier ' + parseInt(j + 1);
                tier.FromVol = j == 0 ? 0 : array[j - 1].UpToVolume;
            };
            $scope.scopeModel.tiers = array;
        };

        function containesInZonesIds(element, index, array) {
            return zoneDirectiveAPI.getSelectedIds().indexOf(element) > -1;
        };
    }
    app.controller('WhS_Deal_VolumeCommitmentItemEditorController', VolumeCommitmentItemEditorController);

})(app);