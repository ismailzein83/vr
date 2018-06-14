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

                if (payload != undefined) {
                    partDefinition = payload.partDefinition;
                    partSettings = payload.partSettings;
                }

                if (partDefinition != undefined && partDefinition.Settings != undefined)
                    getInvoiceTypesTaxes(getInvoiceTypesIds());
                
                function getInvoiceTypesIds() {
                    var invoiceTypesIds = [];
                    if (partDefinition.Settings.InvoiceTypes != undefined) {
                        var invoiceTypes = partDefinition.Settings.InvoiceTypes;
                        for (var i = 0; i < invoiceTypes.length; i++)
                            invoiceTypesIds.push(invoiceTypes[i].InvoiceTypeId);
                    }
                    return invoiceTypesIds;
                }

                function getInvoiceTypesTaxes(invoiceTypesIds) {
                    var input = { InvoiceTypesIds: invoiceTypesIds };
                    return Retail_BE_AccountPartTaxesRuntimeAPIService.GetInvoiceTypesTaxesRuntime(input).then(function (response) {
                        if (response != undefined) {
                            for (var key in response) {
                                if (key != "$type") {
                                    var objectValue = response[key];
                                    var taxDetail = {
                                        InvoiceTypeId:key,
                                        InvoiceTypeTitle: objectValue.InvoiceTypeTitle,
                                        Taxes: [],
                                    };
                                    var invoiceTypeSettingsValues;
                                    if (partSettings != undefined && partSettings.InvoiceTypes != undefined) {
                                        invoiceTypeSettingsValues = UtilsService.getItemByVal(partSettings.InvoiceTypes, key, 'InvoiceTypeId');
                                        if (invoiceTypeSettingsValues != undefined && invoiceTypeSettingsValues.VRTaxSetting != undefined)
                                        {
                                            taxDetail.vatPercentage =invoiceTypeSettingsValues.VRTaxSetting.VAT ;
                                            taxDetail.vatId = invoiceTypeSettingsValues.VRTaxSetting.VATId;
                                        }
                                    }
                                    if (objectValue.TaxesDefinitions != undefined && objectValue.TaxesDefinitions.ItemDefinitions != undefined) {
                                        for (var i = 0; i < objectValue.TaxesDefinitions.ItemDefinitions.length; i++) {
                                            var itemDefinition = objectValue.TaxesDefinitions.ItemDefinitions[i];
                                            var tax = {
                                                ItemId: itemDefinition.ItemId,
                                                Title: itemDefinition.Title,
                                            };
                                            if (invoiceTypeSettingsValues != undefined && invoiceTypeSettingsValues.VRTaxSetting != undefined && invoiceTypeSettingsValues.VRTaxSetting.Items != undefined) {
                                                var itemObject = UtilsService.getItemByVal(invoiceTypeSettingsValues.VRTaxSetting.Items, itemDefinition.ItemId, 'ItemId');
                                                if (itemObject != undefined)
                                                    tax.Value = itemObject.Value;
                                            }

                                            taxDetail.Taxes.push(tax);
                                        }
                                    }
                                    ctrl.invoiceTypesTaxes.push(taxDetail);
                                }
                            }
                        }
                    });
                }
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartTaxesSettings,Retail.BusinessEntity.MainExtensions',
                    InvoiceTypes: getInvoiceTypesTaxesData()
                };
            };

            function getInvoiceTypesTaxesData() {
                
                var invoiceTaxes = [];
                for (var i = 0; i < ctrl.invoiceTypesTaxes.length; i++) {
                    var invoiceType = ctrl.invoiceTypesTaxes[i];
                    var taxItems = [];
                    if (invoiceType != undefined && invoiceType.Taxes != undefined) {
                        for (var j = 0; j < invoiceType.Taxes.length; j++) {
                            var tax = invoiceType.Taxes[j];
                            if (tax.Value != undefined)
                            {
                                taxItems.push({
                                    ItemId: tax.ItemId,
                                    Value: tax.Value
                                });
                            }
                           
                        }
                    }
                    var taxInvoiceTypeSetting = {
                        InvoiceTypeId: invoiceType.InvoiceTypeId,
                        VRTaxSetting: {
                            Items: taxItems,
                            VAT: invoiceType.vatPercentage,
                            VATId: invoiceType.vatId
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