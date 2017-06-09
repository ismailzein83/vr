"use strict";

app.directive("retailMultinetInvoicegeneratorfilterconditionAccounttype", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AccountTypeCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Elements/InvoiceType/InvoiceGeneratorFilterCondition/Templates/AccountTypeConditionTemplate.html"

        };

        function AccountTypeCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();

                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        context = payload.context;
                        if (invoiceFilterConditionEntity != undefined) {
                        }
                    }
                    var promises = [];

                    function loadAccountTypeSelector() {
                        var accountTypeSelectorPayload = {};
                        if (invoiceFilterConditionEntity != undefined) {
                            accountTypeSelectorPayload.selectedIds = invoiceFilterConditionEntity.AccountTypeId;
                        }
                        return accountTypeSelectorAPI.load(accountTypeSelectorPayload);
                    }
                    promises.push(loadAccountTypeSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.MultiNet.Business.InvoiceGeneratorActionAccountTypeCondition ,Retail.MultiNet.Business",
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);