'use strict';

app.directive('retailBeAccountsynchronizerhandlerGrid', ['Retail_BE_AccountSynchronizerHandlerService', 'UtilsService',
    function (Retail_BE_AccountSynchronizerHandlerService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountsynchronizerhandlerGrid = new AccountsynchronizerhandlerGrid($scope, ctrl, $attrs);
                accountsynchronizerhandlerGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountSynchronizerHandler/Templates/AccountSynchronizerHandlerGridTemplate.html'
        };

        function AccountsynchronizerhandlerGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            var accountBeDefinition;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.accountSynchronizerHandlers = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addHandler = function () {
                    addAccountSynchronizerHandler();
                };

                $scope.scopeModel.removeAccountSynchronizerHandler = function (inbound) {
                    $scope.scopeModel.accountSynchronizerHandlers.splice($scope.scopeModel.accountSynchronizerHandlers.indexOf(inbound), 1);
                };

                $scope.scopeModel.validateaAccountSynchronizerHandlers = function () {
                    if ($scope.scopeModel.accountSynchronizerHandlers.length == 0)
                        return 'Add at least 1 Account Synchronizer Handler';
                    return null;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var accountSynchronizerHandlers;

                    if (payload != undefined) {
                        accountSynchronizerHandlers = payload.AccountSynchronizerHandlers;
                        accountBeDefinition = payload.AccountBEDefinitionId;
                    }

                    if (accountSynchronizerHandlers != undefined) {

                        var entities = getAccountSynchronizerHandlersEntities();

                        for (var i = 0; i < accountSynchronizerHandlers.length; i++) {

                            var index = UtilsService.getItemIndexByVal(entities, accountSynchronizerHandlers[i].Name, 'Name');
                            if (index == -1)
                                continue;

                            var existingRecord = $scope.scopeModel.accountSynchronizerHandlers[index];
                            if (existingRecord == undefined)
                                continue;

                            var updatedEntity = UtilsService.cloneObject(existingRecord.Entity);

                            $scope.scopeModel.accountSynchronizerHandlers[index] = { Entity: updatedEntity };
                        }
                    }

                };

                api.getData = function () {
                    return getAccountSynchronizerHandlersEntities();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editAccountSynchronizerHandler
                }];
            }

            function addAccountSynchronizerHandler() {

                var onAccountSynchronizerInsertHandlerAdded = function (addedHandler) {
                    var obj = { Entity: addedHandler };
                    $scope.scopeModel.accountSynchronizerHandlers.push(obj);

                };
                Retail_BE_AccountSynchronizerHandlerService.addAccountSynchronizerHandler(accountBeDefinition, onAccountSynchronizerInsertHandlerAdded);
            }

            function editAccountSynchronizerHandler(entity) {

                var onAccountSynchronizerInsertHandlerUpdated = function (updatedHandler) {
                    var obj = { Entity: updatedHandler };
                    $scope.scopeModel.accountSynchronizerHandlers[$scope.scopeModel.accountSynchronizerHandlers.indexOf(entity)] = obj;

                };
                Retail_BE_AccountSynchronizerHandlerService.editAccountSynchronizerHandler(entity, accountBeDefinition, onAccountSynchronizerInsertHandlerUpdated);
            }

            function getAccountSynchronizerHandlersEntities() {
                return UtilsService.getPropValuesFromArray($scope.scopeModel.accountSynchronizerHandlers, 'Entity');
            }
        }

    }]);