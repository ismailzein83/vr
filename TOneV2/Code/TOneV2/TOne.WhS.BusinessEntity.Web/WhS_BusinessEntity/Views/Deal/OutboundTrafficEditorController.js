(function (appControllers) {

    'use strict';

    OutboundTrafficEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function OutboundTrafficEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var outboundEntity;
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


            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
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

            saleZoneReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    sellingNumberPlanId: sellingNumberPlanId,
                    selectedIds: outboundEntity != undefined ? outboundEntity.SaleZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
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
                SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                CommitedVolume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                CurrentCost: $scope.scopeModel.cost,
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_OutboundTrafficEditorController', OutboundTrafficEditorController);

})(appControllers);