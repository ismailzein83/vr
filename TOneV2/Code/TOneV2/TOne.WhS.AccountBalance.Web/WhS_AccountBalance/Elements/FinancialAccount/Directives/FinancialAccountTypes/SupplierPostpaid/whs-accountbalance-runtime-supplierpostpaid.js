"use strict";

app.directive("whsAccountbalanceRuntimeSupplierpostpaid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/FinancialAccountTypes/SupplierPostpaid/Templates/SupplierPostpaidSettings.html"

        };

        function SupplierPostpaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
       
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.creditLimit = 0;

             
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPostpaid.SupplierPostpaidSettings ,TOne.WhS.AccountBalance.MainExtensions",
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