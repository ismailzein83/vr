CarrierAssignmentEditorController.$inject = ['$scope', 'AccountManagerAPIService', 'VRModalService', 'VRNavigationService', 'UtilsService'];

function CarrierAssignmentEditorController($scope, AccountManagerAPIService, VRModalService, VRNavigationService, UtilsService) {
    var gridApi;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountManager = parameters.accountManager;
        }
    }

    function defineScope() {
        $scope.accountManager = {};
        $scope.carriers = [];

        $scope.onGridReady = function (api) {
            gridApi = api;
            getData();
        }
        $scope.loadMoreData = function () {
            return getData();
        }
        function getData() {
            UtilsService.waitMultipleAsyncOperations([loadParameters]).finally(function () {
                // $scope.accountManager.nodeId is set
                UtilsService.waitMultipleAsyncOperations([getCarriers]).finally(function () {
                    // $scope.carriers is set
                    AccountManagerAPIService.GetAssignedCarriers($scope.accountManager.nodeId).then(function (data) {
                        toggleAssignedCarriers(data);
                    });
                });
            });
        }
        $scope.assignCarriers = function () {
            var updatedCarriers = mapCarriersForAssignment();

            AccountManagerAPIService.AssignCarriers(updatedCarriers).then(function () {
                $scope.modalContext.closeModal();
            });
        }
        $scope.closeModal = function () {
            $scope.modalContext.closeModal();
        }
    }

    function load() {
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

    function getCarriers() {
        var pageInfo = gridApi.getPageInfo();

        return AccountManagerAPIService.GetCarriers(pageInfo.fromRow, pageInfo.toRow).then(function (data) {
            angular.forEach(data, function (item) {
                var object = {
                    UserId: $scope.accountManager.nodeId,
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
