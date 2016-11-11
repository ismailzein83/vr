(function (appControllers) {

    'use strict';

    DealBuyingEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DealBuyingEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var dealBuyingEntity;
        var supplierId;

        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                supplierId = parameters.supplierId;
                dealBuyingEntity = parameters.dealBuying;
            }


            isEditMode = (dealBuyingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDealBuying() : insertDealBuying();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
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

            supplierZoneReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    supplierId: supplierId,
                    selectedIds: dealBuyingEntity != undefined ? dealBuyingEntity.SupplierZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (dealBuyingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealBuyingEntity.Name, 'Buying Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Buying Part');
        }

        function loadStaticData() {
            if (dealBuyingEntity == undefined)
                return;
            $scope.scopeModel.name = dealBuyingEntity.Name;
            $scope.scopeModel.volume = dealBuyingEntity.Volume;
            $scope.scopeModel.rate = dealBuyingEntity.Rate;
            $scope.scopeModel.minSellingRate = dealBuyingEntity.MinSellingRate;
            $scope.scopeModel.substituteRate = dealBuyingEntity.SubstituteRate;
            $scope.scopeModel.extraVolumeRate = dealBuyingEntity.ExtraVolumeRate;
            $scope.scopeModel.asr = dealBuyingEntity.ASR;
            $scope.scopeModel.ner = dealBuyingEntity.NER;
            $scope.scopeModel.acd = dealBuyingEntity.ACD;
        }

        function insertDealBuying() {
            $scope.scopeModel.isLoading = true;

            var dealBuyingObject = buildDealBuyingObjFromScope();
            if ($scope.onDealBuyingAdded != undefined)
                $scope.onDealBuyingAdded(dealBuyingObject);
            $scope.modalContext.closeModal();
        }

        function updateDealBuying() {
            var dealBuyingObject = buildDealBuyingObjFromScope();
            if ($scope.onDealBuyingUpdated != undefined)
                $scope.onDealBuyingUpdated(dealBuyingObject);
            $scope.modalContext.closeModal();
        }

        function buildDealBuyingObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SupplierZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                MinSellingRate: $scope.scopeModel.minSellingRate,
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

    appControllers.controller('WhS_BE_DealBuyingEditorController', DealBuyingEditorController);

})(appControllers);