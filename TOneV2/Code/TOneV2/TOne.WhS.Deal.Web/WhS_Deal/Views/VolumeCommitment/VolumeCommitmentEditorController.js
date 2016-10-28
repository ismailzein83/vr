(function (appControllers) {

    'use strict';

    VolumeCommitmentEditorController.$inject = ['$scope', 'WhS_Deal_DealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Deal_VolumeCommitmentService', 'WhS_Deal_VolumeCommitmentTypeEnum', 'VRValidationService'];

    function VolumeCommitmentEditorController($scope, WhS_Deal_DealAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_Deal_VolumeCommitmentService, WhS_Deal_VolumeCommitmentTypeEnum, VRValidationService) {
        var isEditMode;

        var dealId;
        var volumeCommitmentEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred;

        var volumeCommitmenetItemsAPI;
        var volumeCommitmenetItemsReadyDeferred = UtilsService.createPromiseDeferred();;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                dealId = parameters.dealId;

            isEditMode = (dealId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.volumeCommitmentTypes = UtilsService.getArrayEnum(WhS_Deal_VolumeCommitmentTypeEnum);
            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountSelectorAPI, payload, setLoader, carrierAccountSelectorReadyDeferred);
            };

            $scope.scopeModel.onVolumeCommitmenetItemsReady = function(api)
            {
                volumeCommitmenetItemsAPI = api;
                volumeCommitmenetItemsReadyDeferred.resolve();
            }

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateVolumeCommitment() : insertVolumeCommitment();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.validateDatesRange = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getVolumeCommitment().then(function () {
                        loadAllControls().finally(function () {
                            volumeCommitmentEntity = undefined;
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
            return WhS_Deal_DealAPIService.GetDeal(dealId).then(function (response) {
                volumeCommitmentEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGracePeriod, loadCarrierAccountSelector, loadVolumeCommitmenetItems]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                if (volumeCommitmentEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(volumeCommitmentEntity.Name, 'Volume Commitment');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Volume Commitment');
        }

        function loadStaticData() {
            if (volumeCommitmentEntity == undefined)
                return;
            $scope.scopeModel.description = volumeCommitmentEntity.Name;
            $scope.scopeModel.beginDate = volumeCommitmentEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = volumeCommitmentEntity.Settings.EndDate;
            $scope.scopeModel.active = volumeCommitmentEntity.Settings.Active;
            $scope.scopeModel.selectedVolumeCommitmentType = UtilsService.getItemByVal($scope.scopeModel.volumeCommitmentTypes, volumeCommitmentEntity.Settings.DealType, "value");
        }
        function loadGracePeriod() {
            if (volumeCommitmentEntity != undefined && volumeCommitmentEntity.Settings.GracePeriod != undefined) {
                $scope.scopeModel.gracePeriod = volumeCommitmentEntity.Settings.GracePeriod;
                return;
            }
            return WhS_Deal_DealAPIService.GetSwapDealSettingData().then(function (response) {
                $scope.scopeModel.gracePeriod = response.GracePeriod;
            });
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
                var payload = { context: getContext() }
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
            return WhS_Deal_DealAPIService.AddDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Volume Commitment', response, 'Description')) {
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
            return WhS_Deal_DealAPIService.UpdateDeal(buildVolumeCommitmentObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Volume Commitment', response, 'Description')) {
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
                DealId: dealId,
                Name: $scope.scopeModel.description,
                Settings: {
                    $type: "TOne.WhS.Deal.Entities.VolCommitmentDealSettings, TOne.WhS.Deal.Entities",
                    DealType:$scope.scopeModel.selectedVolumeCommitmentType.value,
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
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
                            case WhS_Deal_VolumeCommitmentTypeEnum.Buy.value: 

                                payload = {
                                    supplierId: carrierAccountSelectorAPI.getSelectedIds(),
                                }
                                if (item != undefined)
                                {
                                    payload.selectedIds = item.ZoneIds;
                                }
                                break;
                            case WhS_Deal_VolumeCommitmentTypeEnum.Sell.value:
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
                },
                getZoneIdAttName:function()
                {
                    if($scope.scopeModel.selectedVolumeCommitmentType !=undefined)
                    {
                        var idName;
                        switch($scope.scopeModel.selectedVolumeCommitmentType.value)
                        {
                            case WhS_Deal_VolumeCommitmentTypeEnum.Buy.value: 
                                idName = 'SupplierZoneId'
                                break;
                            case WhS_Deal_VolumeCommitmentTypeEnum.Sell.value:
                                idName = 'SaleZoneId'
                                break;
                        }
                        return idName;
                    }
                }


            };
            return context;
        }
    }

    appControllers.controller('WhS_Deal_VolumeCommitmentEditorController', VolumeCommitmentEditorController);

})(appControllers);