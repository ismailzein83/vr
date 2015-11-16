CarrierAssignmentEditorController.$inject = ['$scope', 'WhS_BE_AccountManagerAPIService', 'WhS_Be_CarrierAccountTypeEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService','VRUIUtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, CarrierTypeEnum, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

    var selectedAccountManagerId = undefined;
    var assignedCarriers = [];
    var carrierAccountDirectiveAPI;
    var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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

        $scope.onCarrierAccountDirectiveReady = function (api) {
            carrierAccountDirectiveAPI = api;
            carrierAccountReadyPromiseDeferred.resolve();
        }

        $scope.onSelectItem = function (item) {
            var obj = {
                CarrierName: item.Name,
                Entity: { RelationType: item.CarrierType, CarrierAccountId: item.CarrierAccountId, UserId: selectedAccountManagerId },
                IsCustomerAssigned: false,
                IsCustomerAvailable: item.IsCustomerAvailable,
                IsCustomerInDirect:false,
                IsSupplierAssigned:false,
                IsSupplierAvailable:item.IsSupplierAvailable,
                IsSupplierInDirect:false
            }
            $scope.carriers.push(obj)
        }
        $scope.onDeselectItem = function (item) {
            var index1 = UtilsService.getItemIndexByVal($scope.carriers, item.CarrierAccountId, 'Entity.CarrierAccountId');
            $scope.carriers.splice(index1, 1);
        }

        $scope.assignCarriers = function () {

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

        $scope.denyCarrier = function (item) {   
            item.newCustomerSwitchValue = false;
            item.newSupplierSwitchValue = false;

            var index = UtilsService.getItemIndexByVal($scope.selectedCarriers, item.Entity.CarrierAccountId, 'CarrierAccountId');
            $scope.selectedCarriers.splice(index, 1);
            var index1 = UtilsService.getItemIndexByVal($scope.carriers, item.Entity.CarrierAccountId, 'Entity.CarrierAccountId');
            $scope.carriers.splice(index1, 1);
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
        var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

        carrierAccountReadyPromiseDeferred.promise.then(function () {
            var payload = { filter: { AssignableToUserId: selectedAccountManagerId } }
            VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
        });

        return loadCarrierAccountPromiseDeferred.promise;
    }

    function loadAssignedCarriers() {

        return AccountManagerAPIService.GetAssignedCarriersDetail(selectedAccountManagerId, false, CarrierTypeEnum.Exchange.value)
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
                    if (assignedCarriers[i].Entity.RelationType == CarrierTypeEnum.Customer.value) {
                        assignedCarriers[i].customerSwitchValue = true;
                        assignedCarriers[i].newCustomerSwitchValue = true;
                    }
                    else if (assignedCarriers[i].Entity.RelationType == CarrierTypeEnum.Supplier.value) {
                        assignedCarriers[i].supplierSwitchValue = true;
                        assignedCarriers[i].newSupplierSwitchValue = true;
                    }
         
                    $scope.carriers.push(assignedCarriers[i])
        }
    }

    function mapCarriersForAssignment() {
        var mappedCarriers = [];

        for (var i = 0; i < $scope.carriers.length; i++)
           mapCarrierForAssignment($scope.carriers[i], mappedCarriers);

        return mappedCarriers;
    }

    function mapCarrierForAssignment(carrier, mappedCarriers) {
        if (carrier.customerSwitchValue != carrier.newCustomerSwitchValue) {
            var object = {
                UserId: carrier.Entity.UserId,
                CarrierAccountId: carrier.Entity.CarrierAccountId,
                RelationType: CarrierTypeEnum.Customer.value,
                Status: carrier.newCustomerSwitchValue
            };
            mappedCarriers.push(object);
        }
        if (carrier.supplierSwitchValue != carrier.newSupplierSwitchValue) {
            var object = {
                UserId: carrier.Entity.UserId,
                CarrierAccountId: carrier.Entity.CarrierAccountId,
                RelationType: CarrierTypeEnum.Supplier.value,
                Status: carrier.newSupplierSwitchValue
            };
            mappedCarriers.push(object);
          
        }
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

appControllers.controller('WhS_BE_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
