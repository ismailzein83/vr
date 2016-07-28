(function (appControllers) {

    'use strict';

    DealAnalysisEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealAnalysisService', 'WhS_BE_DealBuyingService', 'WhS_BE_DealAnalysisTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_DealService'];

    function DealAnalysisEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealAnalysisService, WhS_BE_DealBuyingService, WhS_BE_DealAnalysisTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, WhS_BE_DealService) {
        var isEditMode;

        var dealId;
        var dealEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        //var dealSellingAPI;
        //var dealSellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        //var dealBuyingAPI;
        //var dealBuyingReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.DurationInMonths = 0;
            $scope.scopeModel.TotalCommitmentCost = 0;
            $scope.scopeModel.TotalCommitmentRevenue = 0;
            $scope.scopeModel.TotalSaving = 0;
            $scope.scopeModel.TotalCustomerProfit = 0;
            $scope.scopeModel.TotalProfitMonth = 0;
            $scope.scopeModel.TotalProfitDay = 0;
            $scope.scopeModel.TotalCommitmentProfit = 0;
            $scope.scopeModel.Margins = 0;
            $scope.scopeModel.Exposure = 0;

            $scope.scopeModel.outboundTraffics = [];
            $scope.scopeModel.inboundTraffics = [];

            $scope.scopeModel.analysisTypes = UtilsService.getArrayEnum(WhS_BE_DealAnalysisTypeEnum);

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.EvalDurationInMonths = function (api) {
                if ($scope.scopeModel.fromDate != undefined && $scope.scopeModel.toDate != undefined) {
                    var days = ($scope.scopeModel.toDate - $scope.scopeModel.fromDate) / (1000 * 60 * 60 * 24) / 30;
                    $scope.scopeModel.DurationInMonths = UtilsService.round(days,1)
                }
                else 
                    $scope.scopeModel.DurationInMonths = 0
            };

            $scope.scopeModel.addOutboundTraffic = function () {
                var supplierId = carrierAccountSelectorAPI.getSelectedValues() != undefined ? carrierAccountSelectorAPI.getSelectedValues().CarrierAccountId : undefined;
                var onOutboundAdded = function (addedOutbound) {
                    var obj = {
                        Name: addedOutbound.Name,
                        SupplierZoneIds: addedOutbound.SupplierZoneIds,
                        CommitedVolume: addedOutbound.CommitedVolume,
                        DailyVolume: (addedOutbound.CommitedVolume/1.5)/30,
                        Rate: addedOutbound.Rate,
                        CurrentCost:addedOutbound.CurrentCost,
                        CostSavingFromRate: addedOutbound.CurrentCost - addedOutbound.Rate,
                        TotalDealSaving: (addedOutbound.CurrentCost - addedOutbound.Rate) * addedOutbound.CommitedVolume,
                        RevenuePerDeal: addedOutbound.CommitedVolume * addedOutbound.Rate
                    }
                    $scope.scopeModel.outboundTraffics.push(obj);
                    evalTotalResultAnalysis();
                };
                WhS_BE_DealAnalysisService.addOutbound(onOutboundAdded, supplierId);
            };

            $scope.scopeModel.addInboundTraffic = function () {
                var sellingNumberPlanId = carrierAccountSelectorAPI.getSelectedValues() != undefined ? carrierAccountSelectorAPI.getSelectedValues().SellingNumberPlanId : undefined;
                   
                var onInboundAdded = function (addedInbound) {
                    var obj = {
                        Name: addedInbound.Name,
                        SaleZoneIds: addedInbound.SaleZoneIds,
                        CommitedVolume: addedInbound.CommitedVolume,
                        DailyVolume: (addedInbound.CommitedVolume / 1.5) / 30,
                        Rate: addedInbound.Rate,
                        CurrentCost: addedInbound.CurrentCost,
                        ProfitFromRate: addedInbound.Rate - addedInbound.CurrentCost,
                        DealProfit: (addedInbound.Rate - addedInbound.CurrentCost) * addedInbound.CommitedVolume,
                        Revenue: addedInbound.CommitedVolume * addedInbound.Rate
                    }
                    $scope.scopeModel.inboundTraffics.push(obj);
                    evalTotalResultAnalysis();
                };
                WhS_BE_DealAnalysisService.addInbound(onInboundAdded, sellingNumberPlanId);
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
                }
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDeal() : insertDeal();
            };

            $scope.scopeModel.hasSavePermission = function () {
                return (isEditMode) ?
                    WhS_BE_DealAPIService.HasEditDealPermission() :
                    WhS_BE_DealAPIService.HasAddDealPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getDeal().then(function () {

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

        function getDeal() {
            return WhS_BE_DealAPIService.GetDeal(dealId).then(function (response) {
                dealEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadDealSelling() {

            var loadDealSellingPromiseDeferred = UtilsService.createPromiseDeferred();


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
                            VRUIUtilsService.callDirectiveLoad(dealSellingAPI, directivePayload, loadDealSellingPromiseDeferred);
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

                        VRUIUtilsService.callDirectiveLoad(dealSellingAPI, directivePayload, loadDealSellingPromiseDeferred);
                    });
            }


            return loadDealSellingPromiseDeferred.promise;
        }

        function loadDealBuying() {

            var loadDealBuyingPromiseDeferred = UtilsService.createPromiseDeferred();


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

                            VRUIUtilsService.callDirectiveLoad(dealBuyingAPI, directivePayload, loadDealBuyingPromiseDeferred);
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

                        VRUIUtilsService.callDirectiveLoad(dealBuyingAPI, directivePayload, loadDealBuyingPromiseDeferred);
                    });
            }


            return loadDealBuyingPromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (dealEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealEntity.Settings.Description, 'Deal Analysis');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Deal Analysis');
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
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }



        function insertDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_DealAPIService.AddDeal(buildDealObjFromScope()).then(function (response) {
                WhS_BE_DealService.addNeedsFields(response.InsertedObject.Entity);

                if (VRNotificationService.notifyOnItemAdded('Deal', response, 'Description')) {
                    if ($scope.onDealAdded != undefined)
                        $scope.onDealAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateDeal() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_DealAPIService.UpdateDeal(buildDealObjFromScope()).then(function (response) {

                WhS_BE_DealService.addNeedsFields(response.UpdatedObject.Entity);

                if (VRNotificationService.notifyOnItemUpdated('Deal', response, 'Description')) {
                    if ($scope.onDealUpdated != undefined)
                        $scope.onDealUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildDealObjFromScope() {
            var obj = {
                DealId: dealId,
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
        function evalTotalResultAnalysis() {
            var totalCommitmentCost = 0;
            var totalSaving = 0;

            var totalCommitmentRevenue = 0;
            var totalCustomerProfit = 0;

            var totalProfitMonth = 0;
            var totalProfitDay = 0;
            var totalCommitmentProfit = 0;
            var margins = 0;
            var exposure = 0;


            if ($scope.scopeModel.outboundTraffics.length > 0) {
                for (var i = 0 ; i < $scope.scopeModel.outboundTraffics.length ; i++) {
                    totalCommitmentCost += $scope.scopeModel.outboundTraffics[i].RevenuePerDeal;
                    totalSaving += $scope.scopeModel.outboundTraffics[i].TotalDealSaving;
                }
                   
            }
            $scope.scopeModel.TotalCommitmentCost = UtilsService.round(totalCommitmentCost,2);
            $scope.scopeModel.TotalSaving = UtilsService.round(totalSaving,2);

            if ($scope.scopeModel.inboundTraffics.length > 0) {
                for (var i = 0 ; i < $scope.scopeModel.inboundTraffics.length ; i++) {
                    totalCustomerProfit += $scope.scopeModel.inboundTraffics[i].DealProfit;
                    totalCommitmentRevenue += $scope.scopeModel.inboundTraffics[i].Revenue;
                }
            }

            $scope.scopeModel.TotalCustomerProfit = UtilsService.round(totalCustomerProfit,2);

            $scope.scopeModel.TotalCommitmentRevenue = UtilsService.round(totalCommitmentRevenue,2) ;

            $scope.scopeModel.TotalCommitmentProfit = UtilsService.round(totalSaving + totalCustomerProfit,2);

            $scope.scopeModel.TotalProfitMonth = UtilsService.round ( ( $scope.scopeModel.DurationInMonths > 0 ? $scope.scopeModel.TotalCommitmentProfit/  $scope.scopeModel.DurationInMonths : 0) ,2) ;

            $scope.scopeModel.TotalProfitDay = UtilsService.round ($scope.scopeModel.TotalProfitMonth /  30 , 2) ;

            $scope.scopeModel.Margins = UtilsService.round ( ($scope.scopeModel.TotalCommitmentProfit /  $scope.scopeModel.TotalCommitmentRevenue),2 );

            $scope.scopeModel.Exposure = UtilsService.round ( totalCommitmentRevenue -  totalCommitmentCost , 2);


        }

    }

    appControllers.controller('WhS_BE_DealAnalysisEditorController', DealAnalysisEditorController);

})(appControllers);