(function (appControllers) {

    'use strict';

    VolumeCommitmentItemTierEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Deal_VolumeCommitmentService'];

    function VolumeCommitmentItemTierEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Deal_VolumeCommitmentService) {

        var volumeCommitmentItemTierEntity;
        var tiers;
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
                tiers = parametersObj.tiers;
                context = parametersObj.context;
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

            $scope.scopeModel.validateUpToVolume = function () {
               
                if ($scope.scopeModel.upToVolume != undefined && getPeriousTier() != undefined && parseFloat(getPeriousTier().UpToVolume) >= parseFloat($scope.scopeModel.upToVolume) )
                    return "Up To Volume must be greater than " + getPeriousTier().UpToVolume;
                if ($scope.scopeModel.upToVolume != undefined && getPeriousTier() != undefined && !getPeriousTier().UpToVolume && parseFloat(getPeriousTier().FromVol) >= parseFloat($scope.scopeModel.upToVolume))
                    return "Up To Volume must be greater than " + getPeriousTier().FromVol;
                if ($scope.scopeModel.upToVolume != undefined && getNextTier()!= undefined && parseFloat(getNextTier().UpToVolume) <= parseFloat($scope.scopeModel.upToVolume))
                    return "Up To Volume must be less than " + getNextTier().UpToVolume;
                return null;
            };
            $scope.scopeModel.resetUpToVolume = function () {
                $scope.scopeModel.upToVolume = undefined;
            };
            $scope.scopeModel.disabelIsLastTier = function () {
                if (!isEditMode && tiers.length == 0 )
                    return true ;
                if (isEditMode && tiers.length == 1 )
                    return true ;
                else
                    return getNextTier() != undefined;
            }
            $scope.scopeModel.removeException = function (dataItem) {
                var index = $scope.scopeModel.exceptions.indexOf(dataItem);
                $scope.scopeModel.exceptions.splice(index, 1);
            };
            $scope.scopeModel.addException  = function () {
                var onVolumeCommitmentItemTierExRateAdded = function (addedObj) {
                    $scope.scopeModel.exceptions.push(addedObj);
                }
                WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItemTierExRate(onVolumeCommitmentItemTierExRateAdded, getContext());
            };

            $scope.scopeModel.disabelAddException = function () {
                return getContext().getExceptionsZoneIds()!= undefined ? getContext().getExceptionsZoneIds().length == getContext().getSelectedZonesIds().length : false;
            }
        }

        function load() {

            $scope.scopeModel.tiers = tiers != undefined ?  bulidNewArray(tiers) : [];
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


        function getContext()
        {
            var currentContext = context;

            currentContext.getExceptionsZoneIds = function () {
                var zonesIds;
                var exceptions = $scope.scopeModel.exceptions;
                if(exceptions.length == 0 )
                    return zonesIds;
                zonesIds = [];
                for (var i = 0; i < exceptions.length; i++) {
                    zonesIds =  zonesIds.concat(exceptions[i].ZoneIds);

                }
                return zonesIds;

            }
           
            return currentContext;
        }
      
        function setTitle() {
            if (isEditMode )
                $scope.title = 'Edit Tier';
            else
                $scope.title = 'Add Tier';
        }

        function loadStaticData() {
            if (!isEditMode)
                return;

            $scope.scopeModel.isLastTier = volumeCommitmentItemTierEntity.UpToVolume == null;
            $scope.scopeModel.upToVolume = volumeCommitmentItemTierEntity.UpToVolume;
            $scope.scopeModel.defaultRate = volumeCommitmentItemTierEntity.DefaultRate;
            $scope.scopeModel.selectedRetroActive = UtilsService.getItemByVal($scope.scopeModel.tiers, volumeCommitmentItemTierEntity.RetroActiveFromTierNumber, 'tierId');
            if (volumeCommitmentItemTierEntity.ExceptionZoneRates != null && volumeCommitmentItemTierEntity.ExceptionZoneRates.length > 0)
                buildExceptionZoneRatesDataSource(volumeCommitmentItemTierEntity.ExceptionZoneRates);

        }
    
        function builVolumeCommitmentItemTierObjFromScope() {
            return {
                UpToVolume: $scope.scopeModel.upToVolume,
                DefaultRate: $scope.scopeModel.defaultRate,
                RetroActiveFromTierNumber: ($scope.scopeModel.selectedRetroActive != undefined) ? $scope.scopeModel.selectedRetroActive.tierId : undefined,
                ExceptionZoneRates: $scope.scopeModel.exceptions
            };
        }

        function addVolumeCommitmentItemTier() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemTierAdded != undefined) {
                $scope.onVolumeCommitmentItemTierAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateVolumeCommitmentItemTier() {
            var parameterObj = builVolumeCommitmentItemTierObjFromScope();
            if ($scope.onVolumeCommitmentItemTierUpdated != undefined) {
                $scope.onVolumeCommitmentItemTierUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }


        function buildExceptionZoneRatesDataSource(exceptionZoneRates) {
            for (var i = 0 ; i < exceptionZoneRates.length ; i++) {
                var obj = exceptionZoneRates[i];
                obj.ZoneNames = getContext().getZonesNames(exceptionZoneRates[i].ZoneIds);
                $scope.scopeModel.exceptions.push(obj);
            }
        }
        function getPeriousTier() {
            var obj;
            if (!isEditMode && tiers.length > 0)
                obj =  tiers[tiers.length - 1];
            if (isEditMode && tiers.length > 0) {
                var index = tiers.indexOf(volumeCommitmentItemTierEntity) -1 ;
                obj = tiers[index];
            }
            return obj;
        };
        function getNextTier() {
            var obj;
            if (isEditMode && tiers.length > 0) {
                var index = tiers.indexOf(volumeCommitmentItemTierEntity) + 1;
                obj =  tiers[index];
            }            
            return obj;
        };

        function bulidNewArray(tiers) {
            if(!isEditMode)
                return tiers;
            var arr = new Array();
            for (var i = 0; i < volumeCommitmentItemTierEntity.tierId; i++) {
                   arr.push(tiers[i]);
            }
            return arr;
        }

    }
    appControllers.controller('WhS_Deal_VolumeCommitmentItemTierEditorController', VolumeCommitmentItemTierEditorController);

})(appControllers);