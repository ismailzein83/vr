'use strict';

app.directive('retailMultinetAccounttypePartRuntimeBranchextendedinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new BranchExtendedInfoRuntime($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/Account/Part/Runtime/Templates/AccountTypePartBranchExtendedInfoRuntimeTemplate.html'
    };

    function BranchExtendedInfoRuntime($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountTypeAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};

        $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
            accountTypeAPI = api;
            accountTypeSelectorReadyDeferred.resolve();
        };
       
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];

                if (payload != undefined && payload.partSettings != undefined) {
                    $scope.scopeModel.branchCode = payload.partSettings.BranchCode;
                    $scope.scopeModel.contractReferenceNumber = payload.partSettings.ContractReferenceNumber;
                    $scope.scopeModel.cNIC = payload.partSettings.CNIC;
                    $scope.scopeModel.nTN = payload.partSettings.NTN;
                    $scope.scopeModel.registrationNumber = payload.partSettings.RegistrationNumber;
                    $scope.scopeModel.refNumber = payload.partSettings.RefNumber;
                    $scope.scopeModel.passportNumber = payload.partSettings.PassportNumber;
                    $scope.scopeModel.assignedNumber = payload.partSettings.AssignedNumber;
                    $scope.scopeModel.pIN = payload.partSettings.PIN;
                    $scope.scopeModel.billingAddress = payload.partSettings.BillingAddress;
                    $scope.scopeModel.technicalAddress = payload.partSettings.TechnicalAddress;
                    $scope.scopeModel.officeAddress = payload.partSettings.OfficeAddress;
                    $scope.scopeModel.homeAddress = payload.partSettings.HomeAddress;
                    $scope.scopeModel.cNICExpiryDate = payload.partSettings.CNICExpiryDate;
                }
                promises.push(loadAccountTypeSelector());

                function loadAccountTypeSelector() {
                    var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    accountTypeSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            selectedIds: payload != undefined && payload.partSettings != undefined ? payload.partSettings.AccountType : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(accountTypeAPI, selectorPayload, accountSelectorLoadDeferred);
                    });
                    return accountSelectorLoadDeferred.promise;
                };


                return UtilsService.waitMultiplePromises(promises);

            };
            api.getData = function () {
                return {
                    $type: 'Retail.MultiNet.Business.MultiNetBranchExtendedInfo, Retail.MultiNet.Business',
                    BranchCode: $scope.scopeModel.branchCode,
                    ContractReferenceNumber: $scope.scopeModel.contractReferenceNumber,
                    CNIC: $scope.scopeModel.cNIC,
                    NTN: $scope.scopeModel.nTN,
                    RegistrationNumber: $scope.scopeModel.registrationNumber,
                    RefNumber: $scope.scopeModel.refNumber,
                    PassportNumber: $scope.scopeModel.passportNumber,
                    AssignedNumber: $scope.scopeModel.assignedNumber,
                    PIN: $scope.scopeModel.pIN,
                    BillingAddress: $scope.scopeModel.billingAddress,
                    TechnicalAddress: $scope.scopeModel.technicalAddress,
                    OfficeAddress: $scope.scopeModel.officeAddress,
                    HomeAddress: $scope.scopeModel.homeAddress,
                    AccountType: accountTypeAPI.getSelectedIds(),
                    CNICExpiryDate: $scope.scopeModel.cNICExpiryDate
                }
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);