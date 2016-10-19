(function (appControllers) {

    'use strict';

    SwapDealEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealSellingService', 'WhS_BE_DealBuyingService', 'WhS_Deal_DealContractTypeEnum', 'WhS_Deal_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_DealService', 'WhS_BE_DealAnalysisService'];

    function SwapDealEditorController($scope, WhS_BE_SwapDealAPIService, WhS_BE_SwapDealSellingService, WhS_BE_SwapDealBuyingService, WhS_Deal_DealContractTypeEnum, WhS_Deal_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_SwapDealService, WhS_BE_SwapDealAnalysisService) {
        var isEditMode;

        var dealId;
        var dealEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dealSellingAPI;
        var dealSellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dealBuyingAPI;
        var dealBuyingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                if (carrierAccountInfo != undefined && carrierAccountInfo.SellingNumberPlanId) {

                    var setLoader = function (value) { $scope.isLoadingSelector = value };
                    var payload = {
                        sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                    }
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payload.SellingParts = dealEntity.Settings.SellingParts;
                    

                    var payloadBuying = {
                        supplierId: carrierAccountInfo.CarrierAccountId
                    }
                    if (dealEntity != undefined && dealEntity.Settings != undefined)
                        payloadBuying.BuyingParts = dealEntity.Settings.BuyingParts;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dealSellingAPI, payload, setLoader);
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dealBuyingAPI, payloadBuying, setLoader);
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

            $scope.scopeModel.onSwapDealSellingDirectiveReady = function (api) {
                dealSellingAPI = api;
                dealSellingReadyPromiseDeferred.resolve();
            }
            
            $scope.scopeModel.onSwapDealBuyingDirectiveReady = function (api) {
                dealBuyingAPI = api;
                dealBuyingReadyPromiseDeferred.resolve();
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
            return WhS_BE_SwapDealAPIService.GetSwapDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwapDealSelling, loadSwapDealBuying]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadSwapDealSelling() {

            var loadSwapDealSellingPromiseDeferred = UtilsService.createPromiseDeferred();


            if (carrierAccountSelectorAPI != undefined && carrierAccountSelectorAPI != null) {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined && carrierAccountInfo.SellingNumberPlanId) {
                    dealSellingReadyPromiseDeferred.promise
                        .then(function() {
                            var directivePayload = {
                                sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                            }
                            if (dealEntity != undefined && dealEntity.Settings != undefined)
                                directivePayload.SellingParts = dealEntity.Settings.SellingParts;
                            VRUIUtilsService.callDirectiveLoad(dealSellingAPI, directivePayload, loadSwapDealSellingPromiseDeferred);
                        });
                }
            }
            else {
                dealSellingReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            sellingNumberPlanId: null
                        }
                        if (dealEntity != undefined && dealEntity.Settings != undefined)
                            directivePayload.SellingParts = dealEntity.Settings.SellingParts;

                        VRUIUtilsService.callDirectiveLoad(dealSellingAPI, directivePayload, loadSwapDealSellingPromiseDeferred);
                    });
            }


            return loadSwapDealSellingPromiseDeferred.promise;
        }

        function loadSwapDealBuying() {

            var loadSwapDealBuyingPromiseDeferred = UtilsService.createPromiseDeferred();


            if (carrierAccountSelectorAPI != undefined && carrierAccountSelectorAPI != null) {
                var carrierAccountInfo = carrierAccountSelectorAPI.getSelectedValues();
                if (carrierAccountInfo != undefined && carrierAccountInfo.SellingNumberPlanId) {
                    dealBuyingReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                sellingNumberPlanId: carrierAccountInfo.SellingNumberPlanId
                            }
                            if (dealEntity != undefined && dealEntity.Settings != undefined)
                                directivePayload.BuyingParts = dealEntity.Settings.BuyingParts;

                            VRUIUtilsService.callDirectiveLoad(dealBuyingAPI, directivePayload, loadSwapDealBuyingPromiseDeferred);
                        });
                }
            }
            else {
                dealBuyingReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            sellingNumberPlanId: null
                        }
                        if (dealEntity != undefined && dealEntity.Settings != undefined)
                            directivePayload.BuyingParts = dealEntity.Settings.BuyingParts;

                        VRUIUtilsService.callDirectiveLoad(dealBuyingAPI, directivePayload, loadSwapDealBuyingPromiseDeferred);
                    });
            }


            return loadSwapDealBuyingPromiseDeferred.promise;
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
            $scope.scopeModel.sellingParts = dealEntity.Settings.SellingParts;
            $scope.scopeModel.buyingParts = dealEntity.Settings.BuyingParts;
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
            return WhS_BE_SwapDealAPIService.AddSwapDeal(buildSwapDealObjFromScope()).then(function (response) {
                WhS_BE_SwapDealService.addNeedsFields(response.InsertedObject.Entity);

                if (VRNotificationService.notifyOnItemAdded('SwapDeal', response, 'Description')) {
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
            return WhS_BE_SwapDealAPIService.UpdateSwapDeal(buildSwapDealObjFromScope()).then(function (response) {


                WhS_BE_SwapDealService.addNeedsFields(response.UpdatedObject.Entity);

                if (VRNotificationService.notifyOnItemUpdated('SwapDeal', response, 'Description')) {
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
                SwapDealId: dealId,
                Settings: {
                    Description: $scope.scopeModel.description,
                    CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                    BeginDate: $scope.scopeModel.beginDate,
                    EndDate: $scope.scopeModel.endDate,
                    GracePeriod: $scope.scopeModel.gracePeriod,
                    ContractType: $scope.scopeModel.selectedContractType.value,
                    AgreementType: $scope.scopeModel.selectedAgreementType.value,
                    SellingParts: dealSellingAPI.getData().sellingParts,
                    BuyingParts: dealBuyingAPI.getData().buyingParts,
                    SellingAmount: dealSellingAPI.getData().sellingAmount,
                    SellingDuration: dealSellingAPI.getData().sellingDuration,
                    BuyingAmount: dealBuyingAPI.getData().buyingAmount,
                    BuyingDuration: dealBuyingAPI.getData().buyingDuration,
                    Active: $scope.scopeModel.active
                }
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealEditorController', SwapDealEditorController);

})(appControllers);