(function (appControllers) {
    "use strict";

    swapDealCapacityController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_Deal_SwapDealAPIService', 'UtilsService'];

    function swapDealCapacityController($scope, utilsService, vrNotificationService, vrNavigationService, WhS_Deal_SwapDealAPIService, UtilsService) {
        var dealContext;
        var carrierAccountId;
        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.swapDealCapacityDataSource = [];

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function loadStaticData() {
            var dealSettings = dealContext.Settings;
            $scope.scopeModel.dealBED = dealSettings.BeginDate;
            $scope.scopeModel.dealEED = dealSettings.EEDToStore;
            var totalSellVolume = 0;
            for (var i = 0; i < dealSettings.Inbounds.length; i++) {
                totalSellVolume = parseInt(totalSellVolume, 10) + parseInt(dealSettings.Inbounds[i].Volume, 10);
            }
            $scope.scopeModel.sellingVolume = totalSellVolume;
            var totalCostVolume = 0;
            for (var j = 0; j < dealSettings.Outbounds.length; j++) {
                totalCostVolume = parseInt(totalCostVolume, 10) + parseInt(dealSettings.Outbounds[j].Volume, 10);
            }
            $scope.scopeModel.buyingVolume = totalCostVolume;
        }
        function loadParameters() {
            UtilsService.setContextReadOnly($scope);
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dealContext = parameters.DealContext;
                carrierAccountId = dealContext.Settings.CarrierAccountId;
            }
        }
        function GetCarrierAccountChannelsLimit() {
            return WhS_Deal_SwapDealAPIService.GetCarrierAccountChannelsLimit(carrierAccountId).then(function (response) {
                $scope.scopeModel.channelsLimit = response;
                $scope.scopeModel.capacityPerDay = response * 60 * 24;
            });
        }
        function setTitle() {
            $scope.title = 'Capacity Validation';
        }
        function GetSwapDealAboveCapacity() {
            return WhS_Deal_SwapDealAPIService.GetSwapDealsAboveCapacity(dealContext).then(function (response) {
                $scope.scopeModel.swapDealCapacityDataSource = response;
            });
        }
        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, GetSwapDealAboveCapacity, GetCarrierAccountChannelsLimit]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }
    }
    appControllers.controller('WhS_BE_SwapDealCapacityController', swapDealCapacityController);
})(appControllers);