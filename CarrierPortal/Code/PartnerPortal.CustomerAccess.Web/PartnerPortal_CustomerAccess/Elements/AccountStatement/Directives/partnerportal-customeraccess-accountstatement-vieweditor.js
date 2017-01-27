"use strict";

app.directive("partnerportalCustomeraccessAccountstatementVieweditor", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/AccountStatement/Directives/Templates/AccountSatementViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountStatementHandlerApi;
            var accountStatementHandlerPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAccountStatementHandlerReady = function (api) {
                    accountStatementHandlerApi = api;
                    accountStatementHandlerPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        $scope.scopeModel.accountTypeId = payload.AccountStatementViewData.AccountTypeId;
                    }

                    var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    connectionSelectorPromiseDeferred.promise.then(function () {
                        var payloadSelector = {
                            filter: { Filters: [] }
                        };
                        payloadSelector.filter.Filters.push({
                            $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                        });
                        if (payload != undefined && payload.AccountStatementViewData != undefined) {
                            payloadSelector.selectedIds = payload.AccountStatementViewData.VRConnectionId;
                        };
                        VRUIUtilsService.callDirectiveLoad(connectionSelectorApi, payloadSelector, connectionSelectorLoadDeferred);
                    });
                    promises.push(connectionSelectorLoadDeferred.promise);

                    var accountStatementHandlerLoadDeferred = UtilsService.createPromiseDeferred();
                    accountStatementHandlerPromiseDeferred.promise.then(function () {
                        var accountStatementHandlerPayload;
                        if (payload != undefined && payload.AccountStatementViewData != undefined) {
                            accountStatementHandlerPayload = payload.AccountStatementViewData.AccountStatementHandler;
                        };
                        VRUIUtilsService.callDirectiveLoad(accountStatementHandlerApi, accountStatementHandlerPayload, accountStatementHandlerLoadDeferred);
                    });
                    promises.push(accountStatementHandlerLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "PartnerPortal.CustomerAccess.Entities.AccountStatementViewSettings, PartnerPortal.CustomerAccess.Entities",
                        AccountStatementViewData:
                            {
                                VRConnectionId: connectionSelectorApi.getSelectedIds(),
                                AccountTypeId: $scope.scopeModel.accountTypeId,
                                AccountStatementHandler: accountStatementHandlerApi.getData()
                            }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);