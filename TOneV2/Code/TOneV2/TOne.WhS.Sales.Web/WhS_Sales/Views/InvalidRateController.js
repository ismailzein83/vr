(function (appControllers) {

    'use strict';

    InvalidRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function InvalidRateController($scope, UtilsService, VRNavigationService)
    {
        var calculatedRates;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var areAllRatesExcluded = false;

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                calculatedRates = parameters.calculatedRates;
            }
        }
        function defineScope()
        {
            $scope.title = 'Invalid Rates';

            $scope.scopeModel = {};
            $scope.scopeModel.invalidRates = [];

            $scope.scopeModel.onGridReady = function (api)
            {
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.isNewRateValid = function (dataItem) {
                if (dataItem.isExcluded)
                    return null;
                if (dataItem.newRate != undefined && dataItem.newRate != null && Number(dataItem.newRate) <= 0)
                    return 'New rate must be > 0';
                return null;
            };

            $scope.scopeModel.save = function ()
            {
                var validCalculatedRates = [];
                if (calculatedRates.ValidCalculatedRates != null) {
                    for (var i = 0; i < calculatedRates.ValidCalculatedRates.length; i++)
                        validCalculatedRates.push(calculatedRates.ValidCalculatedRates[i]);
                }

                for (var i = 0; i < $scope.scopeModel.invalidRates.length; i++)
                {
                    var dataItem = $scope.scopeModel.invalidRates[i];
                    if (!dataItem.isExcluded) {
                        validCalculatedRates.push({
                            ZoneId: dataItem.ZoneId,
                            ZoneName: dataItem.ZoneName,
                            CurrentRate: dataItem.CurrentRate,
                            CalculatedRate: dataItem.newRate
                        });
                    }
                }

                if ($scope.onSaved != undefined && $scope.onSaved != null && typeof ($scope.onSaved) == 'function')
                    $scope.onSaved(validCalculatedRates);

                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.excludeAll = function () {
                areAllRatesExcluded = !areAllRatesExcluded;
                for (var i = 0; i < $scope.scopeModel.invalidRates.length; i++) {
                    $scope.scopeModel.invalidRates[i].isExcluded = areAllRatesExcluded;
                }
            };

            $scope.scopeModel.cancel = function () {
                if ($scope.onCancelled != undefined && $scope.onCancelled != null && typeof ($scope.onCancelled) == 'function')
                    $scope.onCancelled();
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadGrid]).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadGrid()
        {
            gridReadyDeferred.promise.then(function ()
            {
                if (calculatedRates != undefined && calculatedRates.InvalidCalculatedRates != null)
                {
                    for (var i = 0; i < calculatedRates.InvalidCalculatedRates.length; i++)
                    {
                        var dataItem = calculatedRates.InvalidCalculatedRates[i];
                        dataItem.isExcluded = false;
                        $scope.scopeModel.invalidRates.push(dataItem);
                    }
                }
            });
        }
    }

    appControllers.controller('WhS_Sales_InvalidRateController', InvalidRateController);

})(appControllers);