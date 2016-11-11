(function (appControllers) {

    'use strict';

    OutboundTrafficEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function OutboundTrafficEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var outboundEntity;
        var supplierId;

        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                supplierId = parameters.supplierId;
                outboundEntity = parameters.outbound;
            }

            isEditMode = (outboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateOutbound() : insertOutbound();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSupplierZoneSection]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadSupplierZoneSection() {
            var loadSupplierZonePromiseDeferred = UtilsService.createPromiseDeferred();

            supplierZoneReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    supplierId: supplierId,
                    selectedIds: outboundEntity != undefined ? outboundEntity.SupplierZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, payload, loadSupplierZonePromiseDeferred);
            });
            return loadSupplierZonePromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode) {
                if (outboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(outboundEntity.Name, 'Outbound Traffic');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Outbound Traffic');
        }

        function loadStaticData() {
            if (outboundEntity == undefined)
                return;
            $scope.scopeModel.name = outboundEntity.Name;
            $scope.scopeModel.volume = outboundEntity.CommitedVolume;
            $scope.scopeModel.rate = outboundEntity.Rate;
            $scope.scopeModel.cost = outboundEntity.CurrentCost;
        }

        function insertOutbound() {
            $scope.scopeModel.isLoading = true;

            var OutboundObject = buildOutboundObjFromScope();
            if ($scope.onOutboundAdded != undefined)
                $scope.onOutboundAdded(OutboundObject);
            $scope.modalContext.closeModal();
        }

        function updateOutbound() {
            var OutboundObject = buildOutboundObjFromScope();
            if ($scope.onOutboundUpdated != undefined)
                $scope.onOutboundUpdated(OutboundObject);
            $scope.modalContext.closeModal();
        }

        function buildOutboundObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SupplierZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                CommitedVolume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                CurrentCost: $scope.scopeModel.cost,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_OutboundTrafficEditorController', OutboundTrafficEditorController);

})(appControllers);