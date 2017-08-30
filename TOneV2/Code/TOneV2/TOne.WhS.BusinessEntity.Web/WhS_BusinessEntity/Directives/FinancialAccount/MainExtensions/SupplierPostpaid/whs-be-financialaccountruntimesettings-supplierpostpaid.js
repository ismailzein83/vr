"use strict";

app.directive("whsBeFinancialaccountruntimesettingsSupplierpostpaid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_FinancialAccountAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_FinancialAccountAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierPostpaid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/FinancialAccount/MainExtensions/SupplierPostpaid/Templates/SupplierPostpaidSettings.html"

        };

        function SupplierPostpaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var carrierAccountId;
            var carrierProfileId;
            function initializeController() {
                $scope.scopeModel = {};
            
             
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                        carrierAccountId = payload.carrierAccountId;
                        carrierProfileId = payload.carrierProfileId;
                        if (extendedSettings != undefined) {
                            $scope.scopeModel.creditLimit = extendedSettings.CreditLimit;
                        }
                    }
                    var promises = [];
                    promises.push(loadCurrencyName());
                    function loadCurrencyName() {
                        return WhS_BE_FinancialAccountAPIService.GetAccountCurrencyName(carrierProfileId, carrierAccountId).then(function (response) {
                            $scope.scopeModel.currencyName = response;
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPostpaid.SupplierPostpaidSettings ,TOne.WhS.BusinessEntity.MainExtensions",
                        CreditLimit: $scope.scopeModel.creditLimit
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);