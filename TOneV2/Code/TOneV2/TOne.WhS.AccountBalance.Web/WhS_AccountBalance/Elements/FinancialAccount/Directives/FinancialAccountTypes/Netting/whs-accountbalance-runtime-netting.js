"use strict";

app.directive("whsAccountbalanceRuntimeNetting", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerPostPaid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/FinancialAccountTypes/Netting/Templates/NettingSettings.html"

        };

        function CustomerPostPaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
          
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.customerCreditLimit = 0;
                $scope.scopeModel.supplierCreditLimit = 0;

               
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
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.Netting.NettingSettings ,TOne.WhS.AccountBalance.MainExtensions",
                        CustomerCreditLimit: $scope.scopeModel.customerCreditLimit,
                        SupplierCreditLimit: $scope.scopeModel.supplierCreditLimit
                };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);