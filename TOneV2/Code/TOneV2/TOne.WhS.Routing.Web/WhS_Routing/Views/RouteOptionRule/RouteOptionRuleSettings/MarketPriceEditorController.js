(function (appControllers) {

    'use strict';

    MarketPriceController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function MarketPriceController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var marketPriceEntity;
        var marketPrices; //passed for validation

        var zoneServiceConfigSelectorAPI;
        var zoneServiceConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                marketPriceEntity = parameters.marketPrice;
                marketPrices = parameters.marketPrices;
            }

            isEditMode = (marketPriceEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onZoneServiceConfigSelectorReady = function (api) {
                zoneServiceConfigSelectorAPI = api;
                zoneServiceConfigSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateZoneServiceConfig = function () {

                var _serviceValues = $scope.scopeModel.zoneServiceConfigSelectedValues;
                if (_serviceValues == undefined)
                    return 'Required Field';

                var _serviceIds = [];
                for (var index = 0; index < _serviceValues.length; index++) {
                    _serviceIds.push(_serviceValues[index].ZoneServiceConfigId);
                }

                //for Edit Mode
                if (marketPriceEntity != undefined && compareTwoArrayValues(marketPriceEntity.ServiceIds, _serviceIds))
                    return null;

                for (var i = 0; i < marketPrices.length; i++) {
                    var marketPrice = marketPrices[i];
                    if (compareTwoArrayValues(marketPrice.ServiceIds, _serviceIds)) {
                        return 'Same Services Exist';
                    }
                }
                return null;
            };
            $scope.scopeModel.validateRates = function () {
                if ($scope.scopeModel.minRate != undefined && $scope.scopeModel.maxRate != undefined && parseFloat($scope.scopeModel.minRate) > parseFloat($scope.scopeModel.maxRate))
                    return 'Min Rate should be less than Max Rate';

                return null;
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadZoneServiceConfigSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((marketPriceEntity != undefined) ? marketPriceEntity.ServiceIds : null, 'Market Price') :
                    UtilsService.buildTitleForAddEditor('Market Prices');
            }
            function loadStaticData() {
                if (marketPriceEntity == undefined)
                    return;

                $scope.scopeModel.minRate = marketPriceEntity.Minimum;
                $scope.scopeModel.maxRate = marketPriceEntity.Maximum;
            }
            function loadZoneServiceConfigSelector() {

                var zoneServiceConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                zoneServiceConfigSelectorReadyDeferred.promise.then(function () {
                    var zoneServiceConfigSelectorPayload = {};
                    if (marketPriceEntity != undefined) {
                        zoneServiceConfigSelectorPayload.selectedIds = marketPriceEntity.ServiceIds;
                    }

                    VRUIUtilsService.callDirectiveLoad(zoneServiceConfigSelectorAPI, zoneServiceConfigSelectorPayload, zoneServiceConfigSelectorLoadDeferred);
                });

                return zoneServiceConfigSelectorLoadDeferred.promise;
            }
        }

        function insert() {
            var calculationFieldObject = buildMarketPriceObjectFromScope();

            if ($scope.onMarketPriceAdded != undefined && typeof ($scope.onMarketPriceAdded) == 'function') {
                $scope.onMarketPriceAdded(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var calculationFieldObject = buildMarketPriceObjectFromScope();

            if ($scope.onMarketPriceUpdated != undefined && typeof ($scope.onMarketPriceUpdated) == 'function') {
                $scope.onMarketPriceUpdated(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildMarketPriceObjectFromScope() {

            var marketPriceObj = {
                ServiceIds: zoneServiceConfigSelectorAPI.getSelectedIds(),
                Minimum: $scope.scopeModel.minRate,
                Maximum: $scope.scopeModel.maxRate,
            };

            //ServiceNames to display at the grid
            marketPriceObj.ServiceNames = buildServiceNames($scope.scopeModel.zoneServiceConfigSelectedValues);

            return marketPriceObj;
        }
        function buildServiceNames(zoneServiceConfigSelectedValues) {
            var serviceNames = [];

            if (zoneServiceConfigSelectedValues) {
                for (var index in zoneServiceConfigSelectedValues) {
                    serviceNames.push(zoneServiceConfigSelectedValues[index].Symbol);
                }
            }
            return  serviceNames.join(", ");
        }
        function compareTwoArrayValues(array1, array2) {
            if (array1 == undefined || array2 == undefined)
                return false;

            if (array1.sort().join(',') != array2.sort().join(','))
                return false;

            return true;
        }
    }

    appControllers.controller('WhS_Routing_MarketPriceController', MarketPriceController);

})(appControllers);