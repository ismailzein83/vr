"use strict";

app.directive("retailInvoicetypeInvoicefilterconditionMultinetpartner", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MultiNetPartnerCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Elements/InvoiceType/InvoiceGridActionFilter/Templates/MultiNetPartnerCondition.html"

        };

        function MultiNetPartnerCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var accountConditionAPI;
            var accountConditionReadyDeferred  = UtilsService.createPromiseDeferred();

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onAccountConditionDirectiveReady = function(api)
                {
                    accountConditionAPI = api;
                    accountConditionReadyDeferred.resolve();
                }
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([accountConditionReadyDeferred.promise,businessEntityDefinitionSelectorReadyDeferred.promise]).then(function(){
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        context = payload.context;
                    }
                    var promises = [];
                    function loadBusinessEntityDefinitionSelector() {
                        var payload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                }]
                            },
                            selectedIds: invoiceFilterConditionEntity != undefined ? invoiceFilterConditionEntity.AccountBEDefinitionId : undefined
                        };
                        return businessEntityDefinitionSelectorAPI.load(payload);
                    }

                    promises.push(loadBusinessEntityDefinitionSelector());
                   
                    function AccountConditionSelectiveDirectiveLoadPromise() {
                        var accountConditionSelectivePayload = {
                            accountBEDefinitionId: invoiceFilterConditionEntity != undefined ? invoiceFilterConditionEntity.AccountBEDefinitionId : undefined,
                            beFilter: invoiceFilterConditionEntity != undefined ? invoiceFilterConditionEntity.AccountCondition : undefined
                        };
                        return accountConditionAPI.load(accountConditionSelectivePayload);
                    }
                    promises.push(AccountConditionSelectiveDirectiveLoadPromise());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.MultiNet.Business.MultiNetPartnerCondition ,Retail.MultiNet.Business",
                        AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                        AccountCondition: accountConditionAPI.getData()
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