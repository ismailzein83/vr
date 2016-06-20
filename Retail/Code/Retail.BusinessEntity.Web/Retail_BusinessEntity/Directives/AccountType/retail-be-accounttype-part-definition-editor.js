(function (app) {

    'use strict';

    AccountTypePartDefinitionEditorDirective.$inject = ['Retail_BE_AccountTypeService', 'UtilsService'];

    function AccountTypePartDefinitionEditorDirective(Retail_BE_AccountTypeService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountTypePartDefinitionEditor = new AccountTypePartDefinitionEditor($scope, ctrl, $attrs);
                accountTypePartDefinitionEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountType/Templates/AccountTypePartDefinitionTemplate.html"
        };

        function AccountTypePartDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var templateConfigs = [];
            function initializeController() {
                ctrl.accountParts = [];

                ctrl.isValidAccountParts = function () {

                    if (ctrl.accountParts.length > 0)
                        return null;
                    return "At least one account part should be added.";
                }

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addAccountPart = function () {
                    var onAccountPartDefinitionAdded = function (accountPart) {
                        ctrl.accountParts.push({ Entity: accountPart });
                    }
                    Retail_BE_AccountTypeService.addAccountPartDefinition(onAccountPartDefinitionAdded);
                }

                ctrl.removeAccountPart = function (accountPart) {
                    ctrl.accountParts.splice(ctrl.accountParts.indexOf(accountPart), 1);
                }

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.accountParts.length = 0;
                        if (payload.partDefinitions !=undefined && payload.partDefinitions.length > 0) {
                            for (var y = 0; y < payload.partDefinitions.length; y++) {
                                var currentAccounType = payload.partDefinitions[y];
                                ctrl.accountParts.push({ Entity: currentAccounType });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var accountParts = [];
                    for (var i = 0; i < ctrl.accountParts.length ; i++) {
                        var accountPart = ctrl.accountParts[i];
                        accountParts.push(accountPart.Entity);
                    }
                    return accountParts;
                }
                return api;
            }

            function defineMenuActions() {
                ctrl.accountPartsGridMenuActions = [{
                    name: 'Edit',
                    clicked: editAccountType
                }];
            }

            function editAccountType(accountType) {
                var onAccountPartDefinitionUpdated = function (accountTypeObj) {
                    ctrl.accountParts[ctrl.accountParts.indexOf(accountType)] = { Entity: accountTypeObj };
                }
                Retail_BE_AccountTypeService.editAccountPartDefinition(accountType.Entity, onAccountPartDefinitionUpdated);
            }
        }
    }

    app.directive('retailBeAccounttypePartDefinitionEditor', AccountTypePartDefinitionEditorDirective);

})(app);