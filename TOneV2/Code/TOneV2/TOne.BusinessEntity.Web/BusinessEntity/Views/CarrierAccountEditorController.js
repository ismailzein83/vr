CarrierAccountEditorController.$inject = ['$scope', 'CarrierAccountAPIService', 'RoutingStatusEnum', 'AccountTypeEnum', 'ActivationStatusEnum', 'PaymentTypeEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function CarrierAccountEditorController($scope, CarrierAccountAPIService, RoutingStatusEnum, AccountTypeEnum, ActivationStatusEnum, PaymentTypeEnum, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
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
        $scope.ActivationStatus = [];
        for (var td in ActivationStatusEnum)
            $scope.ActivationStatus.push(ActivationStatusEnum[td]);
    }
    function defineAccountTypeChoicesGroup() {
        $scope.AccountType = [];
        for (var td in AccountTypeEnum)
            $scope.AccountType.push(AccountTypeEnum[td]);
    }
    function defineRoutingStatusSelectGroup() {
        $scope.routingStatus = [];
        for (var td in RoutingStatusEnum)
            $scope.routingStatus.push(RoutingStatusEnum[td]);
    }
    function defineCustomerPaymentTypeSelectGroup() {
        $scope.customerPaymentType = [];
        for (var td in PaymentTypeEnum)
            $scope.customerPaymentType.push(PaymentTypeEnum[td]);
    }
    function defineSupplierPaymentTypeSelectGroup() {
        $scope.supplierPaymentType = [];
        for (var td in PaymentTypeEnum)
            $scope.supplierPaymentType.push(PaymentTypeEnum[td]);
    }
    function getCarrierAccount() {
        return CarrierAccountAPIService.GetCarrierAccount($scope.CarrierAccountId)
           .then(function (response) {
               fillScopeFromCarrierAccountObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }
    function updateCarrierAccount() {
        var carrierAccountObject = buildCarrierAccountObjFromScope();
        CarrierAccountAPIService.UpdateCarrierAccount(buildCarrierAccountObjFromScope())
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
            ActivationStatus: $scope.selectedActivationStatus.value,
            RoutingStatus: $scope.selectedRoutingStatus.value,
            AccountType: $scope.SelectedAccountType.value,
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
        $scope.selectedActivationStatus = $scope.ActivationStatus[UtilsService.getItemIndexByVal($scope.ActivationStatus, CarrierAccountObject.ActivationStatus, 'value')];
        $scope.selectedRoutingStatus = $scope.routingStatus[UtilsService.getItemIndexByVal($scope.routingStatus, CarrierAccountObject.RoutingStatus, 'value')];
        $scope.selectedAccountType = $scope.AccountType[UtilsService.getItemIndexByVal($scope.AccountType, CarrierAccountObject.AccountType, 'value')];
        $scope.selectedCustomerPaymentType = $scope.customerPaymentType[UtilsService.getItemIndexByVal($scope.customerPaymentType, CarrierAccountObject.CustomerPaymentType, 'value')];
        $scope.selectedSupplierPaymentType = $scope.supplierPaymentType[UtilsService.getItemIndexByVal($scope.supplierPaymentType, CarrierAccountObject.SupplierPaymentType, 'value')];
        $scope.NameSuffix = CarrierAccountObject.NameSuffix;
    }
}

appControllers.controller('Carrier_CarrierAccountEditorController', CarrierAccountEditorController);