(function (appControllers) {

    'use strict';

    DealSellingEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DealSellingEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var dealSellingEntity;
        var sellingNumberPlanId;

        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                dealSellingEntity = parameters.dealSelling;
            }
            
            isEditMode = (dealSellingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDealSelling() : insertDealSelling();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                loadAllControls();
                loadStaticData();
            }
            else {
                loadAllControls();
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSaleZoneSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadSaleZoneSection() {
            var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

            saleZoneReadyPromiseDeferred.promise.then(function () {
                
                var payload = {
                    sellingNumberPlanId: sellingNumberPlanId,
                    selectedIds: dealSellingEntity != undefined ? dealSellingEntity.SaleZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (dealSellingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealSellingEntity.Name, 'Selling Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Selling Part');
        }

        function loadStaticData() {
            if (dealSellingEntity == undefined)
                return;
            $scope.scopeModel.name = dealSellingEntity.Name;
            $scope.scopeModel.volume = dealSellingEntity.Volume;
            $scope.scopeModel.rate = dealSellingEntity.Rate;
            $scope.scopeModel.maxBuyingRate = dealSellingEntity.MaxBuyingRate;
            $scope.scopeModel.substituteRate = dealSellingEntity.SubstituteRate;
            $scope.scopeModel.extraVolumeRate = dealSellingEntity.ExtraVolumeRate;
            $scope.scopeModel.asr = dealSellingEntity.ASR;
            $scope.scopeModel.ner = dealSellingEntity.NER;
            $scope.scopeModel.acd = dealSellingEntity.ACD;
        }

        function insertDealSelling() {
            $scope.scopeModel.isLoading = true;
            
            var dealSellingObject = buildDealSellingObjFromScope();
            if ($scope.onDealSellingAdded != undefined)
                $scope.onDealSellingAdded(dealSellingObject);
            $scope.modalContext.closeModal();
        }

        function updateDealSelling() {
            var dealSellingObject = buildDealSellingObjFromScope();
            if ($scope.onDealSellingUpdated != undefined)
                $scope.onDealSellingUpdated(dealSellingObject);
            $scope.modalContext.closeModal();
        }

        function buildDealSellingObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                MaxBuyingRate: $scope.scopeModel.maxBuyingRate,
                SubstituteRate: $scope.scopeModel.substituteRate,
                ExtraVolumeRate: $scope.scopeModel.extraVolumeRate,
                ASR: $scope.scopeModel.asr,
                NER: $scope.scopeModel.ner,
                ACD: $scope.scopeModel.acd,
                Amount: $scope.scopeModel.volume * $scope.scopeModel.rate
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_DealSellingEditorController', DealSellingEditorController);

})(appControllers);