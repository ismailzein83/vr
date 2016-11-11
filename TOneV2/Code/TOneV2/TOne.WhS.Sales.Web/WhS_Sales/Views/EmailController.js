﻿(function (appControllers) {

    'use strict';

    EmailController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_BE_CarrierAccountAPIService', 'WhS_Sales_CustomerSelectionTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function EmailController($scope, WhS_BE_CustomerSellingProductAPIService, WhS_BE_CarrierAccountAPIService, WhS_Sales_CustomerSelectionTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var taskId;
        var sellingProductId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                taskId = parameters.TaskId;
            }
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.customers = [];

            $scope.scopeModel.customerSelectionTypes = UtilsService.getArrayEnum(WhS_Sales_CustomerSelectionTypeEnum);
            $scope.scopeModel.selectedCustomerSelectionType = WhS_Sales_CustomerSelectionTypeEnum.All;

            $scope.scopeModel.onSelectionChanged = function () {
                if ($scope.scopeModel.selectedCustomerSelectionType == undefined)
                    return;
                if ($scope.scopeModel.selectedCustomerSelectionType.value != WhS_Sales_CustomerSelectionTypeEnum.All.value)
                    selectOrClearAllCustomers(false);
                else
                    selectOrClearAllCustomers(true);
            };

            $scope.scopeModel.save = function () {
                var customerIds = [];
                if ($scope.scopeModel.selectedCustomerSelectionType.value == WhS_Sales_CustomerSelectionTypeEnum.All.value) {
                    for (var i = 0; i < $scope.scopeModel.customers.length; i++)
                        customerIds.push($scope.scopeModel.customers[i].CarrierAccountId);
                }
                else {
                    for (var i = 0; i < $scope.scopeModel.customers.length; i++)
                        if ($scope.scopeModel.customers[i].isSelected)
                            customerIds.push($scope.scopeModel.customers[i].CarrierAccountId);
                }

                return executeTask(customerIds, true);
            };
            $scope.scopeModel.close = function () {
                return executeTask(undefined, false);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getSellingProduct().then(function () {
                loadAllControls();
                
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSellingProduct() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response != null && response.TaskData != null) {
                    sellingProductId = response.TaskData.SellingProductId;
                }
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = 'Notify Customers';
        }

        function loadGrid() {
            return WhS_BE_CustomerSellingProductAPIService.GetCustomerNamesBySellingProductId(sellingProductId).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.customers.push(response[i]);
                    }
                }
                selectOrClearAllCustomers(true);
            });

        }

        function executeTask(customerIds, taskAction) {

            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.Sales.BP.Arguments.Tasks.EmailTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
                CustomerIds: customerIds,
                Decision: taskAction
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: taskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function selectOrClearAllCustomers(isSelected) {
            for (var i = 0; i < $scope.scopeModel.customers.length; i++)
                $scope.scopeModel.customers[i].isSelected = isSelected;
        }
    }

    appControllers.controller('WhS_Sales_EmailController', EmailController);

})(appControllers);