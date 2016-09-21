(function (appControllers) {

    "use strict";

    customerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService'];

    function customerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, customerManagmentAPIService) {

        var isEditMode;
        var customerEntity;
        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred;

        var customerId;
        var name;
        var configId;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                customerId = parameters.customerId;
            }
            isEditMode = (customerId != undefined);
        }
        function buildCustomerFromScope() {
            var customerObject = {
                CustomerId: (customerId != undefined) ? customerId : 0,
                Name: $scope.name,
                Settings: {
                    PriceListConnector: VRUIUtilsService.getSettingsFromDirective($scope, sourceTypeDirectiveAPI, 'selectedSourceTypeTemplate')
                }
            };

            return customerObject;
        }

        function defineScope() {
            $scope.sourceTypeTemplates = [];
            $scope.close = function () {
                $scope.modalContext.closeModal();
            }
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
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
                if (VRNotificationService.notifyOnItemAdded("Customer", response, 'name')) {
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
            return customerManagmentAPIService.UpdateCustomer(customerObject)
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
            $scope.isLoading = true;
            if (isEditMode) {
                getCustomer().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function getCustomer() {
            return customerManagmentAPIService.GetCustomer(customerId).then(function (customer) {
                customerEntity = customer;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, LoadCustomerTemplates, loadSourceType])
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
        function loadStaticData() {
            if (customerEntity == undefined)
                return;

            $scope.name = customerEntity.Name;
        }
        function LoadCustomerTemplates() {
            return customerManagmentAPIService.GetCustomerTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.sourceTypeTemplates.push(item);
                });
                var sourceConfigId;
                if (customerEntity != undefined && customerEntity.Settings != undefined && customerEntity.Settings.PriceListConnector != undefined)
                    sourceConfigId = customerEntity.Settings.PriceListConnector.ConfigId;

                if (sourceConfigId != undefined)
                    $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");
            });
        }

        function loadSourceType() {

            sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
            sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                sourceDirectiveReadyPromiseDeferred = undefined;
                var obj;
                if (customerEntity != undefined && customerEntity.Settings != undefined && customerEntity.Settings.PriceListConnector != undefined) {

                    obj = {
                        Url: customerEntity.Settings.PriceListConnector.Url,
                        Username: customerEntity.Settings.PriceListConnector.UserName,
                        Password: customerEntity.Settings.PriceListConnector.Password
                    };

                }

                VRUIUtilsService.callDirectiveLoad(sourceTypeDirectiveAPI, obj, loadSourceTemplatePromiseDeferred);
            });
            return loadSourceTemplatePromiseDeferred.promise;
        }
    }
    appControllers.controller('CP_SupplierPricelist_CustomerEditorController', customerEditorController);
})(appControllers);