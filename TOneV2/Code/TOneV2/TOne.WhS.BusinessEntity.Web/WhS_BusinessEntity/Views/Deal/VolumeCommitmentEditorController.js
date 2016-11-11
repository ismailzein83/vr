(function (appControllers) {

    'use strict';

    VolumeCommitmentEditorController.$inject = ['$scope', 'WhS_BE_VolumeCommitmentAPIService',  'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_VolumeCommitmentService','WhS_BE_VolumeCommitmentTypeEnum'];

    function VolumeCommitmentEditorController($scope, WhS_BE_VolumeCommitmentAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_VolumeCommitmentService, WhS_BE_VolumeCommitmentTypeEnum) {
        var isEditMode;

        var volumeCommitmentId;
        var volumeCommitmentEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred;

        var volumeCommitmenetItemsAPI;
        var volumeCommitmenetItemsReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                volumeCommitmentId = parameters.volumeCommitmentId;

            isEditMode = (volumeCommitmentId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.gracePeriod = 7;
            $scope.scopeModel.volumeCommitmentTypes = UtilsService.getArrayEnum(WhS_BE_VolumeCommitmentTypeEnum);
            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountSelectorAPI, payload, setLoader, carrierAccountSelectorReadyDeferred);
            };

            $scope.scopeModel.onVolumeCommitmenetItemsReady = function (api) {
                volumeCommitmenetItemsAPI = api;
                volumeCommitmenetItemsReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitment() : insertVolumeCommitment();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVolumeCommitment().then(function () {
                        loadAllControls().finally(function () {
                            //volumeCommitmentEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVolumeCommitment() {
            return WhS_BE_VolumeCommitmentAPIService.GetVolumeCommitment(volumeCommitmentId).then(function (response) {
                volumeCommitmentEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCarrierAccountSelector, loadVolumeCommitmenetItems]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                if (volumeCommitmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentEntity.Settings.Description, 'VolumeCommitment');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('VolumeCommitment');
        }

        function loadStaticData() {
            if (volumeCommitmentEntity == undefined)
                return;
            $scope.scopeModel.description = volumeCommitmentEntity.Settings.Description;
            $scope.scopeModel.gracePeriod = volumeCommitmentEntity.Settings.GracePeriod;
            $scope.scopeModel.beginDate = volumeCommitmentEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = volumeCommitmentEntity.Settings.EndDate;
            $scope.scopeModel.active = volumeCommitmentEntity.Settings.Active;
            $scope.scopeModel.selectedVolumeCommitmentType = UtilsService.getItemByVal($scope.scopeModel.volumeCommitmentTypes, volumeCommitmentEntity.Settings.Type, "value");
        }

        function loadCarrierAccountSelector() {
            if (volumeCommitmentEntity == undefined)
                return;
            carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                carrierAccountSelectorReadyDeferred = undefined;
                var payload = (volumeCommitmentEntity != undefined) ? { selectedIds: volumeCommitmentEntity.Settings.CarrierAccountId } : undefined;
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }
        function loadVolumeCommitmenetItems() {
            var volumeCommitmenetItemsLoadDeferred = UtilsService.createPromiseDeferred();

            volumeCommitmenetItemsReadyDeferred.promise.then(function () {
                var payload = { context: getContext() };
                if(volumeCommitmentEntity != undefined)
                {
                    payload.volumeCommitmentItems = volumeCommitmentEntity.Settings.Items ;
                }
                VRUIUtilsService.callDirectiveLoad(volumeCommitmenetItemsAPI, payload, volumeCommitmenetItemsLoadDeferred);
            });

            return volumeCommitmenetItemsLoadDeferred.promise;
        }
        function insertVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_VolumeCommitmentAPIService.AddVolumeCommitment(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VolumeCommitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentAdded != undefined)
                        $scope.onVolumeCommitmentAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateVolumeCommitment() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_VolumeCommitmentAPIService.UpdateVolumeCommitment(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VolumeCommitment', response, 'Description')) {
                    if ($scope.onVolumeCommitmentUpdated != undefined)
                        $scope.onVolumeCommitmentUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildVolumeCommitmentObjFromScope() {
            var obj = {
                VolumeCommitmentId: volumeCommitmentId,
                Settings: {
                    Description: $scope.scopeModel.description,
                    Type:$scope.scopeModel.selectedVolumeCommitmentType.value,
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    Active: $scope.scopeModel.active,
                    Items: volumeCommitmenetItemsAPI.getData()
                }
            };
            return obj;
        }

        function getContext()
        {
            var context = {
                getZoneSelector:function()
                {
                    if ($scope.scopeModel.selectedVolumeCommitmentType != undefined)
                        return $scope.scopeModel.selectedVolumeCommitmentType.zoneSelector;
                },
                getZoneSelectorPayload:function(item)
                {
                    if($scope.scopeModel.selectedVolumeCommitmentType !=undefined)
                    {
                        var payload;
                        switch($scope.scopeModel.selectedVolumeCommitmentType.value)
                        {
                            case WhS_BE_VolumeCommitmentTypeEnum.Buy.value:

                                payload = {
                                    supplierId: carrierAccountSelectorAPI.getSelectedIds(),
                                }
                                if (item != undefined)
                                {
                                    payload.selectedIds = item.ZoneIds;
                                }
                                break;
                            case WhS_BE_VolumeCommitmentTypeEnum.Sell.value:
                                var carrierAccount = carrierAccountSelectorAPI.getSelectedValues();
                                payload = {
                                    sellingNumberPlanId: carrierAccount != undefined ? carrierAccount.SellingNumberPlanId : undefined
                                }
                                if (item != undefined) {
                                    payload.selectedIds = item.ZoneIds;
                                }
                                break;
                        }
                        return payload;
                    }
                }
            };
            return context;
        }
    }

    appControllers.controller('WhS_BE_VolumeCommitmentEditorController', VolumeCommitmentEditorController);

})(appControllers);