(function (appControllers) {

    'use strict';

    SwapDealSellingEditorController.$inject = ['$scope',  'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealSellingEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var swapDealSellingEntity;
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
                swapDealSellingEntity = parameters.swapDealSelling;
            }
            
            isEditMode = (swapDealSellingEntity != undefined);
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
                    selectedIds: swapDealSellingEntity != undefined ? swapDealSellingEntity.SaleZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode) {
                if (swapDealSellingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealSellingEntity.Name, 'Selling Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Selling Part');
        }

        function loadStaticData() {
            if (swapDealSellingEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealSellingEntity.Name;
            $scope.scopeModel.volume = swapDealSellingEntity.Volume;
            $scope.scopeModel.rate = swapDealSellingEntity.Rate;
           
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
              
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_DealSellingEditorController', DealSellingEditorController);

})(appControllers);