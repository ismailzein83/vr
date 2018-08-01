(function (appControllers) {
    'use strict';
    ReoccurDealEditorController.$inject = ['$scope','WhS_Deal_DealDefinitionAPIService','VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];
    function ReoccurDealEditorController($scope, WhS_Deal_DealDefinitionAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var dealId;
        var dealName;
        var reoccuringTypeApi;
        var reoccuringTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dealId = parameters.dealId;
                dealName = parameters.dealName;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                reoccurDeal();
            };

            $scope.onReoccuringTypeSelectorReady = function (api) {
                reoccuringTypeApi = api;
                reoccuringTypeReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            UtilsService.waitMultipleAsyncOperations([setTitle,loadReoccuringTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForAddEditor('Recurring Deals for ' + dealName);
        }

        function loadReoccuringTypeSelector() {
            var reoccuringTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            reoccuringTypeReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(reoccuringTypeApi, undefined, reoccuringTypeSelectorLoadDeferred);
            });
            return reoccuringTypeSelectorLoadDeferred.promise;
        }

        function reoccurDeal() {
            $scope.scopeModel.isLoading = true;
            var dealObject = buildSwapDealInboundObjFromScope();
                $scope.onReoccur(dealId, dealObject.ReoccuringNumber, dealObject.ReouccuringType).then(function (response) {
                    console.log(response);
                    if (response.Result == 0)
                        $scope.modalContext.closeModal();
                    else
                        $scope.validationMessages = response.ValidationMessages; 
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwapDealInboundObjFromScope() {
            var obj = {
                ReoccuringNumber: $scope.scopeModel.reoccuringNumber,
                ReouccuringType: reoccuringTypeApi.getSelectedIds(),
            };
            return obj;
        }


    }

    appControllers.controller('WhS_Deal_ReoccurDealEditorController', ReoccurDealEditorController);

})(appControllers);