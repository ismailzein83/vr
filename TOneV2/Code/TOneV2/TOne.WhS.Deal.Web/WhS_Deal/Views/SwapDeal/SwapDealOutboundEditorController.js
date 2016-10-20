(function (appControllers) {

    'use strict';

    SwapDealOutboundEditorController.$inject = ['$scope',  'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealOutboundEditorController($scope,  UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var swapDealOutboundEntity;
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
                swapDealOutboundEntity = parameters.swapDealOutbound;
            }


            isEditMode = (swapDealOutboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDealOutbound() : insertDealOutbound();
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
                    selectedIds: swapDealOutboundEntity != undefined ? swapDealOutboundEntity.SupplierZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (swapDealOutboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealOutboundEntity.Name, 'Buying Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Buying Part');
        }

        function loadStaticData() {
            if (swapDealOutboundEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealOutboundEntity.Name;
            $scope.scopeModel.volume = swapDealOutboundEntity.Volume;
            $scope.scopeModel.rate = swapDealOutboundEntity.Rate;
            $scope.scopeModel.minSellingRate = swapDealOutboundEntity.MinSellingRate;
            $scope.scopeModel.substituteRate = swapDealOutboundEntity.SubstituteRate;
            $scope.scopeModel.extraVolumeRate = swapDealOutboundEntity.ExtraVolumeRate;
            $scope.scopeModel.asr = swapDealOutboundEntity.ASR;
            $scope.scopeModel.ner = swapDealOutboundEntity.NER;
            $scope.scopeModel.acd = swapDealOutboundEntity.ACD;
        }

        function insertDealOutbound() {
            $scope.scopeModel.isLoading = true;

            var dealOutboundObject = buildDealOutboundObjFromScope();
            if ($scope.onDealOutboundAdded != undefined)
                $scope.onDealOutboundAdded(dealOutboundObject);
            $scope.modalContext.closeModal();
        }

        function updateDealOutbound() {
            var dealOutboundObject = buildDealOutboundObjFromScope();
            if ($scope.onDealOutboundUpdated != undefined)
                $scope.onDealOutboundUpdated(dealOutboundObject);
            $scope.modalContext.closeModal();
        }

        function buildDealOutboundObjFromScope() {
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

    appControllers.controller('WhS_Deal_SwapDealOutboundEditorController', SwapDealOutboundEditorController);

})(appControllers);