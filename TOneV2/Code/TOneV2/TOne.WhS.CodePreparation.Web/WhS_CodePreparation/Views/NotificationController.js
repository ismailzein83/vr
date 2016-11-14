(function (appControllers) {

    'use strict';

    NotificationController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'WhS_Sales_CustomerSelectionTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function NotificationController($scope, WhS_BE_CarrierAccountAPIService, WhS_Sales_CustomerSelectionTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var taskId;
        var sellingNumberPlanId;

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

            $scope.scopeModel.showCustomers = false;

            $scope.scopeModel.customerSelectionTypes = UtilsService.getArrayEnum(WhS_Sales_CustomerSelectionTypeEnum);
            $scope.scopeModel.selectedCustomerSelectionType = WhS_Sales_CustomerSelectionTypeEnum.All;

            $scope.scopeModel.onSelectionChanged = function () {
                if ($scope.scopeModel.selectedCustomerSelectionType == undefined)
                    return;
                if ($scope.scopeModel.selectedCustomerSelectionType.value != WhS_Sales_CustomerSelectionTypeEnum.All.value) {
                    clearAllCustomers();
                    $scope.scopeModel.showCustomers = true;
                }
                else
                    $scope.scopeModel.showCustomers = false;
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

            getSellingNumberPlanId().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSellingNumberPlanId() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response != null && response.TaskData != null) {
                    sellingNumberPlanId = response.TaskData.SellingNumberPlanId;
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
            return WhS_BE_CarrierAccountAPIService.GetCustomersBySellingNumberPlanId(sellingNumberPlanId).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.customers.push(response[i]);
                    }
                }
            });
        }

        function executeTask(customerIds, taskAction) {

            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.CodePreparation.BP.Arguments.Tasks.NotificationTaskExecutionInforamtion, TOne.WhS.CodePreparation.BP.Arguments",
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

        function clearAllCustomers(isSelected) {
            for (var i = 0; i < $scope.scopeModel.customers.length; i++)
                $scope.scopeModel.customers[i].isSelected = isSelected;
        }
    }

    appControllers.controller('WhS_CP_NotificationController', NotificationController);

})(appControllers);