(function (app) {

    'use strict';

    SwapDealEditorController.$inject = ['$scope', 'WhS_Deal_DealAPIService', 'WhS_Deal_DealContractTypeEnum', 'WhS_Deal_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_DealService', 'WhS_BE_DealAnalysisService', 'VRValidationService'];

    function SwapDealEditorController($scope, WhS_Deal_DealAPIService, WhS_Deal_DealContractTypeEnum, WhS_Deal_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SwapDealService, WhS_BE_SwapDealAnalysisService, VRValidationService) {
        var isEditMode;

        var dealId;
        var dealEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dealInboundAPI;
        var dealInboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealOutboundAPI;
        var dealOutboundReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectedPromise;

        loadParameters();
        defineScope();
        load();
        var oldselectedCarrier;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealId = parameters.dealId;
            }
            isEditMode = (dealId != undefined);
        }

        function defineScope() {
            
            $scope.scopeModel = {};
            $scope.scopeModel.gracePeriod = 7;
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
                    }
                    var payloadOutbound = {
                        supplierId: carrierAccountInfo.CarrierAccountId
                    }
                    if (carrierAccountSelectedPromise != undefined){
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
                    else if (oldselectedCarrier==undefined) {
                        oldselectedCarrier = carrierAccountInfo;
                        dealInboundAPI.load(payload);
                        dealOutboundAPI.load(payloadOutbound);
                    }
                }
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwapDeal() : insertSwapDeal();
            };

            $scope.scopeModel.analyse = function () {
                WhS_BE_SwapDealAnalysisService.openSwapDealAnalysis(buildSwapDealObjFromScope())
            };

      
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onSwapDealInboundDirectiveReady = function (api) {
                dealInboundAPI = api;
                dealInboundReadyPromiseDeferred.resolve();
            };
            
            $scope.scopeModel.onSwapDealOutboundDirectiveReady = function (api) {
                dealOutboundAPI = api;
                dealOutboundReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.validateDatesRange = function () {               
                 return VRValidationService.validateTimeRange($scope.scopeModel.beginDate, $scope.scopeModel.endDate);
            };

            $scope.scopeModel.validateSwapDealInbounds = function () {
                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues()
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if(dealInboundAPI != undefined && dealInboundAPI.getData().length == 0)
                    return 'Please, one record must be added at least.'
                return null;
            };

            $scope.scopeModel.validateSwapDealOutbounds = function () {
                var selectedcarrier = carrierAccountSelectorAPI.getSelectedValues()
                if (selectedcarrier == undefined)
                    return 'Please select a Carrier Account.';
                if(dealOutboundAPI!=undefined && dealOutboundAPI.getData().length == 0)
                    return 'Please,one record must be added at least.'
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
            else {
                loadAllControls();
            }
        }

        function getSwapDeal() {
            return WhS_Deal_DealAPIService.GetDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCarrierBoundsSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadCarrierBoundsSection() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCarrierAccountPromiseDeferred.promise);

            var payload ;
            if(dealEntity != undefined && dealEntity.Settings!=undefined) {
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
                        Inbounds: dealEntity.Settings.Inbounds
                    }
                  
                    var payloadOutbound = {
                        supplierId: carrierAccountInfo.CarrierAccountId,
                        Outbounds: dealEntity.Settings.Outbounds
                    }
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payloadOutbound.Outbounds = dealEntity.Settings.Outbounds;
                    VRUIUtilsService.callDirectiveLoad(dealInboundAPI, payload, loadSwapDealInboundPromiseDeferred);
                    VRUIUtilsService.callDirectiveLoad(dealOutboundAPI, payloadOutbound, loadSwapDealOutboundPromiseDeferred);
                    carrierAccountSelectedPromise = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
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
            $scope.scopeModel.description = dealEntity.Name;
            //$scope.scopeModel.gracePeriod = dealEntity.Settings.GracePeriod;
            $scope.scopeModel.selectedContractType = UtilsService.getItemByVal($scope.scopeModel.contractTypes, dealEntity.Settings.DealContract, 'value');
            $scope.scopeModel.selectedAgreementType = UtilsService.getItemByVal($scope.scopeModel.agreementTypes, dealEntity.Settings.DealType, 'value');
            $scope.scopeModel.beginDate = dealEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = dealEntity.Settings.EndDate;
            $scope.scopeModel.active = dealEntity.Settings.Active;
        }

        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                var payload = (dealEntity != undefined) ? { selectedIds: dealEntity.Settings.CarrierAccountId } : undefined;
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
                $scope.scopeModel.isLoading = false;
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }

        function insertSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_DealAPIService.AddDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealAdded != undefined)
                        $scope.onSwapDealAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateSwapDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_Deal_DealAPIService.UpdateDeal(buildSwapDealObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Swap Deal', response, 'Description')) {
                    if ($scope.onSwapDealUpdated != undefined)
                        $scope.onSwapDealUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwapDealObjFromScope() {            
            var obj = {
                DealId: dealId,
                Name: $scope.scopeModel.description,
                Settings: {
                    $type: "TOne.WhS.Deal.Entities.SwapDealSettings, TOne.WhS.Deal.Entities",
                    CarrierAccountId:carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    DealContract: $scope.scopeModel.selectedContractType.value,
                    DealType: $scope.scopeModel.selectedAgreementType.value,
                    Inbounds: dealInboundAPI.getData(),
                    Outbounds: dealOutboundAPI.getData()
                }
            };
            return obj;
        }
    }

    app.controller('WhS_Deal_SwapDealEditorController', SwapDealEditorController);

})(app);