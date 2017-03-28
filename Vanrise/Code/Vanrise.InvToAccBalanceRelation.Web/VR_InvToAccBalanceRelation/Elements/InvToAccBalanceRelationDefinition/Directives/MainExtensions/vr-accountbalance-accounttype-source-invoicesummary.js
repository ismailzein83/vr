'use strict';

app.directive('vrAccountbalanceAccounttypeSourceInvoicesummary', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var invoiceSourceSetting = new InvoiceSourceSetting($scope, ctrl, $attrs);
                invoiceSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_InvToAccBalanceRelation/Elements/InvToAccBalanceRelationDefinition/Directives/MainExtensions/Templates/InvoiceSummarySourceSetting.html'
        };

        function InvoiceSourceSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var transactionTypeSelectorAPI;
            var transactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                    transactionTypeSelectorAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, transactionTypeSelectorAPI, undefined, setLoader, transactionTypeSelectorReadyPromiseDeferred);
                 
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var invoiceSource;
                    if (payload != undefined) {
                        invoiceSource = payload.sourceSettingEntity;
                        if (invoiceSource != undefined)
                            $scope.scopeModel.calculateDueAmount = invoiceSource.CalculateDueAmount;
                    }
                    var promises = [];
                    if (invoiceSource.CalculateDueAmount != undefined)
                         promises.push(loadTransactionTypeSelector());

                    function loadTransactionTypeSelector() {
                        var transactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        transactionTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            transactionTypeSelectorReadyPromiseDeferred = undefined;
                            var transactionTypePayload;
                            if (invoiceSource != undefined)
                                transactionTypePayload = {
                                    selectedIds: invoiceSource.TransactionTypeIds
                                };
                            VRUIUtilsService.callDirectiveLoad(transactionTypeSelectorAPI, transactionTypePayload, transactionTypeSelectorLoadPromiseDeferred);
                        });
                        return transactionTypeSelectorLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.InvToAccBalanceRelation.Business.InvoiceSummaryFieldSourceSetting, Vanrise.InvToAccBalanceRelation.Business",
                        CalculateDueAmount: $scope.scopeModel.calculateDueAmount,
                        TransactionTypeIds: transactionTypeSelectorAPI.getSelectedIds(),
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
