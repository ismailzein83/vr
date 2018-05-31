(function (app) {

    'use strict';

    VolumeCommitmentItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_VolumeCommitmentService','WhS_Deal_VolumeCommitmentTypeEnum'];

    function VolumeCommitmentItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Deal_VolumeCommitmentService,WhS_Deal_VolumeCommitmentTypeEnum) {
        $scope.scopeModel = {};
        var volumeCommitmentItemEntity;
        var context;
        var zoneLoaded;
        var isEditMode;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var carrierAccountId;
        var zoneDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                volumeCommitmentItemEntity = parametersObj.volumeCommitmentItemEntity;
                context = parametersObj.context;
                carrierAccountId = parametersObj.carrierAccountId;
                if (context != undefined)
                    $scope.scopeModel.zoneSelector = context.getZoneSelector();
            }
            isEditMode = (volumeCommitmentItemEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel.tiers = [];
            $scope.scopeModel.addTier = function () {
                var onVolumeCommitmentItemTierAdded = function (volumeCommitmentItemTier) {
                    var obj = bulidTierObject(volumeCommitmentItemTier, $scope.scopeModel.tiers.length);
                    $scope.scopeModel.tiers.push(obj);
                };
                WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItemTier(onVolumeCommitmentItemTierAdded, $scope.scopeModel.tiers, getContext());
            };

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.tiers.length == 0)
                    return "You should add at least one tier.";

                if (!hasNotLastTierRecord() && $scope.scopeModel.tiers.length == 1)
                    return "invalid data first tier can not have unknown up to volume.";

                if (hasNotLastTierRecord())
                    return "At least one record should be marked as last tier.";

                var tiersLength = $scope.scopeModel.tiers.length;
                for (var x = 0; x < tiersLength; x++) {
                    var currentTier = $scope.scopeModel.tiers[x];
                    if (currentTier.RetroActiveFromTierNumber != undefined && currentTier.tierId <= currentTier.RetroActiveFromTierNumber)
                        return "Retro Active Tier Number should be less than Tier Number ";
                }

                return null;
            };

            $scope.scopeModel.removeTier = function (dataItem) {
                var index = $scope.scopeModel.tiers.indexOf(dataItem);
                reorderAllTiersAfterRemove(index);
            };

            $scope.scopeModel.onZoneSelectorSelection = function () {
                if (zoneLoaded) {
                    bulidTiersData($scope.scopeModel.tiers);
                }
            };

            $scope.scopeModel.disabelTierAdd = function () {
                return !hasNotLastTierRecord();
            };

            $scope.scopeModel.onZoneSelectorReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSelector = value };
                    var payload = context != undefined ? context.getZoneSelectorPayload(volumeCommitmentItemEntity) : undefined;

                    if (payload != undefined) {
                        if (payload.filter != undefined) {
                            payload.filter.CountryIds = [countryDirectiveApi.getSelectedIds()];

                        }

                        else
                            payload.filter = {
                                CountryIds: [countryDirectiveApi.getSelectedIds()],
                            };
                    }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, zoneDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);

                }
                else if (zoneDirectiveAPI != undefined)
                    $scope.scopeModel.selectedZones.length = 0;
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitmentItem() : addVolumeCommitmentItem();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.tierGridMenuActions = [{
                name: "Edit",
                clicked: editVolumeCommitmentItemTier
            }];
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountryZoneSection]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function getContext() {
            var currentContext = context;
            currentContext.getSelectedZonesIds = function () {
                return zoneDirectiveAPI.getSelectedIds();
            };

            currentContext.getZonesNames = function (ids) {
                var names = [];
                for (var i = 0; i < ids.length; i++) {
                    var attName = currentContext.getZoneIdAttName();
                    var zone = UtilsService.getItemByVal($scope.scopeModel.selectedZones, ids[i], attName);
                    if (zone)
                        names.push(zone.Name);
                }
                return names.join(",");
            };

            currentContext.getCountryId = function () {
                return countryDirectiveApi.getSelectedIds();
            };

            return currentContext;
        }

        function loadCountryZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (volumeCommitmentItemEntity != undefined && volumeCommitmentItemEntity.CountryId != undefined) {
                payload = {};
                payload.selectedIds = volumeCommitmentItemEntity != undefined ? volumeCommitmentItemEntity.CountryId : undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });

            if (volumeCommitmentItemEntity != undefined && volumeCommitmentItemEntity.CountryId != undefined) {
                var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadZonePromiseDeferred.promise);

                UtilsService.waitMultiplePromises([zoneReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var payload = context != undefined ? context.getZoneSelectorPayload(volumeCommitmentItemEntity) : undefined;
                    VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, payload, loadZonePromiseDeferred);
                    loadZonePromiseDeferred.promise.then(function () {
                        if (volumeCommitmentItemEntity != undefined)
                            bulidTiersData(volumeCommitmentItemEntity.Tiers);
                        zoneLoaded = true;
                    });
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
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
                    tiers.push(
                        {
                            DefaultRate: tier.DefaultRate,
                            EvaluatedRate: tier.EvaluatedRate,
                            UpToVolume: tier.UpToVolume,
                            RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                            ExceptionZoneRates: tier.ExceptionZoneRates
                        });
                }
            }
            var saleZones = [];
            var zoneIds = zoneDirectiveAPI.getSelectedIds();
            for (var j = 0; j < zoneIds.length; j++) {
                saleZones.push(
                {
                    ZoneId: zoneIds[j]
                });
            }
            return {
                Name: $scope.scopeModel.name,
                SaleZones: saleZones,
                CountryId: countryDirectiveApi.getSelectedIds(),
                Tiers: tiers
            };
        }

        function bulidTierObject(tier, index, hasException) {
            var tierNb = index + 1;
            return {
                tierId: tierNb,
                tierName: 'Tier ' + parseInt(tierNb),
                FromVol: index == 0 ? 0 : $scope.scopeModel.tiers[index - 1].UpToVolume,
                UpToVolume: tier.UpToVolume,
                Description: tier.Description,
                EvaluatedRate: tier.EvaluatedRate,
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
                var nextItem = $scope.scopeModel.tiers[nextindex];

                if (nextItem != undefined) {
                    $scope.scopeModel.tiers[nextindex] = bulidTierObject(nextItem, nextindex);
                }
            };
            WhS_Deal_VolumeCommitmentService.editVolumeCommitmentItemTier(tierEntity, onVolumeCommitmentItemTierUpdated, $scope.scopeModel.tiers, getContext());
        };

        function findExceptionZoneIds(exrates) {
            var newexrates = [];
            var allselectedzoneids = $scope.scopeModel.selectedZones;

            if (allselectedzoneids == undefined || allselectedzoneids.length == 0)
                return newexrates;

            if (exrates) {
                for (var i = 0 ; i < exrates.length; i++) {

                    if (exrates[i].Zones != undefined) {
                        newexrates[newexrates.length] = {
                            EvaluatedRate: exrates[i].EvaluatedRate,
                            Zones: exrates[i].Zones,
                            Description: exrates[i].Description
                        }
                    }
                };
            }
            return newexrates;
        }

        function bulidTiersData(tiers) {
            var newTiersArray = [];
            for (var i = 0 ; i < tiers.length; i++) {
                var tier = tiers[i];
                var tierExceptions = findExceptionZoneIds(tier.ExceptionZoneRates);
                var tierNb = i + 1;
                var obj = {
                    tierId: tierNb,
                    tierName: 'Tier ' + parseInt(tierNb),
                    FromVol: i == 0 ? 0 : tiers[i - 1].UpToVolume,
                    UpToVolume: tier.UpToVolume,
                    Description: tier.Description,
                    EvaluatedRate: tier.EvaluatedRate,
                    RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber,
                    RetroActiveFromTier: (tier.RetroActiveFromTierNumber != undefined) ? 'Tier ' + parseInt(tier.RetroActiveFromTierNumber) : '',
                    RetroActiveVolume: (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal(newTiersArray, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A',
                    ExceptionZoneRates: tierExceptions,
                    HasException: tierExceptions.length > 0
                };
                newTiersArray.push(obj);
            };

            $scope.scopeModel.tiers = newTiersArray;
        };

        function reorderAllTiersAfterRemove(index) {
            if (index + 1 == $scope.scopeModel.tiers.length) {
                $scope.scopeModel.tiers.splice(index, 1);
                return;
            }
            var deletedTierNb = index + 1;
            var newTiersArray = [];
            for (var i = 0 ; i < $scope.scopeModel.tiers.length; i++) {
                var tier = $scope.scopeModel.tiers[i];
                if (i != index) {

                    var obj = {
                        UpToVolume: tier.UpToVolume,
                        DefaultRate: tier.DefaultRate,
                        Description: tier.Description,
                        EvaluatedRate: tier.EvaluatedRate,
                        RetroActiveFromTierNumber: tier.RetroActiveFromTierNumber != undefined ? deletedTierNb >= tier.RetroActiveFromTierNumber ? tier.RetroActiveFromTierNumber : (tier.RetroActiveFromTierNumber - 1) : undefined,
                        ExceptionZoneRates: tier.ExceptionZoneRates,
                        HasException: tier.ExceptionZoneRates != undefined && tier.ExceptionZoneRates.length > 0
                    };
                    newTiersArray.push(obj);
                };
            };
            rebuildTiersArray(newTiersArray);
        };

        function rebuildTiersArray(array) {
            for (var j = 0 ; j < array.length; j++) {
                var tierNb = j + 1;
                var tier = array[j];
                tier.tierId = tierNb;
                tier.tierName = 'Tier ' + parseInt(tierNb);
                tier.FromVol = j == 0 ? 0 : array[j - 1].UpToVolume;
                tier.RetroActiveFromTier = (tier.RetroActiveFromTierNumber != undefined) ? 'Tier ' + parseInt(tier.RetroActiveFromTierNumber) : '';
                tier.RetroActiveVolume = (tier.RetroActiveFromTierNumber != undefined) ? UtilsService.getItemByVal(array, tier.RetroActiveFromTierNumber, 'tierId').FromVol : 'N/A';
            }
            $scope.scopeModel.tiers = array;
        };

        function containesInZonesIds(element, index, array) {
            return zoneDirectiveAPI.getSelectedIds().indexOf(element) > -1;
        };
    }

    app.controller('WhS_Deal_VolumeCommitmentItemEditorController', VolumeCommitmentItemEditorController);

})(app);