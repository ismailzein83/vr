CarrierAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var gridApi;
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
        
        $scope.assignCarriers = function () {
            $scope.issaving = true;

            var updatedCarriers = mapCarriersForAssignment();

            if (updatedCarriers.length > 0) {

                AccountManagerAPIService.AssignCarriers(updatedCarriers)
                    .then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("Assigned Carriers", response)) {
                            if ($scope.onCarriersAssigned != undefined)
                                $scope.onCarriersAssigned();

                            $scope.modalContext.closeModal();
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    })
                    .finally(function () {
                        $scope.issaving = false;
                    });
            }
        }

        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }

        $scope.gridReady = function (api) {
            gridApi = api;

            $scope.isGettingData = true;

            AccountManagerAPIService.GetAssignedCarriers(selectedAccountManagerId, false, 0).then(function (response) {
                assignedCarriers = response;
                return retrieveData();
            })
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
           .finally(function () {
               $scope.isGettingData = false;
           });
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return AccountManagerAPIService.GetCarriers(dataRetrievalInput)
                .then(function (response) {
                    carriers = getMappedCarriers(response.Data);
                    toggleAssignedCarriers(carriers);
                    response.Data = carriers;
                    onResponseReady(response);
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

    function toggleAssignedCarriers(carriers) {

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
