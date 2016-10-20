(function (appControllers) {

    'use strict';

    SwapDealInboundEditorController.$inject = ['$scope',  'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealInboundEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;



        var swapDealInboundEntity;
        var sellingNumberPlanId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

   

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                sellingNumberPlanId = parameters.sellingNumberPlanId;
                swapDealInboundEntity = parameters.swapDealInbound;
            }
            
            isEditMode = (swapDealInboundEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDealInbound() : insertDealInbound();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }
            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySelector  , loadSaleZoneSection]).catch(function (error) {
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
                    selectedIds: swapDealInboundEntity != undefined ? swapDealInboundEntity.SaleZoneIds : undefined
                };

                VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, payload, loadSaleZonePromiseDeferred);
            });
            return loadSaleZonePromiseDeferred.promise;
        }

        function loadCountrySelector() {
            var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: swapDealInboundEntity != undefined ? swapDealInboundEntity.CountryId :  undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode) {
                if (swapDealInboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealInboundEntity.Name, 'Selling Part');
            }
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Selling Part');
        }

        function loadStaticData() {
            if (swapDealInboundEntity == undefined)
                return;
            $scope.scopeModel.name = swapDealInboundEntity.Name;
            $scope.scopeModel.volume = swapDealInboundEntity.Volume;
            $scope.scopeModel.rate = swapDealInboundEntity.Rate;
           
        }

        function insertDealInbound() {
            $scope.scopeModel.isLoading = true;
            
            var dealInboundObject = buildDealInboundObjFromScope();
            if ($scope.onDealInboundAdded != undefined)
                $scope.onDealInboundAdded(dealInboundObject);
            $scope.modalContext.closeModal();
        }

        function updateDealInbound() {
            var dealInboundObject = buildDealInboundObjFromScope();
            if ($scope.onDealInboundUpdated != undefined)
                $scope.onDealInboundUpdated(dealInboundObject);
            $scope.modalContext.closeModal();
        }

        function buildDealInboundObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SaleZoneIds: saleZoneDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
              
            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealInboundEditorController', SwapDealInboundEditorController);

})(appControllers);