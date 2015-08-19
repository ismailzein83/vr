CarrierAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'CarrierTypeFilterEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, CarrierTypeFilterEnum, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var selectedAccountManagerId = undefined;
    var assignedCarriers = [];

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            selectedAccountManagerId = parameters.selectedAccountManagerId;
        }
    }

    function defineScope() {

        $scope.carriers = [];
        $scope.selectedCarriers = [];
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        
        $scope.assignCarriers = function () {
            console.log($scope.carriers);

            denyDeselectedCarriers();
            var updatedCarriers = mapCarriersForAssignment();

            if (updatedCarriers.length > 0) {

                return AccountManagerAPIService.AssignCarriers(updatedCarriers)
                    .then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Assigned Carriers", response)) {
                            if ($scope.onCarriersAssigned != undefined)
                                $scope.onCarriersAssigned();

                            $scope.modalContext.closeModal();
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            }
        }

        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }

        $scope.denyCarrier = function ($event, item) {
            $event.preventDefault();
            $event.stopPropagation();
            
            item.newCustomerSwitchValue = false;
            item.newSupplierSwitchValue = false;

            var index = UtilsService.getItemIndexByVal($scope.selectedCarriers, item.CarrierAccountId, 'CarrierAccountId');
            $scope.selectedCarriers.splice(index, 1);
        };
    }

    function load() {
        $scope.isGettingData = true;

        UtilsService.waitMultipleAsyncOperations([loadCarriers, loadAssignedCarriers])
            .then(function () {
                toggleAndSelectAssignedCarriers();
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function loadCarriers() {

        return AccountManagerAPIService.GetCarriers(selectedAccountManagerId)
            .then(function (response) {
                $scope.carriers = getMappedCarriers(response);
            });
    }

    function loadAssignedCarriers() {

        return AccountManagerAPIService.GetAssignedCarriers(selectedAccountManagerId, false, CarrierTypeFilterEnum.Exchange.value)
            .then(function (response) {

                assignedCarriers = response;
            });
    }

    function getMappedCarriers(retrievedCarriers) {
        var mappedCarriers = [];

        angular.forEach(retrievedCarriers, function (carrier) {
            var object = {
                UserId: selectedAccountManagerId,
                CarrierAccountId: carrier.CarrierAccountId,
                CarrierName: carrier.CarrierName,
                IsCustomerAvailable: carrier.IsCustomerAvailable,
                customerSwitchValue: false,
                newCustomerSwitchValue: false,
                IsSupplierAvailable: carrier.IsSupplierAvailable,
                supplierSwitchValue: false,
                newSupplierSwitchValue: false
            }

            mappedCarriers.push(object);
        });

        return mappedCarriers;
    }

    function toggleAndSelectAssignedCarriers() {

        for (var i = 0; i < assignedCarriers.length; i++) {

            var carrier = UtilsService.getItemByVal($scope.carriers, assignedCarriers[i].CarrierAccountId, 'CarrierAccountId'); // will never return null
            
            if (assignedCarriers[i].RelationType == CarrierTypeFilterEnum.Customer.value) {
                carrier.customerSwitchValue = true;
                carrier.newCustomerSwitchValue = true;
            }
            else if (assignedCarriers[i].RelationType == CarrierTypeFilterEnum.Supplier.value) {
                carrier.supplierSwitchValue = true;
                carrier.newSupplierSwitchValue = true;
            }

            // 2 array items in $scope.selectedCarriers may correspond to the same carrier
            if (!UtilsService.contains($scope.selectedCarriers, carrier))
                $scope.selectedCarriers.push(carrier); // select the assigned carrier
        }
    }

    function mapCarriersForAssignment() {
        var mappedCarriers = [];

        for (var i = 0; i < $scope.carriers.length; i++)
            mappedCarriers = mappedCarriers.concat(mapCarrierForAssignment($scope.carriers[i]));

        return mappedCarriers;
    }

    function mapCarrierForAssignment(carrier) {
        var mappedCarrier = [];

        if (carrier.customerSwitchValue != carrier.newCustomerSwitchValue) {
            console.log('1');
            var object = {
                UserId: carrier.UserId,
                CarrierAccountId: carrier.CarrierAccountId,
                RelationType: CarrierTypeFilterEnum.Customer.value,
                Status: carrier.newCustomerSwitchValue
            };
            mappedCarrier.push(object);
        }

        if (carrier.supplierSwitchValue != carrier.newSupplierSwitchValue) {
            console.log('2');
            var object = {
                UserId: carrier.UserId,
                CarrierAccountId: carrier.CarrierAccountId,
                RelationType: CarrierTypeFilterEnum.Supplier.value,
                Status: carrier.newSupplierSwitchValue
            };
            mappedCarrier.push(object);
        }

        return mappedCarrier;
    }

    function denyDeselectedCarriers() {

        for (var i = 0; i < assignedCarriers.length; i++) {
            var index = UtilsService.getItemIndexByVal($scope.selectedCarriers, assignedCarriers[i].CarrierAccountId, 'CarrierAccountId');
            
            if (index == -1) { // the assigned carrier has been deselected
                var carrier = UtilsService.getItemByVal($scope.carriers, assignedCarriers[i].CarrierAccountId, 'CarrierAccountId');

                // deny the carrier
                carrier.newCustomerSwitchValue = false;
                carrier.newSupplierSwitchValue = false;
            }
        }
    }
}

appControllers.controller('BusinessEntity_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
