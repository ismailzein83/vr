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
                            for (var p in response) {
                                if (p != "$type") {
                                    var taxDetail = {
                                        InvoiceTypeTitle: response[p].InvoiceTypeTitle,
                                        Taxes: response[p].TaxesDefinitions.ItemDefinitions
                                    };
                                    ctrl.invoiceTypesTaxes.push(taxDetail);
                                }
                            }
                        }
                        if (partSettings != undefined) {
                          if (partSettings.InvoiceTypes != undefined) {
                              for (var i = 0; i < partSettings.InvoiceTypes.length; i++) {
                                  var invoiceTypes = partSettings.InvoiceTypes[i];
                                  if (invoiceTypes != undefined && invoiceTypes.VRTaxSetting != undefined) {
                                      $scope.scopeModel.vatPercentage = invoiceTypes.VRTaxSetting.VAT;
                                      $scope.scopeModel.vatId = invoiceTypes.VRTaxSetting.VATId;
                                      if (invoiceTypes.VRTaxSetting.Items != undefined) {
                                          for (var j = 0; j < invoiceTypes.VRTaxSetting.Items.length; j++) {
                                              ctrl.invoiceTypesTaxes[i].Taxes[j].Value = invoiceTypes.VRTaxSetting.Items[j].Value;
                                            }
                                        }
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
                    InvoiceTypes: getInvoiceTypesTaxesData()
                };
            };

            function getInvoiceTypesTaxesData() {
                
                var invoiceTaxes = [];
                var taxItems = [];
                for (var i = 0; i < ctrl.invoiceTypesTaxes.length; i++) {
                    var invoiceTypes = ctrl.invoiceTypesTaxes[i];
                    if (invoiceTypes != undefined && invoiceTypes.TaxesDefinitions != undefined && invoiceTypes.TaxesDefinitions.ItemDefinitions != undefined) {
                        var taxesDefinition = invoiceTypes.TaxesDefinitions.ItemDefinitions;
                        for (var j = 0; j < taxesDefinition.length; j++) {
                            taxItems.push({
                                ItemId: taxesDefinition[j].ItemId,
                                Value: taxesDefinition[j].Value
                            });
                        }
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