'use strict';

app.directive('retailBeAccounttypePartDefinitionTaxes', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeTaxesPartDefinition = new AccountTypeTaxesPartDefinition($scope, ctrl, $attrs);
            accountTypeTaxesPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeTaxesDefinitionTemplate.html'
    };

    function AccountTypeTaxesPartDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                var selectedInvoiceTypes = [];
                var partDefinitionSettings;

                if (payload != undefined)
                    partDefinitionSettings = payload.partDefinitionSettings;

                promises.push(loadInvoiceTypeSelector());

                function loadInvoiceTypeSelector() {
                    var invoiceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var invoiceTypeSelectorPayload;

                        if (partDefinitionSettings != undefined) {
                            for (var i = 0; i < partDefinitionSettings.InvoiceTypes.length; i++)
                                selectedInvoiceTypes.push(partDefinitionSettings.InvoiceTypes[i].InvoiceTypeId);
                            invoiceTypeSelectorPayload = {
                                selectedIds: selectedInvoiceTypes
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorLoadDeferred);
                    });
                    return invoiceTypeSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartTaxesSettingsDefinition,Retail.BusinessEntity.MainExtensions',
                    InvoiceTypes: getInvoiceTypes()
                };
            };

            function getInvoiceTypes() {
                var invoiceTypes = [];
                var invoices = invoiceTypeSelectorAPI.getSelectedIds();
                for (var i = 0; i < invoices.length ; i++) {
                    invoiceTypes.push({ InvoiceTypeId: invoices [i]});
                }
                return invoiceTypes;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);