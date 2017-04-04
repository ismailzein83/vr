"use strict";

app.directive("whsInvoiceInvoiceaccountSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'WhS_Invoice_InvoiceAccountService', 'WhS_Invoice_InvoiceAccountAPIService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, WhS_Invoice_InvoiceAccountService, WhS_Invoice_InvoiceAccountAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var invoiceAccountSearch = new InvoiceAccountSearch($scope, ctrl, $attrs);
            invoiceAccountSearch.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Invoice/Directives/InvoiceAccount/Templates/InvoiceAccountSearch.html"

    };

    function InvoiceAccountSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var carrierAccountId;
        var carrierProfileId;
        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addInvoiceAccount = function () {
                var onInvoiceAccountAdded = function (obj) {
                    if (context != undefined) {
                        context.checkAllowAddInvoiceAccount();
                    }
                    gridAPI.onInvoiceAccountAdded(obj);
                };
                WhS_Invoice_InvoiceAccountService.addInvoiceAccount(carrierAccountId, carrierProfileId, onInvoiceAccountAdded)
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            defineContext();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined)
                {
                    carrierAccountId = payload.carrierAccountId;
                    carrierProfileId = payload.carrierProfileId;
                }
                var promises = [];
                promises.push(checkAllowAddInvoiceAccount());
                promises.push(gridAPI.loadGrid(getGridQuery()));

                return UtilsService.waitMultiplePromises(promises);
               
            };
            
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
        function checkAllowAddInvoiceAccount() {
            return WhS_Invoice_InvoiceAccountAPIService.CheckCarrierAllowAddInvoiceAccounts(carrierProfileId, carrierAccountId).then(function (response) {
                $scope.scopeModel.showAddButton = response;
            });
        }
        function getGridQuery() {

            var filter = {
                query:{
                    CarrierAccountId: carrierAccountId,
                    CarrierProfileId: carrierProfileId
                },
                context: context
            };
            return filter;
        }
        function defineContext()
        {
            context = {
                checkAllowAddInvoiceAccount: function () {
                    $scope.scopeModel.isLoading = true;
                    return checkAllowAddInvoiceAccount().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            };
        }
    }

    return directiveDefinitionObject;

}]);
