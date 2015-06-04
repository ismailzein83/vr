CarrierAccountEditorController.$inject = ['$scope', 'BusinessEntityAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAccountEditorController($scope, BusinessEntityAPIService, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.CarrierAccountId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.CarrierAccountId = parameters.carrierAccountId;
        console.log(parameters);
        editMode = true;
    }
    function defineScope() {

        $scope.selectedRoutingStatus = [];
        $scope.selectedCustomerPaymentType = [];
        $scope.selectedSupplierPaymentType = [];
        $scope.saveCarrierAccount = function () {
            return updateCarrierAccount();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        defineActivationStatusChoicesGroup();
        defineAccountTypeChoicesGroup();
        defineRoutingStatusSelectGroup();
        defineCustomerPaymentTypeSelectGroup();
        defineSupplierPaymentTypeSelectGroup();
    }

    function load() {
        $scope.isGettingData = true;
        getCarrierAccount().finally(function () {
            $scope.isGettingData = false;
        })
    }
    function defineActivationStatusChoicesGroup() {
        $scope.choicesActivationStatusGroup = [
            { title: "InActive", value: 0 },
            { title: "Test", value: 1 },
            { title: "Active", value: 2 }
        ];

        $scope.choicesActivationStatusReady = function (api) {
            $scope.choicesActivationStatusAPI = api;
        }
    }
    function defineAccountTypeChoicesGroup() {
        $scope.choicesAccountTypeGroup = [
            { title: "Client", value: 0 },
            { title: "Exchange", value: 1 },
            { title: "Termination", value: 2 }
        ];

        $scope.choicesAccountTypeReady = function (api) {
            $scope.choicesAccountTypeAPI = api;
        }
    }
    function defineRoutingStatusSelectGroup() {
        $scope.routingStatus = [
            { title: "Blocked", value: 0 },
            { title: "Blocked In Bound", value: 1 },
            { title: "Blocked Out Bound", value: 2 },
            { title: "Enabled", value: 3 }
        ];
    }
    function defineCustomerPaymentTypeSelectGroup() {
        $scope.customerPaymentType = [
            { title: "PostPaid", value: 0 },
            { title: "PrePaid", value: 1 },
            { title: "Undefined", value: 100 },
            { title: "Defined By Profile", value: 200 }
        ];
    }
    function defineSupplierPaymentTypeSelectGroup() {
        $scope.supplierPaymentType = [
            { title: "PostPaid", value: 0 },
            { title: "PrePaid", value: 1 },
            { title: "Undefined", value: 100 },
            { title: "Defined By Profile", value: 200 }
        ];
    }
    function getCarrierAccount() {
        return BusinessEntityAPIService.GetCarrierAccount($scope.CarrierAccountId)
           .then(function (response) {
               fillScopeFromCarrierAccountObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }
    function updateCarrierAccount() {
        var carrierAccountObject = buildCarrierAccountObjFromScope();
        BusinessEntityAPIService.UpdateCarrierAccount(buildCarrierAccountObjFromScope())
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("CarrierAccount", response)) {
                if ($scope.onCarrierAccountUpdated != undefined)
                    $scope.onCarrierAccountUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }
    function buildCarrierAccountObjFromScope() {
        return {
            CarrierAccountId: ($scope.CarrierAccountId != null) ? $scope.CarrierAccountId : '',
            ProfileId: $scope.ProfileId,
            ProfileName: $scope.ProfileName,
            ProfileCompanyName: $scope.ProfileCompanyName,
            ActivationStatus: $scope.choicesActivationStatusGroup[$scope.selectedActivationStatusChoiceIndex].value,
            RoutingStatus: $scope.selectedRoutingStatus.value,
            AccountType: $scope.choicesAccountTypeGroup[$scope.selectedAccountTypeChoiceIndex].value,
            CustomerPaymentType: $scope.selectedCustomerPaymentType.value,
            SupplierPaymentType: $scope.selectedSupplierPaymentType.value,
            NameSuffix: $scope.NameSuffix
        };

    }

    function fillScopeFromCarrierAccountObj(CarrierAccountObject) {
        $scope.CarrierAccountId = CarrierAccountObject.CarrierAccountId;
        $scope.ProfileId = CarrierAccountObject.ProfileId;
        $scope.ProfileName = CarrierAccountObject.ProfileName;
        $scope.ProfileCompanyName = CarrierAccountObject.ProfileCompanyName;
        $scope.ActivationStatus = CarrierAccountObject.ActivationStatus;
        $scope.choicesActivationStatusAPI.selectChoice(UtilsService.getItemIndexByVal($scope.choicesActivationStatusGroup, $scope.ActivationStatus, 'value'));
        $scope.selectedRoutingStatus = $scope.routingStatus[UtilsService.getItemIndexByVal($scope.routingStatus, CarrierAccountObject.RoutingStatus, 'value')];
        $scope.AccountType = CarrierAccountObject.AccountType;
        $scope.choicesAccountTypeAPI.selectChoice(UtilsService.getItemIndexByVal($scope.choicesAccountTypeGroup, $scope.AccountType, 'value'));
        $scope.CustomerPaymentType = CarrierAccountObject.CustomerPaymentType;
        $scope.selectedCustomerPaymentType = $scope.customerPaymentType[UtilsService.getItemIndexByVal($scope.customerPaymentType, CarrierAccountObject.CustomerPaymentType, 'value')];
        $scope.SupplierPaymentType = CarrierAccountObject.SupplierPaymentType;
        $scope.selectedSupplierPaymentType = $scope.supplierPaymentType[UtilsService.getItemIndexByVal($scope.supplierPaymentType, CarrierAccountObject.SupplierPaymentType, 'value')];
        $scope.NameSuffix = CarrierAccountObject.NameSuffix;
    }

}
appControllers.controller('BusinessEntity_CarrierAccountEditorController', CarrierAccountEditorController);