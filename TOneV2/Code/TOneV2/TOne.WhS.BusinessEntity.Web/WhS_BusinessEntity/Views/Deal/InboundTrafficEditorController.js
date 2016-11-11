(function (appControllers) {

    'use strict';

    WhS_BE_InboundEditorController.$inject = ['$scope', 'WhS_BE_DealAPIService', 'WhS_BE_DealContractTypeEnum', 'WhS_BE_DealAgreementTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function WhS_BE_InboundEditorController($scope, WhS_BE_DealAPIService, WhS_BE_DealContractTypeEnum, WhS_BE_DealAgreementTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var inboundEntity;
        var sellingNumberPlanId;

        
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                inboundEntity = parameters.inbound;
            }


            isEditMode = (inboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateInbound() : insertInbound();
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
                    selectedIds: inboundEntity != undefined ? inboundEntity.SaleZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }



        function setTitle() {
            if (isEditMode) {
                if (inboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(inboundEntity.Name, 'InBound Traffic');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('InBound Traffic');
        }

        function loadStaticData() {
            if (inboundEntity == undefined)
                return;
            $scope.scopeModel.name = inboundEntity.Name;
            $scope.scopeModel.volume = inboundEntity.CommitedVolume;
            $scope.scopeModel.rate = inboundEntity.Rate;
            $scope.scopeModel.cost = inboundEntity.CurrentCost;
        }

        function insertInbound() {
            $scope.scopeModel.isLoading = true;

            var inboundObject = buildInboundObjFromScope();
            if ($scope.onInboundAdded != undefined)
                $scope.onInboundAdded(inboundObject);
            $scope.modalContext.closeModal();
        }

        function updateInbound() {
            var inboundObject = buildInboundObjFromScope();
            if ($scope.onInboundUpdated != undefined)
                $scope.onInboundUpdated(inboundObject);
            $scope.modalContext.closeModal();
        }

        function buildInboundObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                CommitedVolume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                CurrentCost: $scope.scopeModel.cost
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_InboundEditorController', WhS_BE_InboundEditorController);

})(appControllers);