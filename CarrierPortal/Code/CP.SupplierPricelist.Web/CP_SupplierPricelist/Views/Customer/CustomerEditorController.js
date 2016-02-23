(function (appControllers) {

    "use strict";

    customerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService'];

    function customerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, customerManagmentAPIService) {

        var isEditMode;
        var customerEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {

        }
        function buildCustomerFromScope() {

            var customerObject = {
                name: $scope.name//,
                //CustomerSettings: VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, sellingRuleSettingsAPI, 'selectedSellingRuleSettingsTemplate')
            };

            return customerObject;
        }
        function defineScope() {
            $scope.sourceTypeTemplates = [];
            $scope.close = function () {
                $scope.modalContext.closeModal();
            }
            $scope.SaveCustomer = function () {
                if (isEditMode) {
                    return updateCustomer();
                }
                else {
                    return insertCustomer();
                }
            };
        }
        function insertCustomer() {
            var customerObject = buildCustomerFromScope();
            return customerManagmentAPIService.AddCustomer(customerObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer", response)) {
                    if ($scope.onCustomerAdded != undefined)
                        $scope.onCustomerAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCustomer() {
            var customerObject = buildCustomerFromScope();
            customerManagmentAPIService.UpdateCustomer(customerObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Customer", response)) {
                    if ($scope.onCustomerUpdated != undefined)
                        $scope.onCustomerUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
        function load() {
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, LoadCustomerTemplates])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && customerEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(customerEntity.Name, "Customer");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Customer");
        }

        function LoadCustomerTemplates() {

            customerManagmentAPIService.GetCustomerTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.sourceTypeTemplates.push(item);
                });
            });
        }
    }
    appControllers.controller('CP_SupplierPricelist_CustomerEditorController', customerEditorController);
})(appControllers);