(function (appControllers) {

    'use strict';

    InvalidRateController.$inject = ['$scope', 'UtilsService', 'VRNavigationService'];

    function InvalidRateController($scope, UtilsService, VRNavigationService)
    {
        var invalidRates;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                invalidRates = parameters.invalidRates;
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
                if (dataItem.newRate != undefined && dataItem.newRate != null && Number(dataItem.newRate) > 0)
                    return 'New rate must be > 0';
                return null;
            };

            $scope.scopeModel.save = function () {
                if ($scope.onSaved != undefined && $scope.onSaved != null && typeof ($scope.onSaved) == 'function')
                    $scope.onSaved();
                $scope.modalContext.closeModal();
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
                if (invalidRates != undefined) {
                    for (var i = 0; i < invalidRates.length; i++) {
                        var dataItem = invalidRates[i];
                        dataItem.isExcluded = false;
                        $scope.scopeModel.invalidRates.push(dataItem);
                    }
                }
            });
        }
    }

    appControllers.controller('WhS_Sales_InvalidRateController', InvalidRateController);

})(appControllers);