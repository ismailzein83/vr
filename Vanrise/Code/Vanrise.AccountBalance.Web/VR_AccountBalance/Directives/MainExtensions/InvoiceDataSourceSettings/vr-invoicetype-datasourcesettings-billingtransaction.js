"use strict";

app.directive("vrInvoicetypeDatasourcesettingsBillingtransaction", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingtransactionDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/InvoiceDataSourceSettings/Templates/BillingTransactionDataSource.html"

        };

        function BillingtransactionDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var transactionTypeSelectorAPI;
            var transactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                    transactionTypeSelectorAPI = api;
                    transactionTypeSelectorReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([transactionTypeSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataSourceEntity;
                    if (payload != undefined) {
                        dataSourceEntity = payload.dataSourceEntity;
                        if(dataSourceEntity != undefined)
                        {
                            $scope.scopeModel.dataSourceName = dataSourceEntity.DataSourceName;
                            $scope.scopeModel.topRecords = dataSourceEntity.TopRecords;
                        }
                    }
                    var promises = [];
                    function loadTransactionTypeSelector() {
                        var transactionTypePayload = {
                            selectedIds: dataSourceEntity != undefined ? dataSourceEntity.TransactionTypesIds : undefined
                        };
                        return transactionTypeSelectorAPI.load(transactionTypePayload);
                    }
                    promises.push(loadTransactionTypeSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.AccountBalance.MainExtensions.BillingTransactionDataSourceSettings ,Vanrise.AccountBalance.MainExtensions",
                        DataSourceType:1,
                        DataSourceName:$scope.scopeModel.dataSourceName,
                        TransactionTypesIds: transactionTypeSelectorAPI.getSelectedIds(),
                        TopRecords: $scope.scopeModel.topRecords
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);