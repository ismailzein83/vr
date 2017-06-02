'use strict';

app.directive('retailMultinetAccounttypePartRuntimeCompanyextendedinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/MainExtensions/Account/Part/Runtime/Templates/AccountTypePartCompanyExtendedInfoRuntimeTemplate.html'
    };

    function AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var addressTypeAPI;
        var addressTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        var accountTypeAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var genderAPI;
        var genderSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};

        $scope.scopeModel.onAddressTypeSelectorReady = function (api) {
            addressTypeAPI = api;
            addressTypeSelectorReadyDeferred.resolve();
        };

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
                    $scope.scopeModel.cNIC = payload.partSettings.CNIC;
                    $scope.scopeModel.nTN = payload.partSettings.NTN;
                    $scope.scopeModel.passportNumber = payload.partSettings.PassportNumber;
                    $scope.scopeModel.assignedNumber = payload.partSettings.AssignedNumber;
                    $scope.scopeModel.inventoryDetails = payload.partSettings.InventoryDetails;
                    $scope.scopeModel.gPSiteID = payload.partSettings.GPSiteID;
                }

                promises.push(loadAddressTypeSelector());
                promises.push(loadAccountTypeSelector());

                function loadAddressTypeSelector() {
                    var addressSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    addressTypeSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            selectedIds: payload != undefined && payload.partSettings != undefined && payload.partSettings.AddressType || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(addressTypeAPI, selectorPayload, addressSelectorLoadDeferred);
                    });
                    return addressSelectorLoadDeferred.promise;
                };

                function loadAccountTypeSelector() {
                    var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    accountTypeSelectorReadyDeferred.promise.then(function () {
                        var selectorPayload = {
                            selectedIds: payload != undefined && payload.partSettings != undefined && payload.partSettings.AccountType || undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(accountTypeAPI, selectorPayload, accountSelectorLoadDeferred);
                    });
                    return accountSelectorLoadDeferred.promise;
                };


                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                return {
                    $type: 'Retail.MultiNet.MainExtensions.MultiNetCompanyExtendedInfo, Retail.MultiNet.MainExtensions',
                    CNIC: $scope.scopeModel.cNIC,
                    NTN: $scope.scopeModel.nTN,
                    PassportNumber: $scope.scopeModel.passportNumber,
                    AssignedNumber: $scope.scopeModel.assignedNumber,
                    AddressType: addressTypeAPI.getSelectedIds(),
                    InventoryDetails: $scope.scopeModel.inventoryDetails,
                    GPSiteID: $scope.scopeModel.gPSiteID,
                    AccountType: accountTypeAPI.getSelectedIds()
                }

            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);