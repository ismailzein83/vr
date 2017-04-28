'use strict';

app.directive('retailBeFinancialaccountdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new financialAccountDefinitionSettingsnCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountDefinitionSettings.html"
        };

        function financialAccountDefinitionSettingsnCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorAPI = api;
                    invoiceTypeSelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([invoiceTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                })
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var financialAccountDefinitionSettings;
                    if (payload != undefined) {
                        financialAccountDefinitionSettings = payload.componentType;

                        if (financialAccountDefinitionSettings != undefined) {
                            $scope.scopeModel.name = financialAccountDefinitionSettings.Name;
                        }
                    }

                    var promises = [];

                    function loadInvoiceTypeSelector() {
                        var invoiceTypeSelectorPayload;
                        if (financialAccountDefinitionSettings != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: financialAccountDefinitionSettings.InvoiceTypeId };
                        }
                        return invoiceTypeSelectorAPI.load(invoiceTypeSelectorPayload);
                    }

                    promises.push(loadInvoiceTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Retail.BusinessEntity.Entities.FinancialAccountDefinitionSettings, Retail.BusinessEntity.Entities",
                            InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);