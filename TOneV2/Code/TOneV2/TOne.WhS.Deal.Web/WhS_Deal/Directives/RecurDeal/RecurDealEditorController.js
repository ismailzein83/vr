(function (appControllers) {
    'use strict';
    RecurDealEditorController.$inject = ['$scope','WhS_Deal_DealDefinitionAPIService','VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];
    function RecurDealEditorController($scope, WhS_Deal_DealDefinitionAPIService, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var dealId;
        var dealName;
        var recurringTypeApi;
        var recurringTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                recurDeal();
            };

            $scope.onRecurringTypeSelectorReady = function (api) {
                recurringTypeApi = api;
                recurringTypeReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            UtilsService.waitMultipleAsyncOperations([setTitle,loadRecurringTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = UtilsService.buildTitleForAddEditor('Recurring Deals for ' + dealName);
        }

        function loadRecurringTypeSelector() {
            var recurringTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            recurringTypeReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(recurringTypeApi, undefined, recurringTypeSelectorLoadDeferred);
            });
            return recurringTypeSelectorLoadDeferred.promise;
        }

        function recurDeal() {
            $scope.scopeModel.isLoading = true;
            var dealObject = buildSwapDealInboundObjFromScope();
                $scope.onRecur(dealId, dealObject.RecurringNumber, dealObject.RecurringType).then(function (response) {
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
                RecurringNumber: $scope.scopeModel.RecurringNumber,
                RecurringType: recurringTypeApi.getSelectedIds(),
            };
            return obj;
        }


    }

    appControllers.controller('WhS_Deal_RecurDealEditorController', RecurDealEditorController);

})(appControllers);