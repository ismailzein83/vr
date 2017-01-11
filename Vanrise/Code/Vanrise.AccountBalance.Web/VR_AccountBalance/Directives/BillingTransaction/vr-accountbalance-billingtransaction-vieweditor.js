"use strict";

app.directive("vrAccountbalanceBillingtransactionVieweditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/BillingTransaction/Templates/BillingTransactionViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountTypeSelectorApi;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
              
                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorApi = api;
                    accountTypeSelectorPromiseDeferred.resolve();
                };
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
                                selectedIds: payload != undefined ? getIdsFromItems(payload.Items) : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorApi, payloadSelector, accountTypeSelectorLoadDeferred);
                        });
                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.AccountBalance.Entities.BillingTransactionViewSettings, Vanrise.AccountBalance.Entities",
                        Items: buildAccountTypeViewItems()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildAccountTypeViewItems() {
                var tab = [];
                var selectedTypes = accountTypeSelectorApi.getSelectedIds();
                for (var i = 0 ; i < selectedTypes.length; i++) {
                    tab.push({ AccountTypeId: selectedTypes[i] });
                }
                return tab;
            }
            function getIdsFromItems(items) {
                var tab = [];
                var selectedTypes = accountTypeSelectorApi.getSelectedIds();
                for (var i = 0 ; i < items.length; i++) {
                    tab.push(items[i].AccountTypeId);
                }
                return tab;
            }


        }

        return directiveDefinitionObject;
    }
]);