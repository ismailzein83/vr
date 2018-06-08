'use strict';

app.directive('retailBeAccounttypePartRuntimeTaxes', ['UtilsService', 'Retail_BE_AccountPartTaxesRuntimeAPIService', function (UtilsService, Retail_BE_AccountPartTaxesRuntimeAPIService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeTaxesPartRuntime = new AccountTypeTaxesPartRuntime($scope, ctrl, $attrs);
            accountTypeTaxesPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeTaxesPartRuntimeTemplate.html'
    };

    function AccountTypeTaxesPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {
           
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var partDefinition;
                ctrl.invoiceTypesTaxes = [];

                if (payload != undefined) 
                    partDefinition = payload.partDefinition;
                
                if (partDefinition != undefined && partDefinition.Settings != undefined)
                    getInvoiceTypesTaxes(getInvoiceTypesIds());

                function getInvoiceTypesIds() {
                    var invoiceTypesIds = [];
                    var invoiceTypes = partDefinition.Settings.InvoiceTypes;
                    for (var i = 0; i < invoiceTypes.length; i++)
                        invoiceTypesIds.push(invoiceTypes[i].InvoiceTypeId);
                    return invoiceTypesIds;
                }

                function getInvoiceTypesTaxes(invoiceTypesIds) {
                    return Retail_BE_AccountPartTaxesRuntimeAPIService.GetInvoiceTypesTaxesRuntime(invoiceTypesIds).then(function (response) {
                        angular.forEach(response, function (item) {
                            if (item.$type)
                                ctrl.invoiceTypesTaxes.push(item);
                        });
                    });
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartTaxesSettings,Retail.BusinessEntity.MainExtensions',
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);