(function (appControllers) {

    'use strict';

    CarrierAssignmentEditorController.$inject = ['$scope', 'WhS_BE_AccountManagerAPIService', 'WhS_BE_CarrierAccountTypeEnum', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function CarrierAssignmentEditorController($scope, WhS_BE_AccountManagerAPIService, WhS_BE_CarrierAccountTypeEnum, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var selectedAccountManagerId;
        var assignedCarriers;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                selectedAccountManagerId = parameters.selectedAccountManagerId;
            }
        }

        function defineScope() {

            $scope.hasAssignCarriersPermission = function () {
                return WhS_BE_AccountManagerAPIService.HasAssignCarriersPermission();
            };

            $scope.carriers = [];
            $scope.selectedCarriers = [];

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.onSelectItem = function (item) {
                var value = UtilsService.getItemByVal(assignedCarriers, item.CarrierAccountId, 'Entity.CarrierAccountId');
                if (UtilsService.getItemIndexByVal($scope.carriers, item.CarrierAccountId, 'Entity.CarrierAccountId') == -1) {
                    var obj = {
                        CarrierName: item.Name,
                        Entity: { RelationType: item.CarrierType, CarrierAccountId: item.CarrierAccountId, UserId: selectedAccountManagerId },
                        IsCustomerAssigned: value != null ? value.IsCustomerAssigned : false,
                        IsCustomerAvailable: value != null ? value.IsCustomerAvailable : item.IsCustomerAvailable,
                        IsCustomerInDirect: value != null ? value.IsCustomerInDirect : false,
                        IsSupplierAssigned: value != null ? value.IsSupplierAssigned : false,
                        IsSupplierAvailable: value != null ? value.IsSupplierAvailable : item.IsSupplierAvailable,
                        IsSupplierInDirect: value != null ? value.IsSupplierInDirect : false
                    };

                    extendGridItem(obj);
                    $scope.carriers.push(obj);
                }
            };

            $scope.onDeselectItem = function (item) {
                item.newCustomerSwitchValue = false;
                item.newSupplierSwitchValue = false;
                var index1 = UtilsService.getItemIndexByVal($scope.carriers, item.CarrierAccountId, 'Entity.CarrierAccountId');
                $scope.carriers.splice(index1, 1);
            };

            $scope.assignCarriers = function () {
                var updatedCarriers = mapCarriersForAssignment();
                return WhS_BE_AccountManagerAPIService.AssignCarriers(updatedCarriers).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated('Assigned Carriers', response)) {
                        if ($scope.onCarriersAssigned && typeof $scope.onCarriersAssigned == 'function') {
                            $scope.onCarriersAssigned();
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.validateForm = function () {
                if (mapCarriersForAssignment().length > 0)
                    return null;
                return 'No changes made';
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCarriers, loadAssignedCarriers]).then(function () {
                toggleAndSelectAssignedCarriers();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = 'Assign Carriers';
            }

            function loadCarriers() {
                var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    var payload = { filter: { AssignableToUserId: selectedAccountManagerId } }
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
                });

                return loadCarrierAccountPromiseDeferred.promise;
            }

            function loadAssignedCarriers() {
                return WhS_BE_AccountManagerAPIService.GetAssignedCarrierDetails(selectedAccountManagerId, false, WhS_BE_CarrierAccountTypeEnum.Exchange.value).then(function (response) {
                    if (response) {
                        assignedCarriers = [];

                        for (var i = 0; i < response.length; i++) {
                            assignedCarriers.push(response[i]);
                        }
                    }
                });
            }
        }

        function toggleAndSelectAssignedCarriers() {
            for (var i = 0; i < assignedCarriers.length; i++) {
                if (assignedCarriers[i].Entity.RelationType == WhS_BE_CarrierAccountTypeEnum.Customer.value || assignedCarriers[i].IsCustomerAssigned) {
                    assignedCarriers[i].customerSwitchValue = true;
                    assignedCarriers[i].newCustomerSwitchValue = true;
                }
                else {
                    assignedCarriers[i].customerSwitchValue = false;
                    assignedCarriers[i].newCustomerSwitchValue = false;
                }
                if (assignedCarriers[i].Entity.RelationType == WhS_BE_CarrierAccountTypeEnum.Supplier.value || assignedCarriers[i].IsSupplierAssigned) {
                    assignedCarriers[i].supplierSwitchValue = true;
                    assignedCarriers[i].newSupplierSwitchValue = true;
                } else {
                    assignedCarriers[i].supplierSwitchValue = false;
                    assignedCarriers[i].newSupplierSwitchValue = false;
                }
                
                extendGridItem(assignedCarriers[i]);
                
                $scope.carriers.push(assignedCarriers[i])
            }
        }

        function extendGridItem(gridItem) {
            gridItem.isDirty = false;

            gridItem.onSwitchValueChanged = function () {
                gridItem.isDirty = !gridItem.isDirty;
            };
        }

        function mapCarriersForAssignment() {
            var mappedCarriers = [];
            for (var i = 0; i < $scope.carriers.length; i++)
                mapCarrierForAssignment($scope.carriers[i], mappedCarriers);

            return mappedCarriers;
        }

        function mapCarrierForAssignment(carrier, mappedCarriers) {
            if (!carrier.isDirty)
                return;

            if (carrier.customerSwitchValue != carrier.newCustomerSwitchValue && carrier.IsCustomerAvailable) {
                var object = {
                    UserId: carrier.Entity.UserId,
                    CarrierAccountId: carrier.Entity.CarrierAccountId,
                    RelationType: WhS_BE_CarrierAccountTypeEnum.Customer.value,
                    Status: carrier.newCustomerSwitchValue
                };
                mappedCarriers.push(object);
            }

            if (carrier.supplierSwitchValue != carrier.newSupplierSwitchValue && carrier.IsSupplierAvailable) {
                var object = {
                    UserId: carrier.Entity.UserId,
                    CarrierAccountId: carrier.Entity.CarrierAccountId,
                    RelationType: WhS_BE_CarrierAccountTypeEnum.Supplier.value,
                    Status: carrier.newSupplierSwitchValue
                };
                mappedCarriers.push(object);

            }
        }
    }

    appControllers.controller('WhS_BE_CarrierAssignmentEditorController', CarrierAssignmentEditorController);

})(appControllers);
