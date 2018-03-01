(function (appControllers) {

    'use strict';

    SwapDealOutboundEditorController.$inject = ['$scope',  'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwapDealOutboundEditorController($scope,  UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var swapDealOutboundEntity;
        var supplierId;

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierZoneDirectiveAPI;
        var supplierZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySelectedPromiseDeferred;


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
                return (isEditMode) ? updateSwapDealOutbound() : insertSwapDealOutbound();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };


            $scope.onSupplierZoneDirectiveReady = function (api) {
                supplierZoneDirectiveAPI = api;
                supplierZoneReadyPromiseDeferred.resolve();
            };

            $scope.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };


            $scope.onCountrySelectionChanged = function () {
                var country = countryDirectiveApi.getSelectedIds();
                if (country != undefined) {
                    var setLoader = function (value) { $scope.isLoadingSelector = value };
                    var payload = {
                        supplierId: supplierId,
                        filter: { CountryIds: [countryDirectiveApi.getSelectedIds()] },
                        selectedIds: swapDealOutboundEntity != undefined ? swapDealOutboundEntity.SupplierZoneIds : undefined

                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneDirectiveAPI, payload, setLoader, countrySelectedPromiseDeferred);

                }
                else if (supplierZoneDirectiveAPI != undefined)
                    $scope.supplierzones.length = 0;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCountrySupplierZoneSection]).then(function () {
                swapDealOutboundEntity = undefined;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadCountrySupplierZoneSection() {
            var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            promises.push(loadCountryPromiseDeferred.promise);

            var payload;

            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryId != undefined) {
                payload = {};
                payload.selectedIds = swapDealOutboundEntity != undefined ? swapDealOutboundEntity.CountryId : undefined;
                countrySelectedPromiseDeferred = UtilsService.createPromiseDeferred();
            }

            countryReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, payload, loadCountryPromiseDeferred);
            });



            if (swapDealOutboundEntity != undefined && swapDealOutboundEntity.CountryId != undefined) {
                var loadSupplierZonePromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(loadSupplierZonePromiseDeferred.promise);

                UtilsService.waitMultiplePromises([supplierZoneReadyPromiseDeferred.promise, countrySelectedPromiseDeferred.promise]).then(function () {
                    var supplierZonePayload = {
                        supplierId: supplierId,
                        filter: { CountryIds: [swapDealOutboundEntity.CountryId] },
                        selectedIds: swapDealOutboundEntity != undefined ? swapDealOutboundEntity.SupplierZoneIds : undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(supplierZoneDirectiveAPI, supplierZonePayload, loadSupplierZonePromiseDeferred);
                    countrySelectedPromiseDeferred = undefined;
                });
            }

            return UtilsService.waitMultiplePromises(promises);
        }


        function setTitle() {
            if (isEditMode) {
                if (swapDealOutboundEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealOutboundEntity.Name, 'Buying Part',$scope);
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
        }

        function insertSwapDealOutbound() {
            $scope.scopeModel.isLoading = true;

            var swapDealOutboundObject = buildSwapDealOutboundObjFromScope();
            if ($scope.onSwapDealOutboundAdded != undefined)
                $scope.onSwapDealOutboundAdded(swapDealOutboundObject);
            $scope.modalContext.closeModal();
        }

        function updateSwapDealOutbound() {
            var swapDealOutboundObject = buildSwapDealOutboundObjFromScope();
            if ($scope.onSwapDealOutboundUpdated != undefined)
                $scope.onSwapDealOutboundUpdated(swapDealOutboundObject);
            $scope.modalContext.closeModal();
        }

        function buildSwapDealOutboundObjFromScope() {
            var obj = {
                Name: $scope.scopeModel.name,
                SupplierZoneIds: supplierZoneDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModel.volume,
                Rate: $scope.scopeModel.rate,
                CountryId: countryDirectiveApi.getSelectedIds()

            };
            return obj;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealOutboundEditorController', SwapDealOutboundEditorController);

})(appControllers);