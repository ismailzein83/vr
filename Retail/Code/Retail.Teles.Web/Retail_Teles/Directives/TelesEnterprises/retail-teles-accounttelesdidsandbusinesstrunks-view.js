
'use strict';

app.directive('retailTelesAccounttelesdidsandbusinesstrunksView', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountTelesDIDsAndBusinessTrunksCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/TelesEnterprises/Templates/AccountTelesDIDsAndBusinessTrunksView.html'
        };

        function AccountTelesDIDsAndBusinessTrunksCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var didsGridAPI;
            var didsGridReadyDeferred = UtilsService.createPromiseDeferred();
            var businessTrunksAPI;
            var businessTrunksReadyDeferred = UtilsService.createPromiseDeferred();
            var accountBEDefinitionId;
            var accountId;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onDIDsGridReady = function (api) {
                    didsGridAPI = api;
                    didsGridReadyDeferred.resolve();
                };
                $scope.scopeModel.onBusinessTrunksGridReady = function (api) {
                    businessTrunksAPI = api;
                    businessTrunksReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountViewDefinition;
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountId = payload.parentAccountId;
                        accountViewDefinition = payload.accountViewDefinition;
                    }
                    promises.push(loadDIDsGrid());

                    function loadDIDsGrid() {
                        var didsGridLoadDeferred = UtilsService.createPromiseDeferred();

                        didsGridReadyDeferred.promise.then(function () {
                            var didsGridPayload = {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                AccountId: accountId
                            };
                            if (accountViewDefinition != undefined && accountViewDefinition.Settings != undefined) {
                                didsGridPayload.VRConnectionId = accountViewDefinition.Settings.VRConnectionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(didsGridAPI, didsGridPayload, didsGridLoadDeferred);
                        });
                        return didsGridLoadDeferred.promise
                    }

                    promises.push(loadBusinessTrunks());

                    function loadBusinessTrunks() {
                        var businessTrunksLoadDeferred = UtilsService.createPromiseDeferred();

                        businessTrunksReadyDeferred.promise.then(function () {
                            var businessTrunksGridPayload = {
                                AccountBEDefinitionId: accountBEDefinitionId,
                                AccountId: accountId
                            };
                            if (accountViewDefinition != undefined && accountViewDefinition.Settings != undefined) {
                                businessTrunksGridPayload.VRConnectionId = accountViewDefinition.Settings.VRConnectionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(businessTrunksAPI, businessTrunksGridPayload, businessTrunksLoadDeferred);
                        });
                        return businessTrunksLoadDeferred.promise
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);