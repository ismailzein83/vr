CarrierAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var mainGridAPI;
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
        //$scope.accountManager = {};
        $scope.carriers = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        }

        $scope.loadMoreData = function () {
            return getData();
        }
        
        $scope.assignCarriers = function () {
            var updatedCarriers = mapCarriersForAssignment();

            if (updatedCarriers.length > 0) {

                AccountManagerAPIService.AssignCarriers(updatedCarriers).then(function (response) {
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
    }

    function load() {
    }

    function getData() {
        $scope.isGettingData = true;

        return UtilsService.waitMultipleAsyncOperations([getCarriers]).then(function () {
            getAssignedCarriers();
            
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isGettingData = false;
        });        
    }

    function getCarriers() {
        var pageInfo = mainGridAPI.getPageInfo();

        return AccountManagerAPIService.GetCarriers(selectedAccountManagerId, pageInfo.fromRow, pageInfo.toRow).then(function (data) {
            angular.forEach(data, function (item) {
                var object = {
                    UserId: selectedAccountManagerId,
                    CarrierAccountId: item.CarrierAccountId,
                    Name: item.Name,
                    NameSuffix: item.NameSuffix,
                    IsCustomerAvailable: item.IsCustomerAvailable,
                    customerSwitchValue: false,
                    newCustomerSwitchValue: false,
                    IsSupplierAvailable: item.IsSupplierAvailable,
                    supplierSwitchValue: false,
                    newSupplierSwitchValue: false
                }
                $scope.carriers.push(object);
            });
        });
    }

    function getAssignedCarriers()
    {
        return AccountManagerAPIService.GetAssignedCarriers(selectedAccountManagerId, false, 0).then(function (response) {
            toggleAssignedCarriers(response);
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
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

    

    function toggleAssignedCarriers(assignedCarriers) {
        for (var i = 0; i < assignedCarriers.length; i++) {
            var carrier = UtilsService.getItemByVal($scope.carriers, assignedCarriers[i].CarrierAccountId, 'CarrierAccountId');

            if (carrier != null) {
                if (assignedCarriers[i].RelationType == 1) {
                    carrier.customerSwitchValue = true;
                    carrier.newCustomerSwitchValue = true;
                }
                else {
                    carrier.supplierSwitchValue = true;
                    carrier.newSupplierSwitchValue = true;
                }
            }
        }
    }
}

appControllers.controller('BusinessEntity_CarrierAssignmentEditorController', CarrierAssignmentEditorController);
