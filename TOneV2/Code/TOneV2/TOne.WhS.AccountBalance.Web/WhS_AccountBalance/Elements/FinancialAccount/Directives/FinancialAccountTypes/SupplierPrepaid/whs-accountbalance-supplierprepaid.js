"use strict";

app.directive("whsAccountbalanceSupplierprepaid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierPostPaid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/FinancialAccountTypes/SupplierPrepaid/Templates/SupplierPrepaidDefinitionSettings.html"

        };

        function SupplierPostPaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var usageTransactionTypeApi;
            var usageTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onUsageTransactionTypeReady = function (api) {
                    usageTransactionTypeApi = api;
                    usageTransactionTypePromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];
                    promises.push(usageTransactionTypeLoadPromise());

                    function usageTransactionTypeLoadPromise() {
                        var usageTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        usageTransactionTypePromiseDeferred.promise.then(function () {
                            var definitionSettingsPayload;
                            if (payload != undefined) {
                                definitionSettingsPayload = {
                                    selectedIds: extendedSettingsEntity != undefined ? extendedSettingsEntity.UsageTransactionTypeId : undefined
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(usageTransactionTypeApi, definitionSettingsPayload, usageTransactionTypeLoadDeferred);
                        });
                        return usageTransactionTypeLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPrepaid.SupplierPrepaidDefinitionSettings ,TOne.WhS.AccountBalance.MainExtensions",
                        UsageTransactionTypeId: usageTransactionTypeApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);