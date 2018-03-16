(function (app) {

    'use strict';

    VolumeCommitmentItemTierEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_VolumeCommitmentService', 'WhS_Deal_VolumeCommitmentTypeEnum'];

    function VolumeCommitmentItemTierEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Deal_VolumeCommitmentService, WhS_Deal_VolumeCommitmentTypeEnum) {

        var volumeCommitmentItemTierEntity;
        var tiers;
        var context;
        var isEditMode;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var zoneDirectiveAPI;

        var rateEvaluatorSelectiveDirectiveAPI;
        var rateEvaluatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                volumeCommitmentItemTierEntity = parametersObj.volumeCommitmentItemTierEntity;
                tiers = parametersObj.tiers;
                context = parametersObj.context;
                $scope.scopeModel.rateEvaluatorSelective = context.getRateEvaluatorSelective();
            }
            isEditMode = (volumeCommitmentItemTierEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.exceptions = [];
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitmentItemTier() : addVolumeCommitmentItemTier();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onrateEvaluatorSelectiveReady = function (api) {
                rateEvaluatorSelectiveDirectiveAPI = api;
                rateEvaluatorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.validateUpToVolume = function () {
                if ($scope.scopeModel.upToVolume != undefined && getPreviousTier() != undefined && parseFloat(getPreviousTier().UpToVolume) >= parseFloat($scope.scopeModel.upToVolume))
                    return "Up To Volume must be greater than " + getPreviousTier().UpToVolume;
                if ($scope.scopeModel.upToVolume != undefined && getNextTier() != undefined && parseFloat(getNextTier().UpToVolume) <= parseFloat($scope.scopeModel.upToVolume))
                    return "Up To Volume must be less than " + getNextTier().UpToVolume;
                return null;
            };

            $scope.scopeModel.resetUpToVolume = function () {
                $scope.scopeModel.upToVolume = undefined;
            };

            $scope.scopeModel.disabelIsLastTier = function () {
                if (!isEditMode && tiers.length == 0)
                    return true;
                if (isEditMode && tiers.length == 1 && $scope.scopeModel.upToVolume != undefined)
                    return true;
                else
                    return getNextTier() != undefined;
            };

            $scope.scopeModel.removeException = function (dataItem) {
                var index = $scope.scopeModel.exceptions.indexOf(dataItem);
                $scope.scopeModel.exceptions.splice(index, 1);
            };

            $scope.scopeModel.addException = function () {
                var onVolumeCommitmentItemTierExRateAdded = function (addedObj) {
                    $scope.scopeModel.exceptions.push({ Entity: addedObj });
                };
                WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItemTierExRate(onVolumeCommitmentItemTierExRateAdded, getContext());
            };

            $scope.scopeModel.disabelAddException = function () {
                return getContext().getExceptionsZoneIds() != undefined ? getContext().getExceptionsZoneIds().length == getContext().getSelectedZonesIds().length : false;
            };

            $scope.scopeModel.exceptionGridMenuActions = [
           {
               name: "Edit",
               clicked: editVolumeCommitmentItemTierException
           }];
        }

        function load() {
            $scope.scopeModel.tiers = tiers != undefined ? filterRetroActiveDataSourceArray(tiers) : [];
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRateEvaluatorSelectiveDirective]).then(function () {
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadRateEvaluatorSelectiveDirective() {
            var loadREWSelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            rateEvaluatorReadyPromiseDeferred.promise.then(function () {

                var payload = undefined;
                if (volumeCommitmentItemTierEntity != undefined && volumeCommitmentItemTierEntity.EvaluatedRate != undefined)
                    payload =
                    {
                        evaluatedRate: volumeCommitmentItemTierEntity.EvaluatedRate
                    };
                VRUIUtilsService.callDirectiveLoad(rateEvaluatorSelectiveDirectiveAPI, payload, loadREWSelectiveDirectivePromiseDeferred);
            });
            return loadREWSelectiveDirectivePromiseDeferred.promise;
        }
        function setTitle() {
            if (isEditMode)
                $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentItemTierEntity.tierName, 'Volume Commitment Item Tier');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment Item Tier');
        }

        function loadStaticData() {
            if (!isEditMode)
                return;
            $scope.scopeModel.isLastTier = volumeCommitmentItemTierEntity.UpToVolume == null;
            $scope.scopeModel.upToVolume = volumeCommitmentItemTierEntity.UpToVolume;
            // $scope.scopeModel.defaultRate = volumeCommitmentItemTierEntity.DefaultRate;
            $scope.scopeModel.selectedRetroActive = UtilsService.getItemByVal($scope.scopeModel.tiers, volumeCommitmentItemTierEntity.RetroActiveFromTierNumber, 'tierId');
            if (volumeCommitmentItemTierEntity.ExceptionZoneRates != null && volumeCommitmentItemTierEntity.ExceptionZoneRates.length > 0)
                buildExceptionZoneRatesDataSource(volumeCommitmentItemTierEntity.ExceptionZoneRates);

        }

        function builVolumeCommitmentItemTierObjFromScope() {
            var exceptions = [];
            if ($scope.scopeModel.exceptions.length > 0) {
                for (var i = 0, length = $scope.scopeModel.exceptions.length; i < length; i++) {
                    var exception = $scope.scopeModel.exceptions[i];
                    exceptions.push(exception.Entity);
                }
            }
            return {
                UpToVolume: $scope.scopeModel.upToVolume,
                EvaluatedRate: rateEvaluatorSelectiveDirectiveAPI.getData(),
                DefaultRate: 0,
                RetroActiveFromTierNumber: ($scope.scopeModel.selectedRetroActive != undefined) ? $scope.scopeModel.selectedRetroActive.tierId : undefined,
                ExceptionZoneRates: exceptions
            };
        }

        function addVolumeCommitmentItemTier() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemTierAdded != undefined) {
                $scope.onVolumeCommitmentItemTierAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        };

        function updateVolumeCommitmentItemTier() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemTierUpdated != undefined) {
                $scope.onVolumeCommitmentItemTierUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function editVolumeCommitmentItemTierException(dataItem) {
            var onVolumeCommitmentItemTierExRateUpdated = function (updatedItem) {
                var index = $scope.scopeModel.exceptions.indexOf(dataItem);
                $scope.scopeModel.exceptions[index] = { Entity: updatedItem };
            };
            WhS_Deal_VolumeCommitmentService.editVolumeCommitmentItemTierExRate(dataItem.Entity, onVolumeCommitmentItemTierExRateUpdated, getContext());
        };

        function buildExceptionZoneRatesDataSource(exceptionZoneRates) {
            for (var i = 0 ; i < exceptionZoneRates.length ; i++) {
                var obj = filterExceptionsZoneRatesByZonesIds(exceptionZoneRates[i], getContext().getSelectedZonesIds());
                if (obj && obj.ZoneIds) {
                    obj.ZoneNames = getContext().getZonesNames(obj.ZoneIds);
                    $scope.scopeModel.exceptions.push({ Entity: obj });
                }

            }
        }

        function getContext() {
            var currentContext = context;
            currentContext.getExceptionsZoneIds = function (exzoneIds) {
                return getSelectedZonesIdsFromTiers(exzoneIds)
            };
            return currentContext;
        }


        function getSelectedZonesIdsFromTiers(includedIds) {
            var ids = getTierUsedZonesIds();
            var filterdIds;
            if (ids != undefined) {
                filterdIds = [];
                for (var x = 0; x < ids.length; x++) {
                    if (includedIds != undefined && includedIds.indexOf(ids[x]) < 0)
                        filterdIds.push(ids[x]);
                    else if (includedIds == undefined)
                        filterdIds.push(ids[x]);
                }
            }
            return filterdIds;
        };

        // collect all Zones ids used in exceptions grid

        function getTierUsedZonesIds() {
            var zonesIds;
            var length = $scope.scopeModel.exceptions;
            if (length > 0) {
                zonesIds = [];
                for (var i = 0; i < length; i++) {
                    var exception = $scope.scopeModel.exceptions[i];
                    zonesIds = zonesIds.concat(exception.Entity.ZoneIds);
                }
            }
            return zonesIds;
        };

        function getPreviousTier() {
            var obj;
            if (!isEditMode && tiers.length > 0)
                obj = tiers[tiers.length - 1];
            if (isEditMode && tiers.length > 0) {
                var index = tiers.indexOf(volumeCommitmentItemTierEntity) - 1;
                obj = tiers[index];
            }
            return obj;
        };

        function getNextTier() {
            var obj;
            if (isEditMode && tiers.length > 0) {
                var index = tiers.indexOf(volumeCommitmentItemTierEntity) + 1;
                obj = tiers[index];
            }
            return obj;
        };

        //  filtering retro active data source base on previouses tiers.

        function filterRetroActiveDataSourceArray(tiers) {
            if (!isEditMode)
                return tiers;
            var filteredArray = new Array();
            for (var i = 0; i < volumeCommitmentItemTierEntity.tierId - 1; i++) {
                filteredArray.push(tiers[i]);
            }
            return filteredArray;
        }

        //  filtering Exceptions Zone Rates base on Zones selected values from item editor.

        function filterExceptionsZoneRatesByZonesIds(excep, ids) {
            var obj = new Object();
            if (ids == undefined)
                return obj;
            var zoneIds = new Array();
            for (var i = 0 ; i < excep.ZoneIds.length ; i++) {
                if (ids.indexOf(excep.ZoneIds[i]) > -1)
                    zoneIds.push(excep.ZoneIds[i]);
            }
            if (zoneIds.length == 0)
                return obj;
            else {
                obj.ZoneIds = zoneIds;
                obj.EvaluatedRate = excep.EvaluatedRate;
            }
            return obj;
        }

    }
    app.controller('WhS_Deal_VolumeCommitmentItemTierEditorController', VolumeCommitmentItemTierEditorController);

})(app);