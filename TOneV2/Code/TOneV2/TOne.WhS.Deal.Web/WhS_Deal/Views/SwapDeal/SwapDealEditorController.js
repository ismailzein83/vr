(function (appControllers) {

    'use strict';

    SwapDealEditorController.$inject = ['$scope', 'WhS_Deal_DealAPIService', 'WhS_Deal_DealContractTypeEnum', 'WhS_Deal_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_DealService', 'WhS_BE_DealAnalysisService'];

    function SwapDealEditorController($scope, WhS_Deal_DealAPIService, WhS_Deal_DealContractTypeEnum, WhS_Deal_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SwapDealService, WhS_BE_SwapDealAnalysisService) {
        var isEditMode;

        var dealId;
        var dealEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

                    var setLoader = function (value) { $scope.isLoadingSelector = value };
                    var payload = {
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                    }
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payload.InboundParts = dealEntity.Settings.InboundParts;
                    

                    var payloadOutbound = {
                        supplierId: carrierAccountInfo.CarrierAccountId
                    }
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payloadOutbound.OutboundParts = dealEntity.Settings.OutboundParts;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dealInboundAPI, payload, setLoader);
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dealOutboundAPI, payloadOutbound, setLoader);
                }
            }
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
            }
            
            $scope.scopeModel.onSwapDealOutboundDirectiveReady = function (api) {
                dealOutboundAPI = api;
                dealOutboundReadyPromiseDeferred.resolve();
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getSwapDeal().then(function () {

                    loadCarrierAccountSelector().then(function () {
                        loadAllControls().finally(function () {
                            //dealEntity = undefined;
                        });
                    });

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
                loadCarrierAccountSelector();
            }
        }

        function getSwapDeal() {
            return WhS_Deal_DealAPIService.GetSwapDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwapDealInbound, loadSwapDealOutbound]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadSwapDealInbound() {

            var loadSwapDealInboundPromiseDeferred = UtilsService.createPromiseDeferred();


            if (carrierAccountSelectorAPI != undefined && carrierAccountSelectorAPI != null) {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined && carrierAccountInfo.SellingNumberPlanId) {
                    dealInboundReadyPromiseDeferred.promise
                        .then(function() {
                            var directivePayload = {
                                sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                            }
                            if (dealEntity != undefined && dealEntity.Settings != undefined)
                                directivePayload.InboundParts = dealEntity.Settings.InboundParts;
                            VRUIUtilsService.callDirectiveLoad(dealInboundAPI, directivePayload, loadSwapDealInboundPromiseDeferred);
                        });
                }
            }
            else {
                dealInboundReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            sellingNumberPlanId: null
                        }
                        if (dealEntity != undefined && dealEntity.Settings != undefined)
                            directivePayload.InboundParts = dealEntity.Settings.InboundParts;

                        VRUIUtilsService.callDirectiveLoad(dealInboundAPI, directivePayload, loadSwapDealInboundPromiseDeferred);
                    });
            }


            return loadSwapDealInboundPromiseDeferred.promise;
        }

        function loadSwapDealOutbound() {

            var loadSwapDealOutboundPromiseDeferred = UtilsService.createPromiseDeferred();


            if (carrierAccountSelectorAPI != undefined && carrierAccountSelectorAPI != null) {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined && carrierAccountInfo.SupplierId) {
                    dealOutboundReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                supplierId: carrierAccountInfo.SupplierId
                            }
                            if (dealEntity != undefined && dealEntity.Settings != undefined)
                                directivePayload.OutboundParts = dealEntity.Settings.OutboundParts;

                            VRUIUtilsService.callDirectiveLoad(dealOutboundAPI, directivePayload, loadSwapDealOutboundPromiseDeferred);
                        });
                }
            }
            else {
                dealOutboundReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            supplierId: null
                        }
                        if (dealEntity != undefined && dealEntity.Settings != undefined)
                            directivePayload.OutboundParts = dealEntity.Settings.OutboundParts;

                        VRUIUtilsService.callDirectiveLoad(dealOutboundAPI, directivePayload, loadSwapDealOutboundPromiseDeferred);
                    });
            }


            return loadSwapDealOutboundPromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (dealEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealEntity.Settings.Description, 'SwapDeal');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('SwapDeal');
        }

        function loadStaticData() {
            if (dealEntity == undefined)
                return;
            $scope.scopeModel.description = dealEntity.Settings.Description;
            $scope.scopeModel.gracePeriod = dealEntity.Settings.GracePeriod;
            $scope.scopeModel.selectedContractType = UtilsService.getItemByVal($scope.scopeModel.contractTypes, dealEntity.Settings.ContractType, 'value');
            $scope.scopeModel.selectedAgreementType = UtilsService.getItemByVal($scope.scopeModel.agreementTypes, dealEntity.Settings.AgreementType, 'value');
            $scope.scopeModel.beginDate = dealEntity.Settings.BeginDate;
            $scope.scopeModel.endDate = dealEntity.Settings.EndDate;
            $scope.scopeModel.sellingParts = dealEntity.Settings.InboundParts;
            $scope.scopeModel.buyingParts = dealEntity.Settings.OutboundParts;
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
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    ContractType: $scope.scopeModel.selectedContractType.value,
                    AgreementType: $scope.scopeModel.selectedAgreementType.value,
                    Inbounds: dealInboundAPI.getData(),
                    Outbounds: dealOutboundAPI.getData()
                }
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealEditorController', SwapDealEditorController);

})(appControllers);