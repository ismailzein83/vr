'use strict';

app.directive('retailBeRetrievefinancialinfostep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RetrieveFinancialInfoStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/TransformationSteps/Templates/RetrieveFinancialInfoStepTemplate.html';
            }
        };

        function RetrieveFinancialInfoStepCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var stepPayload;

            //Input Fields
            var accountBEDefinitionIdDirectiveReadyAPI;
            var accountBEDefinitionIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountIdDirectiveReadyAPI;
            var accountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveReadyAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var amountDirectiveReadyAPI;
            var amountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyIdDirectiveReadyAPI;
            var currencyIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var transactionTypeIdDirectiveReadyAPI;
            var transactionTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var updateBalanceRecordListDirectiveReadyAPI;
            var updateBalanceRecordListDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //Output Fields
            var financialAccountIdDirectiveReadyAPI;
            var financialAccountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var balanceAccountIdDirectiveReadyAPI;
            var balanceAccountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var classificationDirectiveReadyAPI;
            var classificationDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                //Input Fields
                $scope.scopeModel.onAccountBEDefinitionIdReady = function (api) {
                    accountBEDefinitionIdDirectiveReadyAPI = api;
                    accountBEDefinitionIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAccountIdReady = function (api) {
                    accountIdDirectiveReadyAPI = api;
                    accountIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onEffectiveOnReady = function (api) {
                    effectiveOnDirectiveReadyAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAmountReady = function (api) {
                    amountDirectiveReadyAPI = api;
                    amountDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCurrencyIdReady = function (api) {
                    currencyIdDirectiveReadyAPI = api;
                    currencyIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onTransactionTypeIdReady = function (api) {
                    transactionTypeIdDirectiveReadyAPI = api;
                    transactionTypeIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onUpdateBalanceRecordListReady = function (api) {
                    updateBalanceRecordListDirectiveReadyAPI = api;
                    updateBalanceRecordListDirectiveReadyPromiseDeferred.resolve();
                };

                //Output Fields
                $scope.scopeModel.onFinancialAccountIdReady = function (api) {
                    financialAccountIdDirectiveReadyAPI = api;
                    financialAccountIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onBalanceAccountIdReady = function (api) {
                    balanceAccountIdDirectiveReadyAPI = api;
                    balanceAccountIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onClassificationReady = function (api) {
                    classificationDirectiveReadyAPI = api;
                    classificationDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Input
                    //Loading AccountBEDefinitionId Directive
                    var accountBEDefinitionIdDirectiveLoadPromiseDeferred = getAccountBEDefinitionIdDirectiveLoadPromiseDeferred();
                    promises.push(accountBEDefinitionIdDirectiveLoadPromiseDeferred.promise);

                    //Loading AccountId Directive
                    var accountIdDirectiveLoadPromiseDeferred = getAccountIdDirectiveLoadPromiseDeferred();
                    promises.push(accountIdDirectiveLoadPromiseDeferred.promise);

                    //Loading EffectiveOn Directive
                    var effectiveOnDirectiveLoadPromiseDeferred = getEffectiveOnDirectiveLoadPromiseDeferred();
                    promises.push(effectiveOnDirectiveLoadPromiseDeferred.promise);

                    //Loading Amount Directive
                    var amountDirectivePromiseDeferred = getAmountDirectiveLoadPromiseDeferred();
                    promises.push(amountDirectivePromiseDeferred.promise);

                    //Loading CurrencyId Directive
                    var currencyIdDirectivePromiseDeferred = getCurrencyIdDirectiveLoadPromiseDeferred();
                    promises.push(currencyIdDirectivePromiseDeferred.promise);

                    //Loading TransactionTypeId Directive
                    var transactionTypeIdDirectivePromiseDeferred = getTransactionTypeIdDirectiveLoadPromiseDeferred();
                    promises.push(transactionTypeIdDirectivePromiseDeferred.promise);

                    //Loading UpdateBalanceRecordList Directive
                    var updateBalanceRecordListDirectivePromiseDeferred = getUpdateBalanceRecordListDirectivePromiseDeferred();
                    promises.push(updateBalanceRecordListDirectivePromiseDeferred.promise);

                    //Output
                    //Loading FinancialAccountId Directive
                    var financialAccountIdDirectivePromiseDeferred = getFinancialAccountIdDirectivePromiseDeferred();
                    promises.push(financialAccountIdDirectivePromiseDeferred.promise);

                    //Loading BalanceAccountId Directive
                    var balanceAccountIdDirectivePromiseDeferred = getBalanceAccountIdDirectivePromiseDeferred();
                    promises.push(balanceAccountIdDirectivePromiseDeferred.promise);

                    //Loading Classification Directive
                    var classificationDirectivePromiseDeferred = getBalanceAccountIdDirectivePromiseDeferred();
                    promises.push(classificationDirectivePromiseDeferred.promise);

                    function getAccountBEDefinitionIdDirectiveLoadPromiseDeferred() {
                        var accountBEDefinitionIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        accountBEDefinitionIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var accountBEDefinitionIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                accountBEDefinitionIdPayload.selectedRecords = payload.stepDetails.AccountBEDefinitionId;

                            VRUIUtilsService.callDirectiveLoad(accountBEDefinitionIdDirectiveReadyAPI, accountBEDefinitionIdPayload, accountBEDefinitionIdDirectiveLoadPromiseDeferred);
                        });

                        return accountBEDefinitionIdDirectiveLoadPromiseDeferred;
                    }
                    function getAccountIdDirectiveLoadPromiseDeferred() {
                        var accountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        accountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var accountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                accountIdPayload.selectedRecords = payload.stepDetails.AccountId;

                            VRUIUtilsService.callDirectiveLoad(accountIdDirectiveReadyAPI, accountIdPayload, accountIdDirectiveLoadPromiseDeferred);
                        });

                        return accountIdDirectiveLoadPromiseDeferred;
                    }
                    function getEffectiveOnDirectiveLoadPromiseDeferred() {
                        var effectiveOnDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        effectiveOnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var effectiveOnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                effectiveOnPayload.selectedRecords = payload.stepDetails.EffectiveOn;

                            VRUIUtilsService.callDirectiveLoad(effectiveOnDirectiveReadyAPI, effectiveOnPayload, effectiveOnDirectiveLoadPromiseDeferred);
                        });

                        return effectiveOnDirectiveLoadPromiseDeferred;
                    }
                    function getAmountDirectiveLoadPromiseDeferred() {
                        var amountDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        amountDirectiveReadyPromiseDeferred.promise.then(function () {

                            var amountPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                amountPayload.selectedRecords = payload.stepDetails.Amount;

                            VRUIUtilsService.callDirectiveLoad(amountDirectiveReadyAPI, amountPayload, amountDirectiveLoadPromiseDeferred);
                        });

                        return amountDirectiveLoadPromiseDeferred;
                    }
                    function getCurrencyIdDirectiveLoadPromiseDeferred() {
                        var currencyIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var currencyIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                currencyIdPayload.selectedRecords = payload.stepDetails.CurrencyId;

                            VRUIUtilsService.callDirectiveLoad(currencyIdDirectiveReadyAPI, currencyIdPayload, currencyIdDirectiveLoadPromiseDeferred);
                        });

                        return currencyIdDirectiveLoadPromiseDeferred;
                    }
                    function getTransactionTypeIdDirectiveLoadPromiseDeferred() {
                        var transactionTypeIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        transactionTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var transactionTypeIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                transactionTypeIdPayload.selectedRecords = payload.stepDetails.TransactionTypeId;

                            VRUIUtilsService.callDirectiveLoad(transactionTypeIdDirectiveReadyAPI, transactionTypeIdPayload, transactionTypeIdDirectiveLoadPromiseDeferred);
                        });

                        return transactionTypeIdDirectiveLoadPromiseDeferred;
                    }
                    function getUpdateBalanceRecordListDirectivePromiseDeferred() {
                        var updateBalanceRecordListDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        updateBalanceRecordListDirectiveReadyPromiseDeferred.promise.then(function () {

                            var updateBalanceRecordListPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                updateBalanceRecordListPayload.selectedRecords = payload.stepDetails.UpdateBalanceRecordList;

                            VRUIUtilsService.callDirectiveLoad(updateBalanceRecordListDirectiveReadyAPI, updateBalanceRecordListPayload, updateBalanceRecordListDirectiveLoadPromiseDeferred);
                        });

                        return updateBalanceRecordListDirectiveLoadPromiseDeferred;
                    }

                    function getFinancialAccountIdDirectivePromiseDeferred() {
                        var financialAccountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        financialAccountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var financialAccountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                financialAccountIdPayload.selectedRecords = payload.stepDetails.FinancialAccountId;

                            VRUIUtilsService.callDirectiveLoad(financialAccountIdDirectiveReadyAPI, financialAccountIdPayload, financialAccountIdDirectiveLoadPromiseDeferred);
                        });

                        return financialAccountIdDirectiveLoadPromiseDeferred;
                    }
                    function getBalanceAccountIdDirectivePromiseDeferred() {
                        var balanceAccountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        balanceAccountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var balanceAccountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                balanceAccountIdPayload.selectedRecords = payload.stepDetails.BalanceAccountId;

                            VRUIUtilsService.callDirectiveLoad(balanceAccountIdDirectiveReadyAPI, balanceAccountIdPayload, balanceAccountIdDirectiveLoadPromiseDeferred);
                        });

                        return balanceAccountIdDirectiveLoadPromiseDeferred;
                    }

                    function getClassificationDirectivePromiseDeferred() {
                        var classificationDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        classificationDirectiveReadyPromiseDeferred.promise.then(function () {

                            var classificationPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                classificationPayload.selectedRecords = payload.stepDetails.Classification;

                            VRUIUtilsService.callDirectiveLoad(classificationDirectiveReadyAPI, classificationPayload, classificationDirectiveReadyPromiseDeferred);
                        });

                        return classificationDirectiveReadyPromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.TransformationSteps.RetrieveFinancialInfoStep, Retail.BusinessEntity.MainExtensions",
                        AccountBEDefinitionId: accountBEDefinitionIdDirectiveReadyAPI.getData(),
                        AccountId: accountIdDirectiveReadyAPI.getData(),
                        EffectiveOn: effectiveOnDirectiveReadyAPI.getData(),
                        Amount: amountDirectiveReadyAPI.getData(),
                        CurrencyId: currencyIdDirectiveReadyAPI.getData(),
                        TransactionTypeId: transactionTypeIdDirectiveReadyAPI.getData(),
                        UpdateBalanceRecordList: updateBalanceRecordListDirectiveReadyAPI.getData(),
                        Classification: classificationDirectiveReadyAPI.getData(),
                        FinancialAccountId: financialAccountIdDirectiveReadyAPI.getData(),
                        BalanceAccountId: balanceAccountIdDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);

