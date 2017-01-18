'use strict';

app.directive('vrAccountbalanceBillingtransactionSynchronizer', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRObjectVariableService', 'VRCommon_VRObjectTypeDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRObjectVariableService, VRCommon_VRObjectTypeDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new billingTransactionSynchronizerEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/BillingTransaction/Templates/BillingTransactionSynchronizer.html"
        };

        function billingTransactionSynchronizerEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountTypeSelectorApi;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorApi = api;
                accountTypeSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    promises.push(loadAccountTypeSelector());

                    function loadAccountTypeSelector() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? payload.BalanceAccountTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorApi, payloadSelector, accountTypeSelectorLoadDeferred);
                        });
                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.AccountBalance.MainExtensions.BillingTransaction.BillingTransactionSynchronizer, Vanrise.AccountBalance.MainExtensions",
                        Name: "Billing Transaction Synchronizer",
                        BalanceAccountTypeId: accountTypeSelectorApi.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);