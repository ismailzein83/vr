'use strict';

app.directive('retailMultinetAccounttypePartRuntimeCompanyextendedinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/Account/Part/Runtime/Templates/AccountTypePartCompanyExtendedInfoRuntimeTemplate.html'
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
                    $scope.scopeModel.gPSiteID = payload.partSettings.GPSiteID;
                    $scope.scopeModel.excludeWHTaxes = payload.partSettings.ExcludeWHTaxes;
                    $scope.scopeModel.excludeSaleTaxes = payload.partSettings.ExcludeSaleTaxes;
                    $scope.scopeModel.cNICExpiryDate = payload.partSettings.CNICExpiryDate;
                    $scope.scopeModel.assignedNumber = payload.partSettings.AssignedNumber;
                    if (payload.partSettings.CustomerLogo > 0)
                        $scope.scopeModel.customerLogo = {
                            fileId: payload.partSettings.CustomerLogo
                        };
                    else
                        $scope.scopeModel.customerLogo = null;
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
                    $type: 'Retail.MultiNet.Business.MultiNetCompanyExtendedInfo, Retail.MultiNet.Business',
                    CNIC: $scope.scopeModel.cNIC,
                    NTN: $scope.scopeModel.nTN,
                    GPSiteID: $scope.scopeModel.gPSiteID,
                    AccountType: accountTypeAPI.getSelectedIds(),
                    ExcludeWHTaxes: $scope.scopeModel.excludeWHTaxes,
                    ExcludeSaleTaxes: $scope.scopeModel.excludeSaleTaxes,
                    CustomerLogo: ($scope.scopeModel.customerLogo != null) ? $scope.scopeModel.customerLogo.fileId : 0,
                    CNICExpiryDate: $scope.scopeModel.cNICExpiryDate,
                    AssignedNumber: $scope.scopeModel.assignedNumber

                };

            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);