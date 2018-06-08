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

        ctrl.invoiceTypesTaxes = [];

        function initializeController() {
            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var partDefinition;
                var partSettings;

                $scope.scopeModel.taxItems = [];
                
                if (payload != undefined) {
                    partDefinition = payload.partDefinition;
                    partSettings = payload.partSettings;
                }

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
                            if (item.$type) {
                                ctrl.invoiceTypesTaxes.push(item);
                            }
                        });

                        if (partSettings != undefined) {
                            for (var i = 0; i < partSettings.InviceTypes.length; i++) {
                                if (partSettings.InviceTypes[i].VRTaxSetting != undefined) {
                                    $scope.scopeModel.vatPercentage = partSettings.InviceTypes[i].VRTaxSetting.VAT;
                                    $scope.scopeModel.vatId = partSettings.InviceTypes[i].VRTaxSetting.VATId;
                                    for (var j = 0; j < partSettings.InviceTypes[i].VRTaxSetting.Items.length; j++) {
                                        var taxItem = {
                                            ItemId: partSettings.InviceTypes[i].VRTaxSetting.Items[j].ItemId,
                                            Title: ctrl.invoiceTypesTaxes[i].TaxesDefinitions.ItemDefinitions[j].Title,
                                            Value: partSettings.InviceTypes[i].VRTaxSetting.Items[j].Value
                                        };
                                        $scope.scopeModel.taxItems.push(taxItem);
                                    }
                                }
                            }
                        }
                    });
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartTaxesSettings,Retail.BusinessEntity.MainExtensions',
                    InviceTypes: getInvoiceTypesTaxesData()
                };
            };

            function getInvoiceTypesTaxesData() {
                
                var invoiceTaxes = [];
                for (var i = 0; i < ctrl.invoiceTypesTaxes.length; i++) {
                    var taxesDefinition = ctrl.invoiceTypesTaxes[i].TaxesDefinitions.ItemDefinitions;
                    var taxItems = [];
                    for (var j = 0; j < taxesDefinition.length; j++) {
                        taxItems.push({
                            ItemId: taxesDefinition[j].ItemId,
                            Value: $scope.scopeModel.taxItems[j].Value
                        });
                    }

                    var taxInvoiceTypeSetting = {
                        InvoiceTypeId: ctrl.invoiceTypesTaxes[i].InvoiceTypeId,
                        VRTaxSetting: {
                            Items: taxItems,
                            VAT: $scope.scopeModel.vatPercentage,
                            VATId: $scope.scopeModel.vatId
                        }
                    };
                    invoiceTaxes.push(taxInvoiceTypeSetting);
                }
                return invoiceTaxes;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);