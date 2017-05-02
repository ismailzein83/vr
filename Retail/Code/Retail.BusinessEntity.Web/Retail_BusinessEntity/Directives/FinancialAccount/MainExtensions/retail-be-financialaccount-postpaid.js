'use strict';

app.directive('retailBeFinancialaccountPostpaid', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new postpaidFinancialAccountCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/MainExtensions/Templates/PostpaidFinancialAccount.html"
        };

        function postpaidFinancialAccountCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var creditClassSelectorAPI;
            var creditClassSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCreditClassSelectorReady = function (api) {
                    creditClassSelectorAPI = api;
                    creditClassSelectorReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([creditClassSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                    }

                    var promises = [];

                    var creditClassSelectorLoadPromise = loadCreditClassSelector();
                    promises.push(creditClassSelectorLoadPromise);

                    function loadCreditClassSelector() {
                        var payloadSelector = {
                            selectedIds: extendedSettings != undefined ? extendedSettings.CreditClassId : undefined
                        };
                        return creditClassSelectorAPI.load(payloadSelector);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.FinancialAccount.PostpaidFinancialAccount, Retail.BusinessEntity.MainExtensions",
                        CreditClassId: creditClassSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);