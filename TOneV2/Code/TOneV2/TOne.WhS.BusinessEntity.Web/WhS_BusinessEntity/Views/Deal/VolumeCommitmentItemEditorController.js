(function (appControllers) {

    'use strict';

    VolumeCommitmentItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function VolumeCommitmentItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

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
            $scope.scopeModel.rates = [];
            $scope.scopeModel.addRate = function()
            {
                var dataItem = {
                    UpToVolume: 0,
                    Rate: 0,
                    IsRetroActive:false
                };
                $scope.scopeModel.rates.push(dataItem);
            }

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.rates != undefined && $scope.scopeModel.rates.length > 0)
                    return null;
                return "You Should add at least one rate.";
            }


            $scope.scopeModel.removeRate = function (dataItem) {
                var index = $scope.scopeModel.rates.indexOf(dataItem);
                $scope.scopeModel.rates.splice(index, 1);
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

                if (volumeCommitmentItemEntity.Rates != undefined && volumeCommitmentItemEntity.Rates.length > 0) {
                    for (var i = 0; i < volumeCommitmentItemEntity.Rates.length; i++) {
                        var rate = volumeCommitmentItemEntity.Rates[i];
                        $scope.scopeModel.rates.push(rate);
                    }
                }
            }
        }
    
        function builVolumeCommitmentItemObjFromScope() {
            var rates;
            if ($scope.scopeModel.rates != undefined) {
                rates = [];
                for (var i = 0; i < $scope.scopeModel.rates.length; i++) {
                    var rate = $scope.scopeModel.rates[i];
                    rates.push({
                        Rate: rate.Rate,
                        UpToVolume: rate.UpToVolume,
                        IsRetroActive: rate.IsRetroActive,
                    });
                }
            }
            return {
                Name: $scope.scopeModel.name,
                ZoneIds: zoneDirectiveAPI.getSelectedIds(),
                Rates:rates
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
    appControllers.controller('WhS_BE_VolumeCommitmentItemEditorController', VolumeCommitmentItemEditorController);

})(appControllers);