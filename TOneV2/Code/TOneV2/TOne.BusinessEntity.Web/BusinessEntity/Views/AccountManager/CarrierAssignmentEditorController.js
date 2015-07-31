CarrierAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var gridApi;
    var selectedAccountManagerId = undefined;
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
        
        $scope.assignCarriers = function () {
            var updatedCarriers = mapCarriersForAssignment();

            if (updatedCarriers.length > 0) {

                AccountManagerAPIService.AssignCarriers(updatedCarriers)
                    .then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Assigned Carriers", response)) {
                            if ($scope.onCarriersAssigned != undefined)
                                $scope.onCarriersAssigned();

                            $scope.modalContext.closeModal();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            }
        }

        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            $scope.isGettingData = true;

            return AccountManagerAPIService.GetCarriers(dataRetrievalInput)
                .then(function (response) {

                    response.Data = getMappedCarriers(response.Data);

                    return AccountManagerAPIService.GetAssignedCarriers(selectedAccountManagerId, false, 0)
                        .then(function (assignedCarriers) {
                            toggleAssignedCarriers(response.Data, assignedCarriers);
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        })
                        .finally(function () {
                            $scope.isGettingData = false;
                        });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }
    }

    function load() {
    }

    function retrieveData() {
        var query = {
            UserId: selectedAccountManagerId
        };

        return gridApi.retrieveData(query);
    }

    function getMappedCarriers(carriers) {
        var mappedCarriers = [];

        angular.forEach(carriers, function (carrier) {
            var object = {
                UserId: selectedAccountManagerId,
                CarrierAccountId: carrier.CarrierAccountId,
                Name: carrier.Name,
                NameSuffix: carrier.NameSuffix,
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

    function mapCarriersForAssignment() {
        var mappedCarriers = [];

        for (var i = 0; i < $scope.carriers.length; i++)
            mappedCarriers = mappedCarriers.concat(mapCarrierForAssignment($scope.carriers[i]));

        return mappedCarriers;
    }

    function mapCarrierForAssignment(carrier) {
        var mappedCarrier = [];

        if (carrier.customerSwitchValue != carrier.newCustomerSwitchValue) {
            var object = {
                UserId: carrier.UserId,
                CarrierAccountId: carrier.CarrierAccountId,
                RelationType: 1,
                Status: carrier.newCustomerSwitchValue
            };
            mappedCarrier.push(object);
        }

        if (carrier.supplierSwitchValue != carrier.newSupplierSwitchValue) {
            var object = {
                UserId: carrier.UserId,
                CarrierAccountId: carrier.CarrierAccountId,
                RelationType: 2,
                Status: carrier.newSupplierSwitchValue
            };
            mappedCarrier.push(object);
        }

        return mappedCarrier;
    }

    function toggleAssignedCarriers(carriers, assignedCarriers) {

        for (var i = 0; i < assignedCarriers.length; i++) {
            var carrier = UtilsService.getItemByVal(carriers, assignedCarriers[i].CarrierAccountId, 'CarrierAccountId');

            if (carrier != null) {
                if (assignedCarriers[i].RelationType == 1) {
                    carrier.customerSwitchValue = true;
                    carrier.newCustomerSwitchValue = true;
                }
                else if (assignedCarriers[i].RelationType == 2) {
                    carrier.supplierSwitchValue = true;
                    carrier.newSupplierSwitchValue = true;
                }
            }
        }
    }
}

appControllers.controller('BusinessEntity_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
