(function (appControllers) {

    'use strict';

    DealProgressViewController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DealProgressViewController($scope, WhS_Deal_SwapDealAnalysisAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {
    
        var swapDealDirectiveAPI;
        var swapDealDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var volumeCommitmentDirectiveAPI;
        var volumeCommitmentDirectiveReadyDeferred=UtilsService.createPromiseDeferred();

        var isSwapDeal;
        var dealInfo;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealInfo = parameters.dealInfo;
                
            }
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.isSwapDeal = dealInfo.IsSwapDeal;

            $scope.scopeModel.onSwapDealDirectiveReady = function (api) {
                swapDealDirectiveAPI = api;
                swapDealDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onVolumeCommitmentDirectiveReady = function (api) {
                volumeCommitmentDirectiveAPI = api;
                volumeCommitmentDirectiveReadyDeferred.resolve();
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
            var promises = [setTitle];
            if ($scope.scopeModel.isSwapDeal)
                promises.push(loadSwapDealDirective);
            else
                promises.push(loadVolumeCommitmentDirective);

            return UtilsService.waitMultipleAsyncOperations(promises).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
                $scope.title = "Progress Of Last Deal: " + dealInfo.DealName;
        }
      
        function loadSwapDealDirective() {

            var swapDealDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            swapDealDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    dealInfo : dealInfo
                };
                VRUIUtilsService.callDirectiveLoad(swapDealDirectiveAPI, payload, swapDealDirectiveLoadDeferred);
            });

            return swapDealDirectiveLoadDeferred.promise;
        }
        function loadVolumeCommitmentDirective() {

            var volumeCommitmentDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            volumeCommitmentDirectiveReadyDeferred.promise.then(function () {
                var payload = {
                    dealInfo: dealInfo
                };
                VRUIUtilsService.callDirectiveLoad(volumeCommitmentDirectiveAPI, payload, volumeCommitmentDirectiveLoadDeferred);
            });

            return volumeCommitmentDirectiveLoadDeferred.promise;
        }

     
    }

    appControllers.controller('WhS_Deal_DealProgressViewController', DealProgressViewController);

})(appControllers);