(function (appControllers) {

    'use strict';

    SwapDealBuyingEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealBuyingEditorController($scope, WhS_BE_DealAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var swapDealBuyingEntity;
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
                swapDealBuyingEntity = parameters.swapDealBuying;
            }


            isEditMode = (swapDealBuyingEntity != undefined);
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
            }
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
                    selectedIds: swapDealBuyingEntity != undefined ? swapDealBuyingEntity.SupplierZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (swapDealBuyingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealBuyingEntity.Name, 'Buying Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Buying Part');
        }

        function loadStaticData() {
            if (swapDealBuyingEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealBuyingEntity.Name;
            $scope.scopeModel.volume = swapDealBuyingEntity.Volume;
            $scope.scopeModel.rate = swapDealBuyingEntity.Rate;
            $scope.scopeModel.minSellingRate = swapDealBuyingEntity.MinSellingRate;
            $scope.scopeModel.substituteRate = swapDealBuyingEntity.SubstituteRate;
            $scope.scopeModel.extraVolumeRate = swapDealBuyingEntity.ExtraVolumeRate;
            $scope.scopeModel.asr = swapDealBuyingEntity.ASR;
            $scope.scopeModel.ner = swapDealBuyingEntity.NER;
            $scope.scopeModel.acd = swapDealBuyingEntity.ACD;
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

    appControllers.controller('WhS_Deal_SwapDealBuyingEditorController', SwapDealBuyingEditorController);

})(appControllers);