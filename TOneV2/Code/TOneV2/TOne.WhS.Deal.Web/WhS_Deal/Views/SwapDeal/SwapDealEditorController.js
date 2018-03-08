﻿(function (app) {

    'use strict';

    SwapDealEditorController.$inject = ['$scope', 'WhS_Deal_SwapDealAPIService', 'WhS_Deal_DealContractTypeEnum', 'WhS_Deal_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_Deal_SwapDealService', 'WhS_Deal_SwapDealAnalysisService', 'VRValidationService'];

    function SwapDealEditorController($scope, WhS_Deal_SwapDealAPIService, WhS_Deal_DealContractTypeEnum, WhS_Deal_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SwapDealService, WhS_Deal_SwapDealService, VRValidationService) {
        var isEditMode;

        var dealId;
        var dealEntity;
        var carrierAccountSelectedPromise;
        var context;
        var isViewHistoryMode;
        var oldselectedCarrier;
        var isReadOnly;
        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealInboundAPI;
        var dealInboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealOutboundAPI;
        var dealOutboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealId = parameters.dealId;
                context = parameters.context;

                isReadOnly = parameters.isReadOnly;
                isViewHistoryMode = context != undefined && context.historyId != undefined;
            }
            if (isReadOnly == true)
                UtilsService.setContextReadOnly($scope);
            isEditMode = (dealId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disabelType = isEditMode;
            $scope.scopeModel.contractTypes = UtilsService.getArrayEnum(WhS_Deal_DealContractTypeEnum);
            $scope.scopeModel.agreementTypes = UtilsService.getArrayEnum(WhS_Deal_DealAgreementTypeEnum);

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCarrierAccountSelectionChanged = function () {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined) {
                    var payload = {
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                    };
                    var payloadOutbound = {
                        supplierId: carrierAccountInfo.CarrierAccountId
                    };
                    if (carrierAccountSelectedPromise != undefined) {
                        carrierAccountSelectedPromise.resolve();
                        oldselectedCarrier = $scope.scopeModel.carrierAcount;
                    }
                    else if (oldselectedCarrier != undefined && oldselectedCarrier.CarrierAccountId != carrierAccountInfo.CarrierAccountId) {
                        if (dealInboundAPI.hasData() || dealOutboundAPI.hasData()) {
                            VRNotificationService.showConfirmation('Data will be lost,Are you sure you want to continue?').then(function (response) {
                                if (response) {
                                    dealInboundAPI.load(payload);
                                    dealOutboundAPI.load(payloadOutbound);
                                    oldselectedCarrier = carrierAccountInfo;
                                }
                                else {
                                    $scope.scopeModel.carrierAcount = oldselectedCarrier;
                                }
                            });
                        }
                        else {
                            dealInboundAPI.load(payload);
                            dealOutboundAPI.load(payloadOutbound);
                        }
                    }
                    else if (oldselectedCarrier == undefined) {
                        oldselectedCarrier = carrierAccountInfo;
                        dealInboundAPI.load(payload);
                        dealOutboundAPI.load(payloadOutbound);
                    }
                }
            };

            $scope.scopeModel.onCurrencySelectReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.analyse = function () {
                WhS_BE_SwapDealAnalysisService.openSwapDealAnalysis(buildSwapDealObjFromScope())
            };

            $scope.scopeModel.onSwapDealInboundDirectiveReady = function (api) {
                dealInboundAPI = api;
                dealInboundReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSwapDealOutboundDirectiveReady = function (api) {
                dealOutboundAPI = api;
                dealOutboundReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.validateGracePeriod = function () {
                if (!isGraceValid())
                    return "Grace period should be less than difference between BED and EED";
                return null;
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDeal() : insertSwapDeal();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.validateDatesRange = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
            };

            $scope.scopeModel.validateSwapDealInbounds = function () {
                if (carrierAccountSelectorAPI == undefined)
                    return null;

                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues();
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if (dealInboundAPI != undefined && !dealInboundAPI.hasData())
                    return 'Please, one record must be added at least.';
                return null;
            };

            $scope.scopeModel.validateSwapDealOutbounds = function () {
                if (carrierAccountSelectorAPI == undefined)
                    return null;

                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues();
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if (dealOutboundAPI != undefined && !dealOutboundAPI.hasData())
                    return 'Please,one record must be added at least.';
                return null;
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getSwapDeal().then(function () {
                    loadAllControls().finally(function () {
                        dealEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getSwapDealHistory().then(function () {
                    loadAllControls().finally(function () {
                        dealEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                loadAllControls();
            }
        }

        function getSwapDealHistory() {
            return WhS_Deal_SwapDealAPIService.GetSwapDealHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                dealEntity = response;

            });
        }
        function getSwapDeal() {
            return WhS_Deal_SwapDealAPIService.GetDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCarrierBoundsSection, loadGraceperiod, loadCurrencySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function isGraceValid() {
            var beginDate = new Date($scope.scopeModel.beginDate);
            var endDate = new Date($scope.scopeModel.endDate);
            return ($scope.scopeModel.gracePeriod < UtilsService.diffDays(beginDate, endDate));
        }
        function setTitle() {
            if (isEditMode) {
                if (dealEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealEntity.Name, 'Swap Deal');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Swap Deal');
        }
        function loadStaticData() {
            if (dealEntity == undefined)
                return;
            $scope.scopeModel.isCommitmentAgreement = isCommitmentAgreement();
            //console.log(isCommitmentAgreement())
            //if (isCommitmentAgreement())
            //    UtilsService.setContextReadOnly($scope);
            $scope.scopeModel.description = dealEntity.Name;
            //$scope.scopeModel.gracePeriod = dealEntity.Settings.GracePeriod;
            $scope.scopeModel.selectedContractType = UtilsService.getItemByVal($scope.scopeModel.contractTypes, dealEntity.Settings.DealContract, 'value');
            $scope.scopeModel.selectedAgreementType = UtilsService.getItemByVal($scope.scopeModel.agreementTypes, dealEntity.Settings.DealType, 'value');
            $scope.scopeModel.beginDate = dealEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = dealEntity.Settings.EndDate;
            $scope.scopeModel.active = dealEntity.Settings.Active;
            $scope.scopeModel.difference = dealEntity.Settings.Difference;
        }
        function loadCarrierBoundsSection() {
            var promises = [];

            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadCarrierAccountPromiseDeferred.promise);

            var payload;
            if (dealEntity != undefined && dealEntity.Settings != undefined) {
                payload = { selectedIds: dealEntity.Settings.CarrierAccountId };
                carrierAccountSelectedPromise = UtilsService.createPromiseDeferred();

            }
            carrierAccountSelectorReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, loadCarrierAccountPromiseDeferred);
            });

            if (dealEntity != undefined && dealEntity.Settings != undefined) {

                var loadSwapDealInboundPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSwapDealInboundPromiseDeferred.promise);

                var loadSwapDealOutboundPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSwapDealOutboundPromiseDeferred.promise);

                UtilsService.waitMultiplePromises([dealInboundReadyPromiseDeferred.promise, dealOutboundReadyPromiseDeferred.promise, carrierAccountSelectedPromise.promise]).then(function () {
                    var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                    var payload = {
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId,
                        Inbounds: dealEntity.Settings.Inbounds,
                        lastInboundGroupNumber: dealEntity.Settings.LastInboundGroupNumber
                    };

                    var payloadOutbound = {
                        supplierId: carrierAccountInfo.CarrierAccountId,
                        Outbounds: dealEntity.Settings.Outbounds,
                        lastOutboundGroupNumber: dealEntity.Settings.LastOutboundGroupNumber
                    };
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payloadOutbound.Outbounds = dealEntity.Settings.Outbounds;
                    if ((isEditMode && dealEntity != undefined) || !isEditMode)
                        VRUIUtilsService.callDirectiveLoad(dealInboundAPI, payload, loadSwapDealInboundPromiseDeferred);
                    VRUIUtilsService.callDirectiveLoad(dealOutboundAPI, payloadOutbound, loadSwapDealOutboundPromiseDeferred);
                    carrierAccountSelectedPromise = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function loadGraceperiod() {
            if (dealEntity != undefined && dealEntity.Settings.GracePeriod != undefined) {
                $scope.scopeModel.gracePeriod = dealEntity.Settings.GracePeriod;
                return;
            }
            return WhS_Deal_SwapDealAPIService.GetSwapDealSettingData().then(function (response) {
                $scope.scopeModel.gracePeriod = response.GracePeriod;
            })

        }
        function isCommitmentAgreement() {
            if (dealEntity != undefined && dealEntity.Settings != undefined)
                return (dealEntity.Settings.DealType == WhS_Deal_DealAgreementTypeEnum.Commitment.value)
        }

        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyPayload;
            if (dealEntity != undefined && dealEntity.Settings != undefined && dealEntity.Settings.CurrencyId > 0) {
                currencyPayload = { selectedIds: dealEntity.Settings.CurrencyId };
            }
            else {
                currencyPayload = { selectSystemCurrency: true };
            }

            currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

            });

            return loadCurrencySelectorPromiseDeferred.promise;
        };

        function insertSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_SwapDealAPIService.AddDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealAdded != undefined)
                        $scope.onSwapDealAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
                else {
                    $scope.validationMessages = response.ValidationMessages;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_SwapDealAPIService.UpdateDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealUpdated != undefined)
                        $scope.onSwapDealUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
                else {
                    $scope.validationMessages = response.ValidationMessages;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwapDealObjFromScope() {
            var inboundData = dealInboundAPI.getData();
            var outboundData = dealOutboundAPI.getData();

            var obj = {
                DealId: dealId,
                Name: $scope.scopeModel.description,
                Settings: {
                    $type: "TOne.WhS.Deal.Entities.SwapDealSettings, TOne.WhS.Deal.Entities",
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    DealContract: $scope.scopeModel.selectedContractType.value,
                    DealType: $scope.scopeModel.selectedAgreementType.value,
                    Inbounds: inboundData != undefined ? inboundData.inbounds : undefined,
                    Outbounds: outboundData != undefined ? outboundData.outbounds : undefined,
                    LastInboundGroupNumber: inboundData != undefined ? inboundData.lastInboundGroupNumber : undefined,
                    LastOutboundGroupNumber: outboundData != undefined ? outboundData.lastOutboundGroupNumber : undefined,
                    Difference: $scope.scopeModel.difference,
                    CurrencyId: currencyDirectiveAPI.getSelectedIds()
                }
            };
            return obj;
        }
    }

    app.controller('WhS_Deal_SwapDealEditorController', SwapDealEditorController);

})(app);