(function (appControllers) {

    'use strict';

    OperatorDeclaredInfoItemEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_Be_TrafficDirectionEnum'];

    function OperatorDeclaredInfoItemEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_Be_TrafficDirectionEnum) {
        var isEditMode;

        var operatorDeclaredInfoItemEntity;
      
      
        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var trafficDirectionSelectorAPI;
        var trafficDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                operatorDeclaredInfoItemEntity = parameters.operatorDeclaredInfoItem;
            }
            isEditMode = (operatorDeclaredInfoItemEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onServiceTypeDirectiveReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTrafficDirectionReady = function (api) {
                trafficDirectionSelectorAPI = api;
                trafficDirectionSelectorReadyDeferred.resolve();
            };

           
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateOperatorDeclaredInfoItem() : insertOperatorDeclaredInfoItem();
            };


            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
       
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector, loadTrafficDirectionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                $scope.title = "Edit Operator Declared Info Item";
            }
            else {
                $scope.title = "Operator Declared Info Item";
            }
        }

        function loadStaticData() {
            if (operatorDeclaredInfoItemEntity == undefined)
                return;
            $scope.scopeModel.volume = operatorDeclaredInfoItemEntity.Volume;
            $scope.scopeModel.amount = operatorDeclaredInfoItemEntity.Amount;

        }
        function loadServiceTypeSelector() {
            var serviceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            serviceTypeSelectorReadyDeferred.promise.then(function () {
                var serviceTypeSelectorPayload;
                if (operatorDeclaredInfoItemEntity != undefined)
                    serviceTypeSelectorPayload = {
                        selectedIds: operatorDeclaredInfoItemEntity.ServiceTypeId 
                    };
                VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, serviceTypeSelectorLoadDeferred);
            });
            return serviceTypeSelectorLoadDeferred.promise;
        }
        function loadTrafficDirectionSelector() {
            var trafficDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            trafficDirectionSelectorReadyDeferred.promise.then(function () {
                var payload;
                if (operatorDeclaredInfoItemEntity != undefined)
                   payload = {
                       selectedIds: operatorDeclaredInfoItemEntity.TrafficDirection
                   };
                VRUIUtilsService.callDirectiveLoad(trafficDirectionSelectorAPI, payload, trafficDirectionSelectorLoadDeferred);
            });
            return trafficDirectionSelectorLoadDeferred.promise;
        }
       
       
       function insertOperatorDeclaredInfoItem() {
            $scope.scopeModel.isLoading = true;
            var operatorDeclaredInfoObj = buildOperatorDeclaredInfoItemObjFromScope();
            if ($scope.onOperatorDeclaredInfoItemAdded != undefined)
                $scope.onOperatorDeclaredInfoItemAdded(operatorDeclaredInfoObj);
           $scope.modalContext.closeModal();            
          $scope.scopeModel.isLoading = false;
        }
        function updateOperatorDeclaredInfoItem() {
            $scope.scopeModel.isLoading = true;
            var operatorDeclaredInfoObj = buildOperatorDeclaredInfoItemObjFromScope();
            if ($scope.onOperatorDeclaredInfoItemUpdated != undefined) {
                $scope.onOperatorDeclaredInfoItemUpdated(operatorDeclaredInfoObj);
            }
            $scope.scopeModel.isLoading = false;
            $scope.modalContext.closeModal();

        }
        function buildOperatorDeclaredInfoItemObjFromScope() {
            var obj = {
                $type: "Retail.BusinessEntity.Entities.OperatorDeclaredInfoItem, Retail.BusinessEntity.Entities",
                ServiceTypeId: serviceTypeSelectorAPI.getSelectedIds(),
                ServiceTypeName: $scope.scopeModel.serviceType.Title,
                TrafficDirectionValue: $scope.scopeModel.trafficDirection.description,
                TrafficDirection :trafficDirectionSelectorAPI.getSelectedIds(),
                Volume: $scope.scopeModel.volume,
                Amount: $scope.scopeModel.amount
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_OperatorDeclaredInfoItemEditorController', OperatorDeclaredInfoItemEditorController);

})(appControllers);